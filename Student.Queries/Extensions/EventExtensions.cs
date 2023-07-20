using Newtonsoft.Json;

namespace StudentQueries.Extensions;

public static class EventExtensions
{
    public static TEvent Deserialize<TEvent>(this string json) 
        => JsonConvert.DeserializeObject<TEvent>(json) 
           ?? throw new ArgumentException("Deserialization failed");
}