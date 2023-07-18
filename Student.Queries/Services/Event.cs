using Microsoft.EntityFrameworkCore.Diagnostics;

namespace StudentQueries.Services;

public record Event<T> where T : IEventData
{
    public Guid AggregateId { get; private set;}
    public int Sequence { get; private set;}
    public DateTime DateTime { get; private set;}
    public T Data { get; private set;}
    public int Version { get; private set;}

    public Event(
        Guid AggregateId,
        int Sequence,
        DateTime DateTime,
        T Data,
        int Version
    )
    {
        this.AggregateId = AggregateId;
        this.Sequence = Sequence;
        this.DateTime = DateTime;
        this.Data = Data;
        this.Version = Version;
    }

    protected Event()
    {
        
    }
}

public interface IEventData
{
}