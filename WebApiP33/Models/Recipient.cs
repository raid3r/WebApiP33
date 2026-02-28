namespace WebApiP33.Models;

public class Recipient
{
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual User User { get; set; }
}
