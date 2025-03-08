namespace AgendaMedicaApp.Models
{
    public class User
    {
        public string Id { get; set; } // ID único del usuario (generado por Firebase)
        public string Name { get; set; } // Nombre del usuario
        public string Email { get; set; } // Correo electrónico del usuario
        public string Password { get; set; } // Contraseña del usuario
        public string Type { get; set; } // Tipo de usuario: "Paciente" o "Administrador"
    }
}