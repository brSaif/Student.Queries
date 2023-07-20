using Grpc.Core;
using MediatR;
using Newtonsoft.Json;
using Student.Query.EventHistory;
using Student.Query.TriggerBuild;
using StudentQueries.CreateStudent;
using StudentQueries.Extensions;
using StudentQueries.UpdateStudent;

namespace StudentQueries.Services;

public class TriggerHistoryBuild : TriggerBuild.TriggerBuildBase
{
    private readonly EventHistory.EventHistoryClient _client;
    private readonly IMediator _mediator;
    private readonly ILogger<TriggerHistoryBuild> _logger;

    public TriggerHistoryBuild(
        EventHistory.EventHistoryClient client, 
        IMediator mediator,
        ILogger<TriggerHistoryBuild> logger)
    {
        _client = client;
        _mediator = mediator;
        _logger = logger;
    }
    
    public override async Task<Empty> Build(Empty request, ServerCallContext context)
    {
        _logger.LogInformation("a gRPC request to build state from history is received ....");
        
        for (int i = 1; i >0; i++)
        {
            var getEventsRequest = new GetEventsRequest()
            {
                CurrentPage = i,
                PageSize = 3
            };
            
            var response = await _client.GetEventsAsync(getEventsRequest);

            if (response.Events.Count > 0)
            {
                await HandleResponseAsync(response);
            }
            else
            {
                break;
            }
        }
        _logger.LogInformation("Building state from history operation successfully ended.");
        return new Empty();
    }

    private async Task HandleResponseAsync(Response response)
    {
        foreach (EventMessage eventMessage in response.Events)
        {
            var json = JsonConvert.SerializeObject(MessageBody.MapTo(eventMessage));
            
            var _ = eventMessage.Type switch
            {
                nameof(StudentCreated) => await _mediator.Send(json.Deserialize<StudentCreated>()),
                nameof(StudentUpdated) => await _mediator.Send(json.Deserialize<StudentUpdated>()),
                _ => false
            };
        }
    }

    
    
    
}