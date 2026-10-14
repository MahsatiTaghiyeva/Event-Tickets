namespace EventManagement.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public decimal TicketPrice { get; set; }

    public List<Ticket> Tickets { get; set; } = new();

    public override string ToString()
    {
        return $"ID: {Id} | Name: {Name} | Date: {EventDate:dd.MM.yyyy HH:mm} | Price: {TicketPrice}";
    }
}