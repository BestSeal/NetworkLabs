using MailKit.Net.Pop3;

namespace SmtpClient.MailHandler;

public static class LocalStorage
{
    public static string Email { get; set; }
    public static string Password { get; set; }

    public static string SmptAdress { get; set; } = "smtp.gmail.com";
    
    public static string PopAdress { get; set; } = "pop.gmail.com";

    public static int SmptPortNum { get; set; } = 587;
    
    public static int PopPortNum { get; set; } = 995;

    public static System.Net.Mail.SmtpClient SmtpClient { get; set; } = new System.Net.Mail.SmtpClient(SmptAdress, SmptPortNum);

    public static Pop3Client Pop3Client { get; set; } = new Pop3Client();
}