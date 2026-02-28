using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiP33.Models;

public class ChatMessage
{
    public int Id { get; set; }
    
    [ForeignKey("From")]
    public int FromId { get; set; }
    public virtual Recipient From { get; set; }
    
    [ForeignKey("To")]
    public int ToId { get; set; }
    public virtual Recipient To { get; set; }

    [MaxLength(1024)]
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
}
