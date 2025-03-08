using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaMedicaApp.Utils
{
    public static class FirebaseConfig
    {
        // Cambia la ruta para que sea relativa al directorio base del proyecto
        public static string CredentialsPath => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebase-adminsdk.json");

        public static string ProjectId => "agendadecitas-fe25e";
    }
}
