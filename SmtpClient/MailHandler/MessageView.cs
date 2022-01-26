using System.Linq;
using MimeKit;

namespace SmtpClient.MailHandler;

public class MessageView
{
    public string Title { get; set; } = "";

    public string FromAddr { get; set; } = "";

    public string Content { get; set; } = "";

    public string HtmlBody { get; set; } = "";

    public int Index { get; set; }

    public MessageView(MimeMessage message, int index = 0)
    {
        Title = message?.Subject ?? "";
        FromAddr = message?.From.FirstOrDefault()?.Name ?? "";
        Content = message?.TextBody ?? "";
        HtmlBody = message?.HtmlBody ?? "";
        Index = index;
    }

    public MessageView()
    {
    }
}