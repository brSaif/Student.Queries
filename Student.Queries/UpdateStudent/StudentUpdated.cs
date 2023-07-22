using MediatR;
using Newtonsoft.Json;
using StudentQueries.Services;

namespace StudentQueries.UpdateStudent;

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