using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Services.Description;

namespace ClothesShop.Utils
{
    public class MailUtils
    {
        public static bool sendEmail(string _to, string _subject, string _body)
        {
            try
            {
                var _from = "hue.nhoc@gmail.com";
                var message = new MailMessage(
                 from: _from,
                 to: _to,
                 subject: _subject,
                 body: _body
                );
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                message.ReplyToList.Add(new MailAddress(_from));
                message.Sender = new MailAddress(_from);
                using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential("hue.nhoc@gmail.com", "brfa nqha yfur spyk");
                    client.EnableSsl = true;
                    client.Send(message);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}