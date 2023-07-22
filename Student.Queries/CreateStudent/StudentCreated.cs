using MediatR;
using Newtonsoft.Json;
using StudentQueries.Services;

namespace StudentQueries.CreateStudent;


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