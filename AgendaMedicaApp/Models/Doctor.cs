using Google.Cloud.Firestore;

namespace AgendaMedicaApp.Models
{
    [FirestoreData] // Indica que se usará en Firestore
    public class Doctor
    {
        [FirestoreDocumentId] // Para que Firestore use el ID del documento
        public string Id { get; set; }

        [FirestoreProperty] // Mapea el campo "Name"
        public string Name { get; set; }

        [FirestoreProperty] // Mapea el campo "Specialty"
        public string Specialty { get; set; }

        [FirestoreProperty]
        public string Email { get; set; } // Correo electrónico del doctor


        public override string ToString()
        {
            return Name; // Mostrar el nombre del doctor en el ComboBox
        }
        public Doctor() { } // Constructor vacío requerido por Firestore
    }
}