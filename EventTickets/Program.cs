using EventManagement.Data;
using EventManagement.Exceptions;
using EventManagement.Interfaces;
using EventManagement.Models;
using EventManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection")));

services.AddScoped<IEventService, EventService>();
services.AddScoped<ITicketService, TicketService>();

using var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();

var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();

while (true)
{
    Console.WriteLine("\n===== EVENT TICKETS =====");
    Console.WriteLine("1. Create event");
    Console.WriteLine("2. Show all events");
    Console.WriteLine("3. Find event");
    Console.WriteLine("4. Update event");
    Console.WriteLine("5. Delete event");
    Console.WriteLine("6. Buy ticket");
    Console.WriteLine("7. Use ticket");
    Console.WriteLine("8. Show all tickets");
    Console.WriteLine("9. Rank events by tickets sold");
    Console.WriteLine("10. Rank events by revenue");
    Console.WriteLine("11. Show future events");
    Console.WriteLine("12. Show past event ticket statistics");
    Console.WriteLine("0. Exit");
    Console.Write("Choose: ");

    if (!int.TryParse(Console.ReadLine(), out int choice))
    {
        Console.WriteLine("Invalid choice.");
        continue;
    }

    if (choice == 0)
        break;

    try
    {
        switch (choice)
        {
            case 1:
                Console.WriteLine(
                    await eventService.CreateFromInputAsync());
                break;

            case 2:
                foreach (var e in await eventService.GetAllAsync())
                {
                    Console.WriteLine(e);
                    foreach (var t in e.Tickets)
                        Console.WriteLine($"  {t}");
                }
                break;

            case 3:
                Console.Write("Event ID: ");
                if (int.TryParse(Console.ReadLine(), out int findId))
                {
                    var found = await eventService.GetByIdAsync(findId);
                    Console.WriteLine(found ?? null!);
                    if (found == null)
                        Console.WriteLine("Event not found.");
                }
                break;

            case 4:
                Console.Write("Event ID: ");
                if (!int.TryParse(Console.ReadLine(), out int updateId))
                    break;

                var existing = await eventService.GetByIdAsync(updateId);

                if (existing == null)
                {
                    Console.WriteLine("Event not found.");
                    break;
                }

                Console.Write($"New name ({existing.Name}): ");
                string? name = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(name))
                    existing.Name = name.Trim();

                Console.Write("New date (dd.MM.yyyy HH:mm): ");
                if (!DateTime.TryParseExact(
                    Console.ReadLine(),
                    "dd.MM.yyyy HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var date))
                {
                    Console.WriteLine("Invalid date.");
                    break;
                }

                Console.Write("New ticket price: ");
                if (!decimal.TryParse(
                    Console.ReadLine(),
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var price) || price < 0)
                {
                    Console.WriteLine("Invalid price.");
                    break;
                }

                existing.EventDate = date;
                existing.TicketPrice = price;

                await eventService.UpdateAsync(existing);
                Console.WriteLine("Event updated.");
                break;

            case 5:
                Console.Write("Event ID: ");
                if (int.TryParse(Console.ReadLine(), out int deleteId))
                {
                    await eventService.DeleteAsync(deleteId);
                    Console.WriteLine("Event deleted.");
                }
                break;

            case 6:
                await ticketService.BuyTicketFromInputAsync();
                break;

            case 7:
                Console.Write("Ticket ID: ");
                if (int.TryParse(Console.ReadLine(), out int ticketId))
                {
                    await ticketService.UseTicketAsync(ticketId);
                    Console.WriteLine("Ticket used successfully.");
                }
                break;

            case 8:
                foreach (var t in await ticketService.GetAllTicketsAsync())
                    Console.WriteLine(t);
                break;

            case 9:
                foreach (var x in await eventService.GetEventsBySalesAsync())
                    Console.WriteLine($"{x.Event} | Tickets sold: {x.TicketsSold}");
                break;

            case 10:
                foreach (var x in await eventService.GetEventsByRevenueAsync())
                    Console.WriteLine($"{x.Event} | Revenue: {x.Revenue}");
                break;

            case 11:
                foreach (var e in await eventService.GetFutureEventsAsync())
                    Console.WriteLine(e);
                break;

            case 12:
                foreach (var x in await eventService.GetPastEventTicketStatsAsync())
                    Console.WriteLine(
                        $"{x.Event.Name} - {x.Purchased}/{x.Used} purchased/used");
                break;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
    catch (EventExpiredException ex)
    {
        Console.WriteLine($"Event expired: {ex.Message}");
    }
    catch (TicketAlreadyUsedException ex)
    {
        Console.WriteLine($"Ticket already used: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}