using System;
using System.Windows.Forms;
using AgendaMedicaApp.Forms;

namespace AgendaMedicaApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Crear y mostrar el formulario de inicio de sesión
            Application.Run(new LoginForm());
        }
    }
}