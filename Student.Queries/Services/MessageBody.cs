using MediatR;
using Newtonsoft.Json;
using Student.Query.EventHistory;

namespace StudentQueries.Services;

public class MessageBody<T>  : IRequest<bool>
    where T : IEventData
{
   public Guid AggregateId { get; set; }
    public int Sequence { get; set; }
    public string Type { get; set; }
    public T Data { get; set; }
    public DateTime DateTime { get; set; }
    public int Version { get; set; }
    
    public static MessageBody<T> MapTo(EventMessage eventMessage)
    
    {
        var msgBody = new MessageBody<T>();
        msgBody.AggregateId = Guid.Parse(eventMessage.AggregateId);
        msgBody.Sequence = eventMessage.Sequence;
        msgBody.DateTime = eventMessage.DateTime.ToDateTime();
        msgBody.Data = JsonConvert.DeserializeObject<T>(eventMessage.Data);
        msgBody.Type = eventMessage.Type;
        msgBody.Version = msgBody.Version;

        return msgBody;
    }
}