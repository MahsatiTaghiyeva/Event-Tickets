namespace EventManagement.Exceptions;

public class EventExpiredException : Exception
{
    public EventExpiredException(string message) : base(message)
    {
    }
}