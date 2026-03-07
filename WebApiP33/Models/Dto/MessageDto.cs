namespace WebApiP33.Models.Dto;

public class MessageDto
{
    public int Id { get; set; }
    public RecipientDto From { get; set; }
    public RecipientDto To { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
}
