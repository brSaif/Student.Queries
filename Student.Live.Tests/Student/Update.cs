using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentEvents.Tests;
using StudentQueries.Data;
using StudentQueries.Tests;
using Xunit.Abstractions;

namespace Student.Live.Tests.Student;

public class Update : TestBase
{
    private const int Delay = 7_000;

    public Update(ITestOutputHelper output) : base(output)
    { }

    [Fact]
    public async Task Create_WhenGivenValidData_CreatesAStudent()
    {
        var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback
            = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        var httpClient = new HttpClient(httpHandler);
        using var channel = GrpcChannel.ForAddress(
            "https://localhost:5001", 
            new GrpcChannelOptions { HttpClient = httpClient }
        );
        
        var grpcClient = new DemoEvents.DemoEventsClient(channel);
        var studentId = Guid.NewGuid();
        var createRequest = new CreateRequest()
        {
            AggregateId = studentId.ToString(),
            Name = "Super Unique Name",
            Email = "Test@Test.com",
            PhoneNumber = "0123654789"
        };
        
        var updatRequest = new UpdateStudentRequest()
        {
            AggregateId = studentId.ToString(),
            Sequence = 2,
            Name = "Super Unique Name",
            Email = "Test@Test.com",
            PhoneNumber = "0123654789"
        };
        
        
        await grpcClient.CreateAsync(createRequest);
        await Task.Delay(Delay);
        await grpcClient.UpdateAsync(updatRequest);
        await Task.Delay(Delay);
        
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var response = await context.Students.FirstAsync(x => x.Id == studentId);
        
        Assert.NotNull(response);
        Assert.Equal(2, response.Sequence);
        Assert.Equal(updatRequest.Name, response.Name );
        Assert.Equal(updatRequest.Email, response.Email );
        Assert.Equal(updatRequest.PhoneNumber, response.PhoneNumber );
    }
}