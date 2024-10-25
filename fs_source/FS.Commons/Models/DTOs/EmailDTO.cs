using MimeKit;

namespace FS.Commons.Models.DTOs;

public class EmailDTO
{
    public List<MailboxAddress> To { get; set; }
    public string Subject { get; set; }
    public string? BodyPlainText { get; set; }
    public string? BodyHtml { get; set; }
    public EmailDTO(IEnumerable<string> to, string subject, string body)
    {
        To = new List<MailboxAddress>();
        To.AddRange(to.Select(x => new MailboxAddress("email", x)));
        Subject = subject;
        BodyPlainText = body;
    }

}