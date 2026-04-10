using DnDWorldMapEditor.Settings;
using MailKit.Net.Smtp;
using MimeKit;

namespace DnDWorldMapEditor.Services;

public class SendEmail
{
    public void Send(string userName,string body, string host, int portNumber, string serverUserName, string password)
    {
        var email = new MimeMessage();  
        email.From.Add(MailboxAddress.Parse("DnDWorldMapEditor"));  
        email.To.Add(MailboxAddress.Parse(userName));  
        email.Subject = "Confirm your account";  
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) {Text = body };  
  
        using var smtp = new SmtpClient();  
        
        smtp.Connect(host, portNumber, MailKit.Security.SecureSocketOptions.StartTls);  
        smtp.Authenticate(serverUserName, password);  
        smtp.Send(email);  
        smtp.Disconnect(true);  
    }
}