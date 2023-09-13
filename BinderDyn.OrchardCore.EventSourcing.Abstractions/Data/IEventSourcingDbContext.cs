using BinderDyn.OrchardCore.EventSourcing.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace BinderDyn.OrchardCore.EventSourcing.Abstractions.Data;

public interface IEventSourcingDbContext
{
    public DbSet<Event> Events { get; set; }
    public Task SaveChangesAsync();
}