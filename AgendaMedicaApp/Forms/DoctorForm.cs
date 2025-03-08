using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgendaMedicaApp.Models;
using AgendaMedicaApp.Services;

namespace AgendaMedicaApp.Forms
{
    public partial class DoctorForm : Form
    {
        private string userId;
        private FirebaseService firebaseService;

        public DoctorForm(string userId, FirebaseService firebaseService)
        {
            InitializeComponent();
            this.userId = userId;
            this.firebaseService = firebaseService;

            // Configurar los controles
            ConfigureDataGridView();
            LoadAppointmentsAsync();

            // Cargar las opciones del ComboBox
            cmbStatus.Items.AddRange(new string[] { "Pendiente", "Confirmado", "Completado" });
            cmbStatus.SelectedIndex = 0; // Seleccionar la primera opción por defecto
        }

        private void ConfigureDataGridView()
        {
            dataGridViewAppointments.AutoGenerateColumns = false;

            // Columna oculta para el ID de la cita
            dataGridViewAppointments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AppointmentId",
                Name = "AppointmentId",
                Visible = false // Ocultar esta columna
            });

            dataGridViewAppointments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "UserName",
                HeaderText = "Usuario",
                Name = "UserName"
            });

            dataGridViewAppointments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DoctorName",
                HeaderText = "Doctor",
                Name = "DoctorName"
            });

            dataGridViewAppointments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Date",
                HeaderText = "Fecha",
                Name = "Date"
            });

            dataGridViewAppointments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Time",
                HeaderText = "Hora",
                Name = "Time"
            });

            dataGridViewAppointments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Estado",
                Name = "Status"
            });
        }

        private async void LoadAppointmentsAsync()
        {
            try
            {
                // Obtener todas las citas desde Firestore
                var query = firebaseService.FirestoreDb.Collection("appointments");
                var snapshot = await query.GetSnapshotAsync();

                // Filtrar solo las citas asignadas al doctor actual
                var filteredAppointments = snapshot.Documents
                    .Where(doc => doc.GetValue<string>("DoctorId") == userId)
                    .ToList();

                if (filteredAppointments.Any())
                {
                    // Convertir las citas para mostrar nombres y el ID
                    var displayAppointments = new List<AppointmentDisplay>();

                    foreach (var doc in filteredAppointments)
                    {
                        var appointment = doc.ConvertTo<Appointment>();
                        var appointmentId = doc.Id; // Obtener el ID del documento

                        // Obtener el nombre del usuario
                        var userDoc = await firebaseService.FirestoreDb.Collection("users").Document(appointment.UserId).GetSnapshotAsync();
                        string userName = userDoc.Exists ? userDoc.GetValue<string>("Name") : "Usuario Desconocido";

                        // Obtener el nombre del doctor
                        var doctorDoc = await firebaseService.FirestoreDb.Collection("doctors").Document(appointment.DoctorId).GetSnapshotAsync();
                        string doctorName = doctorDoc.Exists ? doctorDoc.GetValue<string>("Name") : "Doctor Desconocido";

                        displayAppointments.Add(new AppointmentDisplay
                        {
                            AppointmentId = appointmentId,
                            UserName = userName,
                            DoctorName = doctorName,
                            Date = appointment.Date,
                            Time = appointment.Time,
                            Status = appointment.Status
                        });
                    }

                    // Asignar la lista formateada al DataGridView
                    dataGridViewAppointments.DataSource = displayAppointments;
                }
                else
                {
                    // Limpiar el DataGridView si no hay citas
                    dataGridViewAppointments.DataSource = null;
                    MessageBox.Show("No hay más citas disponibles.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            if (dataGridViewAppointments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una cita para actualizar su estado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dataGridViewAppointments.SelectedRows[0];
            var appointmentId = selectedRow.Cells["AppointmentId"].Value?.ToString(); // Obtener el ID de la cita
            var newStatus = cmbStatus.SelectedItem?.ToString(); // Obtener el estado seleccionado

            if (string.IsNullOrEmpty(appointmentId))
            {
                MessageBox.Show("No se pudo obtener el ID de la cita seleccionada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(newStatus))
            {
                MessageBox.Show("Selecciona un nuevo estado para la cita.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Verificar si el estado es "Completado" para eliminar la cita automáticamente
                if (newStatus == "Completado")
                {
                    var appointmentDoc = firebaseService.FirestoreDb.Collection("appointments").Document(appointmentId);
                    await appointmentDoc.DeleteAsync();

                    MessageBox.Show("La cita ha sido completada y eliminada automáticamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Actualizar el estado de la cita
                    var appointmentDoc = firebaseService.FirestoreDb.Collection("appointments").Document(appointmentId);
                    await appointmentDoc.UpdateAsync("Status", newStatus);

                    MessageBox.Show("Estado de la cita actualizado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Recargar las citas para reflejar el cambio
                LoadAppointmentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el estado de la cita: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.Show();
            this.Close(); // Cierra el formulario actual
        }

        private async void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = dtpFilter.Value.Date;

            try
            {
                // Obtener todas las citas desde Firestore
                var query = firebaseService.FirestoreDb.Collection("appointments");
                var snapshot = await query.GetSnapshotAsync();

                // Filtrar solo las citas asignadas al doctor actual y que coincidan con la fecha seleccionada
                var filteredAppointments = snapshot.Documents
                    .Where(doc => doc.GetValue<string>("DoctorId") == userId && doc.GetValue<string>("Date") == selectedDate.ToString("yyyy-MM-dd"))
                    .ToList();

                if (filteredAppointments.Any())
                {
                    // Convertir las citas para mostrar nombres y el ID
                    var displayAppointments = new List<AppointmentDisplay>();

                    foreach (var doc in filteredAppointments)
                    {
                        var appointment = doc.ConvertTo<Appointment>();
                        var appointmentId = doc.Id; // Obtener el ID del documento

                        // Obtener el nombre del usuario
                        var userDoc = await firebaseService.FirestoreDb.Collection("users").Document(appointment.UserId).GetSnapshotAsync();
                        string userName = userDoc.Exists ? userDoc.GetValue<string>("Name") : "Usuario Desconocido";

                        // Obtener el nombre del doctor
                        var doctorDoc = await firebaseService.FirestoreDb.Collection("doctors").Document(appointment.DoctorId).GetSnapshotAsync();
                        string doctorName = doctorDoc.Exists ? doctorDoc.GetValue<string>("Name") : "Doctor Desconocido";

                        // Agregar la cita formateada a la lista
                        displayAppointments.Add(new AppointmentDisplay
                        {
                            AppointmentId = appointmentId,
                            UserName = userName,
                            DoctorName = doctorName,
                            Date = appointment.Date,
                            Time = appointment.Time,
                            Status = appointment.Status
                        });
                    }

                    // Asignar la lista formateada al DataGridView
                    dataGridViewAppointments.DataSource = displayAppointments;
                }
                else
                {
                    MessageBox.Show("No hay citas para la fecha seleccionada.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Limpiar el DataGridView si no hay citas
                    dataGridViewAppointments.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al filtrar citas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogout_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Crear una nueva instancia del formulario de inicio de sesión
                var loginForm = new LoginForm();

                // Mostrar el formulario de inicio de sesión
                loginForm.Show();

                // Cerrar el formulario actual (DoctorForm)
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cerrar sesión: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}