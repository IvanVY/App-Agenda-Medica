using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AgendaMedicaApp.Models;
using AgendaMedicaApp.Utils;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace AgendaMedicaApp.Services
{
    public class FirebaseService
    {
        private FirestoreDb firestoreDb;

        public FirebaseService()
        {
            try
            {
                var credentialsPath = FirebaseConfig.CredentialsPath;
                var projectId = FirebaseConfig.ProjectId;

                if (!File.Exists(credentialsPath))
                {
                    throw new FileNotFoundException("El archivo de credenciales no fue encontrado. Verifica la ruta.");
                }

                Console.WriteLine($"Archivo de credenciales encontrado en: {credentialsPath}");

                var credential = GoogleCredential.FromFile(credentialsPath);

                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = credential,
                        ProjectId = projectId
                    });
                }

                var builder = new FirestoreDbBuilder
                {
                    ProjectId = projectId,
                    Credential = credential
                };

                firestoreDb = builder.Build();
                Console.WriteLine("Firestore inicializado correctamente.");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al inicializar Firebase: {ex.Message}");
                throw;
            }
        }

        public FirestoreDb FirestoreDb => firestoreDb;

        public async Task<string> RegisterUserAsync(string email, string password, string name, string type)
        {
            try
            {
                // Crear el usuario en Firebase Authentication
                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
                {
                    Email = email,
                    Password = password,
                    DisplayName = name
                });

                // Guardar información adicional en Firestore
                var userDoc = firestoreDb.Collection("users").Document(userRecord.Uid);
                await userDoc.SetAsync(new
                {
                    Name = name,
                    Email = email,
                    Type = type
                });

                return userRecord.Uid;
            }
            catch (FirebaseAuthException authEx)
            {
                throw new Exception($"Error de autenticación al registrar usuario: {authEx.Message}");
            }
            catch (Exception firestoreEx) // Captura excepciones genéricas
            {
                throw new Exception($"Error de Firestore al registrar usuario: {firestoreEx.Message}");
            }
        }
        public async Task<string> RegisterDoctorAsync(string email, string password, string name, string specialty)
        {
            try
            {
                // Crear el usuario en Firebase Authentication
                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
                {
                    Email = email,
                    Password = password,
                    DisplayName = name
                });

                // Guardar información adicional en Firestore
                var doctorDoc = firestoreDb.Collection("doctors").Document(userRecord.Uid);
                await doctorDoc.SetAsync(new
                {
                    Name = name,
                    Specialty = specialty,
                    Email = email,
                    Type = "doctor" // Rol del doctor
                });

                return userRecord.Uid;
            }
            catch (FirebaseAuthException authEx)
            {
                throw new Exception($"Error de autenticación al registrar doctor: {authEx.Message}");
            }
            catch (Exception firestoreEx)
            {
                throw new Exception($"Error de Firestore al registrar doctor: {firestoreEx.Message}");
            }
        }
        public async Task<List<Appointment>> GetUserAppointmentsAsync(string userId)
        {
            try
            {
                var query = firestoreDb.Collection("appointments").WhereEqualTo("UserId", userId);
                var snapshot = await query.GetSnapshotAsync();

                return snapshot.Documents
                    .Select(doc => doc.ConvertTo<Appointment>())
                    .ToList();
            }
            catch (Exception firestoreEx) // Captura excepciones genéricas
            {
                throw new Exception($"Error de Firestore al obtener citas del usuario: {firestoreEx.Message}");
            }
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            try
            {
                Console.WriteLine("Cargando todas las citas...");
                var query = firestoreDb.Collection("appointments");
                var snapshot = await query.GetSnapshotAsync();

                return snapshot.Documents
                    .Select(doc => doc.ConvertTo<Appointment>())
                    .ToList();
            }
            catch (Exception firestoreEx) // Captura excepciones genéricas
            {
                throw new Exception($"Error de Firestore al cargar todas las citas: {firestoreEx.Message}");
            }
        }

        public async Task AddAppointmentAsync(string userId, string doctorId, string date, string time, string doctorName, string specialty)
        {
            try
            {
                // Validar que la hora de la cita no sea anterior a la hora actual
                DateTime appointmentDateTime = DateTime.Parse($"{date} {time}");
                if (appointmentDateTime <= DateTime.Now.AddHours(1))
                {
                    throw new Exception("La cita debe ser programada al menos una hora después de la hora actual.");
                }

                // Verificar si ya existe una cita para el mismo doctor en la misma fecha y hora
                var query = firestoreDb.Collection("appointments")
                    .WhereEqualTo("DoctorId", doctorId)
                    .WhereEqualTo("Date", date)
                    .WhereEqualTo("Time", time);

                var snapshot = await query.GetSnapshotAsync();

                if (snapshot.Documents.Any())
                {
                    throw new Exception("Ya existe una cita programada para este doctor en la fecha y hora seleccionadas.");
                }

                // Guardar la cita en Firestore con estado "Pendiente"
                await firestoreDb.Collection("appointments").AddAsync(new
                {
                    UserId = userId,
                    DoctorId = doctorId,
                    DoctorName = doctorName,
                    Specialty = specialty,
                    Date = date,
                    Time = time,
                    Status = "Pendiente" // Estado inicial de la cita
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar cita: {ex.Message}");
            }
        }

        public async Task<(bool success, string userId, string userType)> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                // Verificar si el correo existe en Firebase Authentication
                var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

                // Buscar el usuario en la colección "users"
                var userDoc = await firestoreDb.Collection("users").Document(userRecord.Uid).GetSnapshotAsync();

                if (userDoc.Exists && userDoc.ContainsField("Type"))
                {
                    // Usuario encontrado en la colección "users"
                    string userTypeFromFirestore = userDoc.GetValue<string>("Type");
                    return (true, userRecord.Uid, userTypeFromFirestore);
                }

                // Si no se encuentra en "users", buscar en la colección "doctors"
                var doctorDoc = await firestoreDb.Collection("doctors").Document(userRecord.Uid).GetSnapshotAsync();

                if (doctorDoc.Exists && doctorDoc.ContainsField("Type"))
                {
                    // Doctor encontrado en la colección "doctors"
                    string userTypeFromFirestore = doctorDoc.GetValue<string>("Type");
                    return (true, userRecord.Uid, userTypeFromFirestore);
                }

                // Si no se encuentra ni en "users" ni en "doctors"
                return (false, null, null); // El usuario no tiene un tipo definido
            }
            catch (FirebaseAuthException)
            {
                return (false, null, null); // Credenciales incorrectas
            }
        }
    }
}