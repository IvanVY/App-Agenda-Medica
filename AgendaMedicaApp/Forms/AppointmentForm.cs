using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgendaMedicaApp.Models;
using AgendaMedicaApp.Services;
using AgendaMedicaApp.Utils;

namespace AgendaMedicaApp.Forms
{
    public partial class AppointmentForm : Form
    {
        private FirebaseService firebaseService;
        private string userId;
        private string userType;
        private List<Doctor> doctors;

        public AppointmentForm(string userId, string userType, FirebaseService firebaseService)
        {
            InitializeComponent();
            this.userId = userId;
            this.userType = userType;
            this.firebaseService = firebaseService;

            // Inicializar componentes dinámicos
            InitializeDynamicControls();
        }

        private async void InitializeDynamicControls()
        {
            try
            {
                doctors = await LoadDoctorsAsync();

                if (doctors != null && doctors.Any())
                {
                    cmbDoctor.DataSource = doctors;
                    cmbDoctor.DisplayMember = "Name";
                    cmbDoctor.ValueMember = "Id";

                    // Seleccionar el primer doctor por defecto
                    cmbDoctor.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("No hay doctores disponibles. Agrega médicos en Firestore.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnSchedule.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar doctores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSchedule.Enabled = false;
            }
            finally
            {
                // Forzar validación después de cargar doctores
                ValidateFields();
            }
        }

        private async Task<List<Doctor>> LoadDoctorsAsync()
        {
            var query = firebaseService.FirestoreDb.Collection("doctors");
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(doc => doc.ConvertTo<Doctor>())
                .ToList();
        }



        private async void btnSchedule_Click(object sender, EventArgs e)
        {
            try
            {
                string doctorId = (cmbDoctor.SelectedItem as Doctor)?.Id;
                string date = dtpDate.Value.ToString("yyyy-MM-dd");
                string time = cmbTime.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(doctorId) || string.IsNullOrEmpty(time))
                {
                    MessageBox.Show("Por favor, completa todos los campos correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar la hora de la cita
                if (!IsValidAppointmentTime(date, time))
                {
                    return;
                }

                // Agregar la cita
                await firebaseService.AddAppointmentAsync(userId, doctorId, date, time, (cmbDoctor.SelectedItem as Doctor)?.Name, txtSpecialty.Text);
                MessageBox.Show("Cita agendada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Redirigir al formulario principal
                var homeForm = new HomeForm(userId, userType, firebaseService);
                homeForm.Show();
                this.Close(); // Cierra el formulario actual
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AppointmentForm_Load(object sender, EventArgs e)
        {
            // Configurar el ComboBox de horas
            cmbTime.Items.Clear();
            cmbTime.Items.AddRange(new string[] {
                "09:00", "10:00", "11:00", "12:00",
                "14:00", "15:00", "16:00", "17:00"
            });
            cmbTime.SelectedIndex = 0; // Seleccionar la primera hora por defecto

            // Configurar el DateTimePicker para fechas futuras
            dtpDate.MinDate = DateTime.Today;

            // Forzar validación al cargar el formulario
            ValidateFields();
        }
        private void ValidateFields()
        {
            // Validar que se haya seleccionado un doctor
            bool doctorSelected = cmbDoctor.SelectedItem != null;
            Console.WriteLine($"Doctor seleccionado: {doctorSelected}");

            // Validar fecha (debe ser hoy o posterior)
            bool isValidDate = dtpDate.Value.Date >= DateTime.Today;
            Console.WriteLine($"Fecha válida: {isValidDate}");

            // Validar hora (debe estar seleccionada y ser válida)
            string selectedTime = cmbTime.SelectedItem?.ToString()?.Trim(); // Limpiar espacios adicionales
            Console.WriteLine($"Hora seleccionada: '{selectedTime}'"); // Imprimir el valor exacto
            bool isValidTime = !string.IsNullOrEmpty(selectedTime) && ValidationUtils.IsValidTime(selectedTime);
            Console.WriteLine($"Hora válida: {isValidTime}");

            // Habilitar el botón solo si todos los campos son válidos
            btnSchedule.Enabled = doctorSelected && isValidDate && isValidTime;
            Console.WriteLine($"Botón habilitado: {btnSchedule.Enabled}");
        }

        private bool IsValidAppointmentTime(string date, string time)
        {
            try
            {
                DateTime appointmentDateTime = DateTime.Parse($"{date} {time}");
                DateTime currentDateTime = DateTime.Now.AddHours(1);

                if (appointmentDateTime <= currentDateTime)
                {
                    MessageBox.Show("La cita debe ser programada al menos una hora después de la hora actual.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                return true;
            }
            catch
            {
                MessageBox.Show("La fecha u hora seleccionada no es válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private void cmbDoctor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Obtener el doctor seleccionado
                var selectedDoctor = cmbDoctor.SelectedItem as Doctor;

                if (selectedDoctor != null)
                {
                    // Mostrar la especialidad del doctor en el campo correspondiente
                    txtSpecialty.Text = selectedDoctor.Specialty;
                }
                else
                {
                    txtSpecialty.Clear(); // Limpiar el campo si no hay un doctor seleccionado
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener la especialidad del doctor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ValidateFields();
            }
        }

        private void cmbTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateFields();
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            ValidateFields();
        }
        private void txtSpecialty_TextChanged(object sender, EventArgs e)
        {
            ValidateFields();
        }
    }
}