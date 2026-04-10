using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using System.Net;
using System.Net.Mail;

namespace stage_2_final_project_tgbooks_backend.Data.Implementations
{
    public class EmailSender : IEmailSender
    {
        public void Send(string to, string subject, string body)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            string? smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

            if (string.IsNullOrEmpty(smtpPassword))
            {
                throw new Exception("SMTP_PASSWORD environment variable is not set.");
            }

            smtpClient.Credentials = new NetworkCredential("gachechiladzetengiz8@gmail.com", smtpPassword);
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("gachechiladzetengiz8@gmail.com");
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;

            smtpClient.Send(mail);
        }
    }
}
