using System;
using System.Windows.Forms;
using System.Xml.Linq;
using AgendaMedicaApp.Forms;
using AgendaMedicaApp.Services;
using AgendaMedicaApp.Utils;

namespace AgendaMedicaApp
{
    public partial class RegisterForm : Form
    {
        private FirebaseService firebaseService;


        // Constructor que recibe una instancia de FirebaseService
        public RegisterForm(FirebaseService firebaseService)
        {
            InitializeComponent();
            this.firebaseService = firebaseService;
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            // Validar los campos usando ValidationUtils
            if (string.IsNullOrEmpty(name) || !ValidationUtils.IsValidEmail(email) || !ValidationUtils.IsValidPassword(password))
            {
                MessageBox.Show("Por favor, completa todos los campos correctamente.");
                return;
            }

            try
            {
                await firebaseService.RegisterUserAsync(email, password, name, "Paciente");
                MessageBox.Show("Registro exitoso. Ahora puedes iniciar sesión.");
                this.Close();
                var loginForm = new LoginForm(); // Abre el formulario anterior (por ejemplo, el de inicio de sesión)
                loginForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar: {ex.Message}");
            }
        }
    }
}