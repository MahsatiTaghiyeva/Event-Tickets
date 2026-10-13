public class Ticket
{
    public int Id {get; set;}
    public decimal Price {get; set;}
    public int EventId {get; set;}
    public Event Event {get; set;} = null!;
    public override string ToString()
    {
        return $"Id: {Id}, Price: {Price}";
    }
}