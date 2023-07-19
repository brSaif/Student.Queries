using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentEvents.Tests;
using StudentQueries.Data;
using StudentQueries.Services;
using StudentQueries.Tests;
using Xunit.Abstractions;

namespace Student.Live.Tests.Student;

public class Create : TestBase
{
    public const int Delay = 3_000;
    public Create(ITestOutputHelper output) : base(output)
    {
    }
    
    [Fact]
    public async Task Create_WhenGivenValidData_CreatesAStudent()
    {
        
        var grpcClient = new DemoEvents.DemoEventsClient(GrpcChannel.ForAddress("https://localhost:5001"));
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var studentId = Guid.NewGuid();
        var request = new CreateRequest()
        {
            AggregateId = studentId.ToString(),
            Name = "Super Unique Name",
            Email = "Test@Test.com",
            PhoneNumber = "0123654789"
        };
        
        await grpcClient.CreateAsync(request);

        await Task.Delay(Delay);

        await Factory.Services.GetRequiredService<IHostedService>().StopAsync(new CancellationToken(false));

        var response = await context.Students.FirstAsync(x => x.Id == studentId);
        
        Assert.NotNull(response);
        Assert.Equal(1, response.Sequence);
        Assert.Equal(request.Name, response.Name );
        Assert.Equal(request.Email, response.Email );
        Assert.Equal(request.PhoneNumber, response.PhoneNumber );
    }
}