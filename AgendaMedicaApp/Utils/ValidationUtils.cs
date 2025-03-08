using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AgendaMedicaApp.Utils
{
    public static class ValidationUtils
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            // Requisitos: al menos 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, pattern);
        }

        public static bool IsValidDate(string date)
        {   
            if (string.IsNullOrEmpty(date))
                return false;

            return DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _);
        }

        public static bool IsValidTime(string time)
        {
            if (string.IsNullOrEmpty(time))
                return false;

            Console.WriteLine($"Validando hora: '{time}'"); // Imprimir el valor exacto

            // Limpiar espacios adicionales y caracteres invisibles
            time = time.Trim();
            time = new string(time.Where(c => !char.IsControl(c)).ToArray());

            // Dividir la hora en partes (horas y minutos)
            var parts = time.Split(':');
            if (parts.Length != 2) // Debe haber exactamente dos partes (horas y minutos)
                return false;

            if (!int.TryParse(parts[0], out int hours) || !int.TryParse(parts[1], out int minutes))
                return false; // Ambas partes deben ser números enteros

            // Verificar que las horas estén entre 0 y 23 y los minutos entre 0 y 59
            if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59)
                return false;

            Console.WriteLine($"Resultado de validación: True");
            return true;
        }

    }
}