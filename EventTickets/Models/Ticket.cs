namespace EventManagement.Models;

public class Ticket
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public bool IsUsed { get; set; } = false;

    public int EventId { get; set; }
    public Event? Event { get; set; }

    public override string ToString()
    {
        return $"Ticket ID: {Id} | Event ID: {EventId} | Price: {Price} | Used: {IsUsed}";
    }
}