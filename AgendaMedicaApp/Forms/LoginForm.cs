using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgendaMedicaApp.Services;
using FirebaseAdmin.Auth;

namespace AgendaMedicaApp.Forms
{
    public partial class LoginForm : Form
    {
        private FirebaseService firebaseService;

        public LoginForm()
        {
            InitializeComponent();
            firebaseService = new FirebaseService();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Por favor, ingresa tu correo electrónico y contraseña.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Autenticar al usuario
                var result = await firebaseService.AuthenticateUserAsync(email, password);

                if (!result.success)
                {
                    MessageBox.Show("Correo electrónico o contraseña incorrectos. Por favor, inténtalo de nuevo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Redirigir al formulario correspondiente según el rol del usuario
                this.Hide();
                if (result.userType == "admin")
                {
                    var adminForm = new AdminForm(firebaseService);
                    adminForm.Show();
                }
                else if (result.userType == "doctor")
                {
                    var doctorForm = new DoctorForm(result.userId, firebaseService);
                    doctorForm.Show();
                }
                else
                {
                    var homeForm = new HomeForm(result.userId, result.userType, firebaseService);
                    homeForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar sesión: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            var registerForm = new RegisterForm(firebaseService);
            registerForm.Show();
            this.Hide();
        }
    }
}