using System;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace CareToCareServer.Services
{
    public class Email
    {
        static string User = "caretocare@outlook.pt";
        static string Password = "Cen2Peia_4";

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /**
         * Email no registo
         */ 
        public static void GreetingEmail(string toEmail)
        {
            SendEmail(toEmail, "Bem vindo � CareToCare", "Bem vindo � CareToCare, desejamos-lhe uma boa experi�ncia. Instale a App CareToCare no seu Android, para prosseguir com o uso servi�o. \n\nA equipa CareToCare", true);
        }

        public static string SecretNumberEmail(string toEmail)
        {
            string randomCode = RandomString(5);
            SendEmail(
                toEmail, 
                "Recupera��o da palavra-chave", 
                "Introduza o seguinte c�digo na aplica��o para proceder � atualiza��o da sua password.\n\n"+randomCode+"\n\nA equipa CareToCare", 
                false);
            return randomCode;
        }

        /**
         * Notifa��o quando um caregiver tem uma nova marcacao
         */
        public static void CaregiverNotification(string toEmail)
        {
            SendEmail(toEmail, "Nova Notifica��o", "Recebeu um novo pedido de marca��o. Abra a aplica��o CareToCare para aceitar ou recusar a nova marca��o.\n\nA equipa CareToCare", false);
        }

        /**
         * Email no registo
         */
        public static void ContactsEmail(DTO.MessageDTO message)
        {
            String msg = 
                "Nome : " + message.Name + 
                "\nContacto : "+message.Email+
                "\nAssunto : "+message.Type+
                "\nMensagem : "+message.Message;
            SendEmail(User, "Contacto Via Homepage", msg, true);
        }

        public static void SendEmail(string toEmail, string subject, string body, bool withAttachment)
        {
            try
            {
                // CLIENT
                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(User, Password);
                client.Host = "smtp.live.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                // MESSAGE
                MailMessage mail = new MailMessage(User, toEmail);
                mail.Subject = subject;
                mail.Body = body;

                if (withAttachment)
                {
                    /*
                    // ATTACHMENT
                    System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType();
                    contentType.MediaType = System.Net.Mime.MediaTypeNames.Application.Octet;
                    contentType.Name = "test.docx";
                    mail.Attachments.Add(new Attachment("I:/files/test.docx", contentType));
                    */
                }

                // SEND
                client.Send(mail);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
        }
    }
}
