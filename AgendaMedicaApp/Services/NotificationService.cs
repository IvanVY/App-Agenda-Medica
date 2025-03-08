using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AgendaMedicaApp.Services
{
    public class NotificationService
    {
        private static string serverKey = "TU_SERVER_KEY_DE_FIREBASE"; // Reemplaza con tu clave de servidor

        public async Task SendNotificationAsync(string title, string body, string token)
        {
            var message = new
            {
                to = token,
                notification = new
                {
                    title = title,
                    body = body
                }
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={serverKey}");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://fcm.googleapis.com/fcm/send", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Error al enviar la notificación.");
                }
            }
        }
    }
}