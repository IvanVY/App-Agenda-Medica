using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using AgendaMedicaApp.Services;

namespace AgendaMedicaApp.Forms
{
    public partial class AdminForm : Form
    {
        private FirebaseService firebaseService;

        public AdminForm(FirebaseService firebaseService)
        {
            InitializeComponent();
            this.firebaseService = firebaseService;

            // Configurar el ComboBox para seleccionar el tipo de usuario
            cmbUserType.Items.AddRange(new string[] { "Paciente", "Doctor" });
            cmbUserType.SelectedIndex = 0; // Seleccionar "Paciente" por defecto
                                           // Configurar el ComboBox para seleccionar la especialidad
            cmbSpecialty.Items.AddRange(new string[]
            {
                "Cardiología",
                "Dermatología",
                "Pediatría",
                "Neurología",
                "Ginecología",
                "Ortopedia"
            });
            // Inicialmente ocultar el ComboBox de especialidades
            lblSpecialty.Visible = false;
            cmbSpecialty.Visible = false;
        }

        private async void btnRegisterUser_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text.Trim();
                string userType = cmbUserType.SelectedItem?.ToString().ToLower();

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Por favor, completa todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (userType == "doctor")
                {
                    // Obtener la especialidad seleccionada
                    string specialty = cmbSpecialty.SelectedItem?.ToString();

                    if (string.IsNullOrEmpty(specialty))
                    {
                        MessageBox.Show("Por favor, selecciona una especialidad válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Registrar un doctor
                    await firebaseService.RegisterDoctorAsync(email, password, name, specialty);
                    MessageBox.Show("Doctor registrado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                else if (userType == "paciente")
                {
                    // Registrar un paciente
                    await firebaseService.RegisterUserAsync(email, password, name, "patient");
                    MessageBox.Show("Paciente registrado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Tipo de usuario no válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Limpiar los campos después del registro
                txtName.Clear();
                txtEmail.Clear();
                txtPassword.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbUserType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Verificar si el tipo de usuario seleccionado es "Doctor"
            string userType = cmbUserType.SelectedItem?.ToString().ToLower();
            if (userType == "doctor")
            {
                lblSpecialty.Visible = true; // Mostrar el label de especialidad
                cmbSpecialty.Visible = true; // Mostrar el ComboBox de especialidades
            }
            else
            {
                lblSpecialty.Visible = false; // Ocultar el label de especialidad
                cmbSpecialty.Visible = false; // Ocultar el ComboBox de especialidades
            }
        }
    }
}