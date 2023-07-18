using MediatR;
using Newtonsoft.Json;
using StudentQueries.Services;

namespace StudentQueries.CreateStudent;

public record StudentCreated :  Event<StudentCreatedData>, IRequest<bool>
{
    public Guid AggregateId { get; private set; }
    public int Sequence { get; private set; }
    
    public DateTime DateTime { get; private set; }
    public StudentCreatedData Data { get; private set; }
    public int Version { get; private set; }
    
    public StudentCreated(
        Guid AggregateId, 
        int Sequence, 
        DateTime DateTime, 
        StudentCreatedData Data, int Version)
    : base(AggregateId, Sequence, DateTime, Data, Version)
    {
        this.AggregateId = AggregateId;
        this.Sequence = Sequence;
        this.DateTime = DateTime;
        this.Data = Data;
        this.Version = Version;
    }

    private StudentCreated()
    { }

}

public record StudentCreatedData : IEventData
{
    [JsonProperty("Name")]
    public string Name { get; private set; } 

    [JsonProperty("Email")]
    public string Email { get; private set; }
    
    [JsonProperty("PhoneNumber")]
    public string PhoneNumber { get; private set; }

    public StudentCreatedData(string Name, string Email, string PhoneNumber)
    {
        this.Name = Name;
        this.Email = Email;
        this.PhoneNumber = PhoneNumber;
    }

    public StudentCreatedData()
    {
        
    }

}