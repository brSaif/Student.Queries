using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using Azure.Messaging.ServiceBus;
using MediatR;
using Newtonsoft.Json;
using StudentQueries.CreateStudent;
using StudentQueries.Exceptions;
using StudentQueries.Exceptions.Create;
using StudentQueries.Exceptions.Update;
using StudentQueries.UpdateStudent;

namespace StudentQueries.Services;

public class EventListener : IHostedService, IDisposable
{
    private readonly IServiceProvider _sp;
    private readonly ServiceBusSessionProcessor _processor;
    // private readonly object syncRoot = new();

    public EventListener(
        ServiceBusClient client, 
        IServiceProvider sp
        )
    {
        _sp = sp;

        var options = new ServiceBusSessionProcessorOptions
        {
            AutoCompleteMessages = false,
            PrefetchCount = 1,
            MaxConcurrentCallsPerSession = 1,
            MaxConcurrentSessions = 100,
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        };

        _processor = client.CreateSessionProcessor(
            "brseif", 
            "es-demo-queries", options);

        // configure the message and error handler to use .
        _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
        _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

    }


    private async Task Processor_ProcessMessageAsync(ProcessSessionMessageEventArgs arg)
    {
        var IsHandled = false;
        try
        {
            IsHandled = await TryHandleAsync(arg.Message);
            await arg.CompleteMessageAsync(arg.Message);
        }
        catch (StudentException  e)
        {
            List<string> ExceptionsThatShouldntBlockMessageDeletion = new()
            {
                nameof(StudentAlreadyExistException),
                nameof(StudentAlreadyUpdatedException),
                nameof(InvalidSequenceNumberException),
            };

            if (ExceptionsThatShouldntBlockMessageDeletion.Any(x => x == e.GetType().Name))
            {
                await arg.CompleteMessageAsync(arg.Message);
            }
            throw;
        }
    }

    private async Task<bool> TryHandleAsync(ServiceBusReceivedMessage argMessage)
    {
        using var scope = _sp.CreateScope();
        var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
        var json = Encoding.UTF8.GetString(argMessage.Body);
        
        return argMessage.Subject switch
        {
            nameof(StudentCreated) => await mediatr.Send(Deserialize<StudentCreated>(json)),
            nameof(StudentUpdated) => await mediatr.Send(Deserialize<StudentUpdated>(json)),
            _ => false
        };
    }

    public static TEvent Deserialize<TEvent>(string json) 
        => JsonConvert.DeserializeObject<TEvent>(json) 
           ?? throw new ArgumentException("Deserialization failed");
    
    private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        throw arg.Exception;
    }

    public Task CloseAsync() => _processor.CloseAsync();

    public Task StartAsync(CancellationToken cancellationToken)
        => _processor.StartProcessingAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken)
        => _processor.StopProcessingAsync(cancellationToken);

    public void Dispose()
    {
        _processor.DisposeAsync();
    }
}