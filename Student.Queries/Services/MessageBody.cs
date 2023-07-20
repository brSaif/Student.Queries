using Newtonsoft.Json;
using Student.Query.EventHistory;

namespace StudentQueries.Services;

public class MessageBody
{
   public Guid AggregateId { get; set; }
    public int Sequence { get; set; }
    public string Type { get; set; }
    public object Data { get; set; }
    public DateTime DateTime { get; set; }
    public int Version { get; set; }
    
    public static MessageBody MapTo(EventMessage eventMessage)
    
    {
        var msgBody = new MessageBody();
        msgBody.AggregateId = Guid.Parse(eventMessage.AggregateId);
        msgBody.Sequence = eventMessage.Sequence;
        msgBody.DateTime = eventMessage.DateTime.ToDateTime();
        msgBody.Data = JsonConvert.DeserializeObject(eventMessage.Data);
        msgBody.Type = eventMessage.Type;
        msgBody.Version = msgBody.Version;

        return msgBody;
    }
}