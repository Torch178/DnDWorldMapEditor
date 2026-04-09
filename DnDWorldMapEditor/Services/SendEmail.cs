using MailKit.Net.Smtp;
using MimeKit;

namespace DnDWorldMapEditor.Services;

public class SendEmail
{
    public void Send(string userName,string body)  
    {  
        var email = new MimeMessage();  
        email.From.Add(MailboxAddress.Parse("DnDWorldMapEditor"));  
        email.To.Add(MailboxAddress.Parse(userName));  
        email.Subject = "Confirm your account";  
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) {Text = body };  
  
        using var smtp = new SmtpClient();  
        
        //ToDo add/setup dependency injection for app password
        smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);  
        smtp.Authenticate("dndworldmapeditor.noreply@gmail.com", "xbwh qcyg ufpo twgu");  
        smtp.Send(email);  
        smtp.Disconnect(true);  
    }
}