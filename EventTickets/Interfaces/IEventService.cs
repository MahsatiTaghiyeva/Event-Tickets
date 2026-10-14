namespace EventManagement.Interfaces;

public interface IEventService
{
    Task<List<Event>> GetAllAsync();

    Task<Event?> GetByIdAsync(int id);

    Task AddAsync(Event newEvent);

    Task UpdateAsync(Event updatedEvent);

    Task DeleteAsync(int id);
}