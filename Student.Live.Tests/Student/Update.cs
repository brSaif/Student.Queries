using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentEvents.Tests;
using StudentQueries.Data;
using StudentQueries.Tests;
using StudentQueries.Tests.Faker;
using Xunit.Abstractions;

namespace Student.Live.Tests.Student;

public class Update : TestBase
{
    private const int Delay = 3_000;

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
        
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var studentId = Guid.NewGuid();
        var student = Generate(studentId);
        await context.Students.AddAsync(student);
        await context.SaveChangesAsync();
        
        var updateRequest = new UpdateStudentRequest()
        {
            AggregateId = studentId.ToString(),
            Sequence = 2,
            Name = "Super Unique Name",
            Email = "Test@Test.com",
            PhoneNumber = "0123654789"
        };

        await grpcClient.UpdateAsync(updateRequest);
        await Task.Delay(Delay);
        
        await Factory.Services.GetRequiredService<IHostedService>().StopAsync(new CancellationToken(false));        
        await Task.Delay(5_000);
        await context.Entry(student).ReloadAsync();
        
        Assert.NotNull(student);
        Assert.Equal(updateRequest.Sequence, student.Sequence);
        Assert.Equal(updateRequest.Name, student.Name );
        Assert.Equal(updateRequest.Email, student.Email );
        Assert.Equal(updateRequest.PhoneNumber, student.PhoneNumber );
    }
    
    
    private StudentQueries.Domain.Student Generate( Guid id)
    {
        var  createdStudent = new StudentCreatedFaker()
            .RuleFor(
                x => x.Data,
                new StudentCreatedDataFaker()
                    .RuleFor(x => x.PhoneNumber, "9785641320"))
            .RuleFor(x => x.Sequence, 1)
            .RuleFor(x => x.AggregateId, id)
            .Generate();

        return StudentQueries.Domain.Student.FromCreatedEvent(createdStudent);
    }
}