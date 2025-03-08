using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgendaMedicaApp.Models;
using AgendaMedicaApp.Services;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace AgendaMedicaApp.Forms
{
    public partial class HomeForm : Form
    {
        private string userId;
        private string userType;
        private FirebaseService firebaseService;

        public HomeForm(string userId, string userType, FirebaseService firebaseService)
        {
            InitializeComponent();
            this.userId = userId;
            this.userType = userType;
            this.firebaseService = firebaseService;

            // Configurar los controles
            ConfigureDataGridView();
            LoadAppointmentsAsync();

        }

        private void ConfigureDataGridView()
        {
            dataGridViewAppointments.AutoGenerateColumns = false;

            // Columna oculta para el ID de la cita
            dataGridViewAppointments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AppointmentId", // Asociar con el campo AppointmentId del modelo
                Name = "AppointmentId",
                Visible = false // Ocultar esta columna
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

                // Filtrar solo las citas asignadas al usuario actual
                var filteredAppointments = snapshot.Documents
                    .Where(doc => doc.GetValue<string>("UserId") == userId)
                    .ToList();

                if (filteredAppointments.Any())
                {
                    // Convertir las citas para mostrar nombres y el ID
                    var displayAppointments = new List<AppointmentDisplay>();

                    foreach (var doc in filteredAppointments)
                    {
                        var appointment = doc.ConvertTo<Appointment>();
                        var appointmentId = doc.Id; // Obtener el ID del documento

                        // Obtener el nombre del doctor
                        var doctorDoc = await firebaseService.FirestoreDb.Collection("doctors").Document(appointment.DoctorId).GetSnapshotAsync();
                        string doctorName = doctorDoc.Exists ? doctorDoc.GetValue<string>("Name") : "Doctor Desconocido";

                        displayAppointments.Add(new AppointmentDisplay
                        {
                            AppointmentId = appointmentId,
                            DoctorName = doctorName, // Asignar el nombre del doctor
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
                    MessageBox.Show("No hay citas asignadas.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            string filePath = "citas.pdf";

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                document.Add(new Paragraph("Resumen de Citas"));
                document.Add(new Paragraph("----------------------------"));

                foreach (var appointment in (List<AppointmentDisplay>)dataGridViewAppointments.DataSource)
                {
                    document.Add(new Paragraph($"Usuario: {appointment.UserName}"));
                    document.Add(new Paragraph($"Doctor: {appointment.DoctorName}"));
                    document.Add(new Paragraph($"Especialidad: {appointment.Specialty}")); // Agregar la especialidad
                    document.Add(new Paragraph($"Fecha: {appointment.Date}"));
                    document.Add(new Paragraph($"Hora: {appointment.Time}"));
                    document.Add(new Paragraph($"Estado: {appointment.Status}"));
                    document.Add(new Paragraph("----------------------------"));
                }

                document.Close();
            }

            MessageBox.Show($"PDF exportado exitosamente: {Path.GetFullPath(filePath)}");
        }

        // Método para cerrar sesión
        private void btnLogout_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.Show();
            this.Close(); // Cierra el formulario actual
        }

        // Método para abrir el formulario de agendamiento de citas
        private void btnScheduleAppointment_Click(object sender, EventArgs e)
        {
            var appointmentForm = new AppointmentForm(userId, userType, firebaseService);
            appointmentForm.Show();
            this.Hide(); // Oculta el formulario actual mientras se abre el formulario de citas
        }

        private async void btnDeleteAppointment_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewAppointments.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Selecciona una cita para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRow = dataGridViewAppointments.SelectedRows[0];
                var appointmentId = selectedRow.Cells["AppointmentId"].Value?.ToString(); // Obtener el ID de la cita
                var status = selectedRow.Cells["Status"].Value?.ToString();

                if (string.IsNullOrEmpty(appointmentId))
                {
                    MessageBox.Show("No se pudo obtener el ID de la cita seleccionada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Verificar que el estado de la cita sea "Pendiente"
                if (status != "Pendiente")
                {
                    MessageBox.Show("Solo puedes eliminar citas con estado 'Pendiente'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Confirmar la eliminación
                DialogResult result = MessageBox.Show("¿Estás seguro de que deseas eliminar esta cita?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Eliminar la cita de Firestore
                    var appointmentDoc = firebaseService.FirestoreDb.Collection("appointments").Document(appointmentId);
                    await appointmentDoc.DeleteAsync();

                    MessageBox.Show("Cita eliminada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Recargar las citas para reflejar el cambio
                    LoadAppointmentsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la cita: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}