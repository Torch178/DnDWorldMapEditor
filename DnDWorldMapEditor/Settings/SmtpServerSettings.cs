namespace DnDWorldMapEditor.Settings;

public class SmtpServerSettings
{
    public required string Host { get; set; }
    public required int PortNumber { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
}