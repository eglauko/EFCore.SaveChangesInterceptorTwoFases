
using System.Text.Json;

namespace EFCore.SaveChangesInterceptorTwoFases;

public interface IEntity
{
    ICollection<IDomainEvent> Events { get; }
}

public abstract class Entity : IEntity
{
    public ICollection<IDomainEvent> Events { get; } = new HashSet<IDomainEvent>();

    protected void Fire<TEvent>(TEvent @event) where TEvent : IDomainEvent
    {
        Events.Add(@event);
    }
}

public class Blog : Entity
{
    public Blog(string name, string description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));

        Fire(new BlogCreated(this));
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<Post> Posts { get; set; }

    public override string? ToString()
    {
        return $"Blog: {Id}, {Name}, {Description}";
    }
}

public class Post
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public DateTime DateTime { get; set; }

    public Blog Blog { get;}
}

public class DomainEventDetails
{
    public DomainEventDetails(IDomainEvent domainEvent)
    {
        if (domainEvent is null)
            throw new ArgumentNullException(nameof(domainEvent));

        Id = domainEvent.Id;
        Occurred = domainEvent.Occurred;
        TypeFullName = FullNameWithAssembly(domainEvent.GetType());
        EventData = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());
    }

    protected DomainEventDetails() { }

    public Guid Id { get; set; }

    public DateTime Occurred { get; set; }

    public string TypeFullName { get; set; }

    public string EventData { get; set; }

    public override string ToString()
    {
        return $"Domain Event [{Id}] occurred {Occurred} of type {TypeFullName}, with values: {EventData}";
    }

    private static string FullNameWithAssembly(Type type)
    {
        var names = type.AssemblyQualifiedName!.Split(',');
        return names[0] + ',' + names[1];
    }
}