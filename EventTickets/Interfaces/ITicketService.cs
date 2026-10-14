using EventManagement.Models;

namespace EventManagement.Interfaces;

public interface ITicketService
{
    Task<Ticket> BuyTicketAsync(int eventId);
    Task UseTicketAsync(int ticketId);
    Task<List<Ticket>> GetAllTicketsAsync();
    Task BuyTicketFromInputAsync();
}