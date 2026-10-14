using EventManagement.Data;
using EventManagement.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Services;

public class EventService : IEventService
{
    private readonly AppDbContext _context;

    public EventService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Event>> GetAllAsync()
    {
        return await _context.Events
            .Include(e => e.Tickets)
            .ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(int id)
    {
        return await _context.Events
            .Include(e => e.Tickets)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task AddAsync(Event newEvent)
    {
        await _context.Events.AddAsync(newEvent);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Event updatedEvent)
    {
        Event? existingEvent = await _context.Events
            .FirstOrDefaultAsync(e => e.Id == updatedEvent.Id);

        if (existingEvent == null)
        {
            throw new Exception($"Event with ID {updatedEvent.Id} was not found.");
        }

        existingEvent.Name = updatedEvent.Name;
        existingEvent.EventDate = updatedEvent.EventDate;
        existingEvent.TicketPrice = updatedEvent.TicketPrice;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Event? existingEvent = await _context.Events
            .FirstOrDefaultAsync(e => e.Id == id);

        if (existingEvent == null)
        {
            throw new Exception($"Event with ID {id} was not found.");
        }

        _context.Events.Remove(existingEvent);

        await _context.SaveChangesAsync();
    }
}