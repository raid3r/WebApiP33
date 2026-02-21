namespace WebApiP33.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public Recipient From { get; set; }
    public Recipient To { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
}
