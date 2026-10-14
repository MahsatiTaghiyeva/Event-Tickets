namespace EventManagement.Exceptions;

public class TicketAlreadyUsedException : Exception
{
    public TicketAlreadyUsedException(string message) : base(message)
    {
    }
}