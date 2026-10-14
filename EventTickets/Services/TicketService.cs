using EventManagement.Data;
using EventManagement.Exceptions;
using EventManagement.Interfaces;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _context;

    public TicketService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket> BuyTicketAsync(int eventId)
    {
        var currentEvent = await _context.Events
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (currentEvent == null)
            throw new KeyNotFoundException("Event not found.");

        if (currentEvent.EventDate <= DateTime.Now)
            throw new EventExpiredException(
                "Cannot buy a ticket for an event that has already started.");

        var ticket = new Ticket
        {
            EventId = currentEvent.Id,
            Price = currentEvent.TicketPrice,
            IsUsed = false
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return ticket;
    }

    public async Task UseTicketAsync(int ticketId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);

        if (ticket == null)
            throw new KeyNotFoundException("Ticket not found.");

        if (ticket.IsUsed)
            throw new TicketAlreadyUsedException(
                "This ticket has already been used.");

        ticket.IsUsed = true;
        await _context.SaveChangesAsync();
    }

    public async Task<List<Ticket>> GetAllTicketsAsync()
    {
        return await _context.Tickets
            .OrderBy(t => t.Id)
            .ToListAsync();
    }

    public async Task BuyTicketFromInputAsync()
    {
        var events = await _context.Events
            .Where(e => e.EventDate > DateTime.Now)
            .OrderBy(e => e.EventDate)
            .ToListAsync();

        if (events.Count == 0)
        {
            Console.WriteLine("No upcoming events are available.");
            return;
        }

        Console.WriteLine("\nUpcoming events:");

        foreach (var e in events)
        {
            Console.WriteLine(
                $"ID: {e.Id} | {e.Name} | {e.EventDate:g} | Price: {e.TicketPrice}");
        }

        while (true)
        {
            Console.Write("Enter event ID (0 to cancel): ");

            if (!int.TryParse(Console.ReadLine(), out int eventId))
            {
                Console.WriteLine("Invalid ID.");
                continue;
            }

            if (eventId == 0)
                return;

            if (!events.Any(e => e.Id == eventId))
            {
                Console.WriteLine("Event ID not found. Try again.");
                continue;
            }

            var ticket = await BuyTicketAsync(eventId);
            Console.WriteLine($"Ticket purchased: {ticket}");
            return;
        }
    }
}