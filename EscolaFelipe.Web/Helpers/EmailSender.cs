using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace EscolaFelipe.Web.Helpers
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Simulated email sending — just writes to the console
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine($"[EMAIL SIMULADO] Para: {email}");
            Console.WriteLine($"Assunto: {subject}");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine(htmlMessage);
            Console.WriteLine("------------------------------------------------");
            return Task.CompletedTask;
        }
    }
}
