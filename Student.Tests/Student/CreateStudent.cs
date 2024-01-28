using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentQueries.CreateStudent;
using StudentQueries.Data;
using StudentQueries.Exceptions.Create;
using StudentQueries.Tests.Faker;
using StudentQueries.UpdateStudent;
using Xunit.Abstractions;

namespace StudentQueries.Tests.Student;

public class CreateStudent : TestBase
{
    public CreateStudent(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task Create_WhenGivenInvalidSequenceNumber_ThrowsApplicationException()
    {
        var  createdStudent = new StudentCreatedFaker()
            .RuleFor(
                x => x.Data,
                new StudentCreatedDataFaker()
                    .RuleFor(x => x.PhoneNumber, "9785641320"))
            .RuleFor(x => x.Sequence, 3)
            .Generate();

        using var scope = Factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var exception = await Assert.ThrowsAsync<InvalidSequenceNumberException>(
            async () => await mediator.Send(createdStudent));

        Assert.NotEmpty(exception.Message);
        Assert.Contains("Invalid sequence number", exception.Message);
    }


    [Fact]
    public async Task Create_WhenAStudentWithTheSameAggregateIdExists_ThrowsApplicationException()
    {
        var studentId = Guid.NewGuid();
        var createdStudent = new StudentCreatedFaker()
            .RuleFor(
                x => x.Data,
                new StudentCreatedDataFaker()
                    .RuleFor(x => x.PhoneNumber, "9785641320"))
            .RuleFor(x => x.AggregateId, studentId)
            .RuleFor(x => x.Sequence, 1)
            .Generate();

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var student = Domain.Student.FromCreatedEvent(createdStudent);
        context.Students.Add(student!);
        
        var exception = await Assert.ThrowsAsync<StudentAlreadyExistException>(
            async () => await mediator.Send(createdStudent));

        Assert.NotEmpty(exception.Message);
        Assert.Contains( "already exist", exception.Message);
    }


    [Fact]
    public async Task Create_WhenGivenValidData_CreatesTheStudent()
    {
        var studentId = Guid.NewGuid();
        var createdStudent = new StudentCreatedFaker()
            .RuleFor(
                x => x.Data,
                new StudentCreatedDataFaker()
                    .RuleFor(x => x.PhoneNumber, "9785641320"))
            .RuleFor(x => x.AggregateId, studentId)
            .RuleFor(x => x.Sequence, 1)
            .Generate();
        
        using var scope = Factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        
        var creationResult = await mediator.Send(createdStudent);
        var response = await context.Students.FirstAsync(x => x.Id == studentId);
        
        
        Assert.True(creationResult);
        Assert.Equal(response.Sequence, createdStudent.Sequence);
        Assert.Equal(response.Name, createdStudent.Data.Name);
        Assert.Equal(response.Email, createdStudent.Data.Email);
        Assert.Equal(response.PhoneNumber, createdStudent.Data.PhoneNumber);
    }
}