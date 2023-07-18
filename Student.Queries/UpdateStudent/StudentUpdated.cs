using MediatR;
using Newtonsoft.Json;
using StudentQueries.Services;

namespace StudentQueries.UpdateStudent;

public record StudentUpdated : Event<StudentUpdatedData>, IRequest<bool>
{
    public Guid AggregateId { get; private set; }
    public int Sequence { get; private set; }
    public DateTime DateTime { get; private set; }
    public StudentUpdatedData Data { get; private set; }
    public int Version { get; private set; }

    public StudentUpdated(
        Guid AggregateId, 
        int Sequence,
        DateTime DateTime, 
        StudentUpdatedData Data, int version)
    : base(AggregateId, Sequence, DateTime, Data, version)
    {
        this.AggregateId = AggregateId;
        this.Sequence = Sequence;
        this.DateTime = DateTime;
        this.Data = Data;
        Version = version;
    }

    private StudentUpdated() : base()
    { }
    
}


public record StudentUpdatedData : IEventData
{
    [JsonProperty("Name")]
    public string Name { get; private set; }
    
    [JsonProperty("Email")]
    public string Email { get; private set; }
    
    [JsonProperty("PhoneNumber")]
    public string PhoneNumber { get; private set; }

    public StudentUpdatedData(string Name, string Email, string PhoneNumber)
    {
        this.Name = Name;
        this.Email = Email;
        this.PhoneNumber = PhoneNumber;
    }

    public StudentUpdatedData()
    {
        
    }
}