using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace AgendaMedicaApp.Models
{
    

    [FirestoreData]
    public class Appointment
    {
        
        [FirestoreProperty]
        public string UserId { get; set; }  // ID del usuario

        [FirestoreProperty]
        public string DoctorId { get; set; }  // ID del doctor

        [FirestoreProperty]
        public string Date { get; set; }  // Fecha en formato "yyyy-MM-dd"

        [FirestoreProperty]
        public string Time { get; set; }  // Hora en formato "HH:mm"

        [FirestoreProperty]
        public string Status { get; set; }  // Hora en formato "HH:mm"

        public Appointment() { }  // Constructor vacío necesario
    }
    public class AppointmentDisplay
    {
        public string AppointmentId { get; set; } // Campo para el ID de la cita
        public string UserName { get; set; }
        public string DoctorName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Status { get; set; }
        public string Specialty { get; set; } // Campo para la especialidad del doctor
    }
}