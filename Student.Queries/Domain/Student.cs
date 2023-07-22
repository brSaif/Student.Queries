using StudentQueries.CreateStudent;
using StudentQueries.Services;
using StudentQueries.UpdateStudent;

namespace StudentQueries.Domain;

public class Student
{
    public Guid Id { get; private set; }
    public int Sequence { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastUpdated { get; private set; }
    
    private Student(Guid id, int sequence, string name, string email, string phoneNumber)
    {
        Id = id;
        Sequence = sequence;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        CreatedAt = DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;
    }

    public static Student FromCreatedEvent(MessageBody<StudentCreatedData> @event)
        => new (
            @event.AggregateId,
            @event.Sequence,
            @event.Data.Name,
            @event.Data.Email,
            @event.Data.PhoneNumber
            );

    public void Apply(MessageBody<StudentUpdatedData> @event)
    {
        this.Sequence = @event.Sequence;
        this.Name = @event.Data.Name;
        this.Email = @event.Data.Email;
        this.PhoneNumber = @event.Data.PhoneNumber;
        this.LastUpdated = DateTime.UtcNow;
    }
}

