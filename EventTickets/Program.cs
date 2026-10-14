using EventManagement.Data;
using EventManagement.Interfaces;
using EventManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection")));

services.AddScoped<IEventService, EventService>();

var serviceProvider = services.BuildServiceProvider();

using var scope = serviceProvider.CreateScope();

var eventService = scope.ServiceProvider
    .GetRequiredService<IEventService>();

try
{
    // CREATE
    Event newEvent = new Event
    {
        Name = "Programming Conference",
        EventDate = new DateTime(2026, 12, 15),
        TicketPrice = 50
    };

    newEvent.Tickets.Add(new Ticket { Price = 50 });
    newEvent.Tickets.Add(new Ticket { Price = 75 });

    await eventService.AddAsync(newEvent);

    Console.WriteLine("Event added successfully!");

    // READ ALL
    Console.WriteLine("\nAll Events:");

    List<Event> events = await eventService.GetAllAsync();

    foreach (Event currentEvent in events)
    {
        Console.WriteLine(currentEvent);

        foreach (Ticket ticket in currentEvent.Tickets)
        {
            Console.WriteLine(ticket);
        }
    }

    // READ BY ID
    Console.WriteLine("\nFind Event:");

    Event? foundEvent = await eventService.GetByIdAsync(newEvent.Id);

    Console.WriteLine(foundEvent);

    // UPDATE
    Console.WriteLine("\nUpdate Event:");

    Event updatedEvent = new Event
    {
        Id = newEvent.Id,
        Name = "Updated Conference",
        EventDate = new DateTime(2026, 12, 20),
        TicketPrice = 60
    };

    await eventService.UpdateAsync(updatedEvent);

    Console.WriteLine("Event updated successfully!");

    // DELETE
    Console.WriteLine("\nDelete Event:");

    await eventService.DeleteAsync(newEvent.Id);

    Console.WriteLine("Event deleted successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    if (serviceProvider is IDisposable disposable)
    {
        disposable.Dispose();
    }
}