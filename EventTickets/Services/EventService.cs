using EventManagement.Data;
using EventManagement.Interfaces;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EventManagement.Services;

public class EventService : IEventService
{
    private readonly AppDbContext _context;

    public EventService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Event> AddAsync(Event newEvent)
    {
        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();
        return newEvent;
    }

    public async Task<List<Event>> GetAllAsync()
    {
        return await _context.Events
            .Include(e => e.Tickets)
            .OrderBy(e => e.Id)
            .ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(int id)
    {
        return await _context.Events
            .Include(e => e.Tickets)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task UpdateAsync(Event updatedEvent)
    {
        var existing = await _context.Events.FindAsync(updatedEvent.Id);

        if (existing == null)
            throw new KeyNotFoundException("Event not found.");

        existing.Name = updatedEvent.Name;
        existing.EventDate = updatedEvent.EventDate;
        existing.TicketPrice = updatedEvent.TicketPrice;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _context.Events.FindAsync(id);

        if (existing == null)
            throw new KeyNotFoundException("Event not found.");

        _context.Events.Remove(existing);
        await _context.SaveChangesAsync();
    }

    public async Task<List<(Event Event, int TicketsSold)>>
        GetEventsBySalesAsync()
    {
        var events = await _context.Events
            .Include(e => e.Tickets)
            .ToListAsync();

        return events
            .Select(e => (Event: e, TicketsSold: e.Tickets.Count))
            .OrderByDescending(x => x.TicketsSold)
            .ToList();
    }

    public async Task<List<(Event Event, decimal Revenue)>>
        GetEventsByRevenueAsync()
    {
        var events = await _context.Events
            .Include(e => e.Tickets)
            .ToListAsync();

        return events
            .Select(e => (
                Event: e,
                Revenue: e.Tickets.Sum(t => t.Price)))
            .OrderByDescending(x => x.Revenue)
            .ToList();
    }

    public async Task<List<Event>> GetFutureEventsAsync()
    {
        return await _context.Events
            .Where(e => e.EventDate > DateTime.Now)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<List<(Event Event, int Purchased, int Used)>>
        GetPastEventTicketStatsAsync()
    {
        var events = await _context.Events
            .Include(e => e.Tickets)
            .Where(e => e.EventDate < DateTime.Now)
            .ToListAsync();

        return events.Select(e => (
            Event: e,
            Purchased: e.Tickets.Count,
            Used: e.Tickets.Count(t => t.IsUsed)
        )).ToList();
    }

    public async Task<Event> CreateFromInputAsync()
    {
        string name;

        do
        {
            Console.Write("Event name: ");
            name = Console.ReadLine()?.Trim() ?? "";
        }
        while (string.IsNullOrWhiteSpace(name));

        DateTime date;

        while (true)
        {
            Console.Write("Date and time (dd.MM.yyyy HH:mm): ");

            if (DateTime.TryParseExact(
                Console.ReadLine(),
                "dd.MM.yyyy HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date) && date > DateTime.Now)
            {
                break;
            }

            Console.WriteLine("Enter a valid future date and time.");
        }

        decimal price;

        while (true)
        {
            Console.Write("Ticket price: ");

            if (decimal.TryParse(
                Console.ReadLine(),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out price) && price >= 0)
            {
                break;
            }

            Console.WriteLine("Enter a valid non-negative price, e.g. 50.00.");
        }

        return await AddAsync(new Event
        {
            Name = name,
            EventDate = date,
            TicketPrice = price
        });
    }
}