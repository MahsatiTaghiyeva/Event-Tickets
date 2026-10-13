public class Event
{
    public int Id {get; set;}
    public string Name {get; set;} = null!;
    public DateTime EventDate {get; set;}
    public decimal TicketPrice {get; set;}
    public List<Ticket> Tickets = new List<Ticket>();
    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Date: {EventDate}, Price: {TicketPrice}";
    }
}