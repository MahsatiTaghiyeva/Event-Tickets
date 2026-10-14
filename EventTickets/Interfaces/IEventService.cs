using EventManagement.Models;

namespace EventManagement.Interfaces;

public interface IEventService
{
    Task<Event> AddAsync(Event newEvent);
    Task<List<Event>> GetAllAsync();
    Task<Event?> GetByIdAsync(int id);
    Task UpdateAsync(Event updatedEvent);
    Task DeleteAsync(int id);

    Task<List<(Event Event, int TicketsSold)>> GetEventsBySalesAsync();
    Task<List<(Event Event, decimal Revenue)>> GetEventsByRevenueAsync();
    Task<List<Event>> GetFutureEventsAsync();

    Task<List<(Event Event, int Purchased, int Used)>>
        GetPastEventTicketStatsAsync();

    Task<Event> CreateFromInputAsync();
}