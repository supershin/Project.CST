using Project.ConstructionTracking.Web.Models;
using System.Net.Mail;
using System.Net;

namespace Project.ConstructionTracking.Web.Infras.Services
{
    public class MailService
    {
        public void SendMail(EmailModel email)
        {
            try
            {
                if (email.To.Count > 0)
                {
                    //email.Body = System.Net.WebUtility.HtmlDecode("&#35;");

                    var msg = new MailMessage(
                          email.From,
                          string.Join(",", email.To.ToArray()),
                          email.Subject,
                         email.Body
                          );

                    msg.IsBodyHtml = true;

                    if (email.Bcc.Count > 0)
                    {
                        msg.Bcc.Add(string.Join(",", email.Bcc.ToArray()));
                    }

                    if (email.CC.Count > 0)
                    {
                        msg.CC.Add(string.Join(",", email.CC.ToArray()));
                    }

                    var client = new SmtpClient(email.Host, email.PORT)
                    {
                        Credentials = new NetworkCredential(email.Username, email.Password),
                        EnableSsl = true
                    };

                    //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //client.UseDefaultCredentials = false;
                    //client.Timeout = 120000;

                    //Add this line to bypass the certificate validation
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                            System.Security.Cryptography.X509Certificates.X509Chain chain,
                            System.Net.Security.SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };
                    client.Send(msg);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
