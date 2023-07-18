using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentQueries.CreateStudent;
using StudentQueries.Data;
using StudentQueries.Exceptions.Update;
using StudentQueries.Tests.Faker;
using StudentQueries.UpdateStudent;
using Xunit.Abstractions;

namespace StudentQueries.Tests.Student;

public class UpdateStudent : TestBase
{
    public UpdateStudent(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task Update_WhenNoStudentWithTheGivenAggregateId_ThrowsApplicationException()
    {
        StudentUpdated updatedStudent = new StudentUpdatedFaker()
            .RuleFor(
                x => x.Data,
                new StudentUpdatedDataFaker()
                    .RuleFor(x => x.PhoneNumber, "9785641320"))
            .RuleFor(x => x.Sequence, 3)
            .Generate();

        using var scope = Factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var exception = await Assert.ThrowsAsync<StudentNotFoundException>(
            async () => await mediator.Send(updatedStudent));

        Assert.NotEmpty(exception.Message);
        Assert.Contains("Student with aggregate id:", exception.Message);
    }

    [Fact]
    public async Task Update_WhenTheGivenSequenceNumberBiggerThanThePersistedSequenceInDb_SequenceIsOutOfOrderException()
    {
        var studentId = Guid.NewGuid();
        StudentCreated createdStudent = new StudentCreatedFaker()
            .RuleFor(
                x => x.Data,
                new StudentCreatedDataFaker()
                    .RuleFor(x => x.PhoneNumber, "9785641320"))
            .RuleFor(x => x.AggregateId, studentId)
            .RuleFor(x => x.Sequence, 1)
            .Generate();
        
       StudentUpdated updateStudent2 = new StudentUpdatedFaker()
            .RuleFor(
                x => x.Data,
                new StudentUpdatedDataFaker()
                    .RuleFor(x => x.PhoneNumber, "9785641320"))
            .RuleFor(x => x.AggregateId, studentId)
            .RuleFor(x => x.Sequence, 3)
            .Generate();
        

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var student = Domain.Student.FromCreatedEvent(createdStudent);
        context.Students.Add(student!);
        
        await context.SaveChangesAsync();
        
        var exception = await Assert.ThrowsAsync<SequenceIsOutOfOrderException>(
            async () => await mediator.Send(updateStudent2));

        Assert.NotEmpty(exception.Message);
        Assert.Contains( "cannot be treated due to the event being out of order", exception.Message);
    }

    [Fact]
    public async Task Update_WhenTheGivenSequenceNumberHigherThePersistedStudentSequenceByMoreThanOne_StudentAlreadyUpdatedException()
    {
        var studentId = Guid.NewGuid();
        StudentCreated createdStudent = new StudentCreatedFaker()
            .RuleFor(
                x => x.Data,
                new StudentCreatedDataFaker()
                    .RuleFor(x => x.PhoneNumber, "9785641320"))
            .RuleFor(x => x.AggregateId, studentId)
            .RuleFor(x => x.Sequence, 1)
            .Generate();
        
        StudentUpdated updateStudent2 = new StudentUpdatedFaker()
            .RuleFor(
                x => x.Data,
                new StudentUpdatedDataFaker()
                    .RuleFor(x => x.PhoneNumber, "9785641320"))
            .RuleFor(x => x.AggregateId, studentId)
            .RuleFor(x => x.Sequence, 2)
            .Generate();
        

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var student = Domain.Student.FromCreatedEvent(createdStudent);
        context.Students.Add(student!);
        
        await context.SaveChangesAsync();
        student.Apply(updateStudent2);
        await context.SaveChangesAsync();
        
        var exception = await Assert.ThrowsAsync<StudentAlreadyUpdatedException>(
            async () => await mediator.Send(updateStudent2));

        Assert.NotEmpty(exception.Message);
        Assert.Contains( "Already treated", exception.Message);
    }

    [Fact]
    public async Task Update_WhenGivenValidData_UpdatesTheStudent()
    {
        var studentId = Guid.NewGuid();
        StudentCreated createdStudent = new StudentCreatedFaker()
            .RuleFor(
                x => x.Data,
                new StudentCreatedDataFaker()
                    .RuleFor(x => x.PhoneNumber, "9785641320"))
            .RuleFor(x => x.AggregateId, studentId)
            .RuleFor(x => x.Sequence, 1)
            .Generate();
        
        StudentUpdated updateStudent1 = new StudentUpdatedFaker()
            .RuleFor(
                x => x.Data,
                new StudentUpdatedDataFaker()
                    .RuleFor(x => x.PhoneNumber, "9785641320"))
            .RuleFor(x => x.AggregateId, studentId)
            .RuleFor(x => x.Sequence, 2)
            .Generate();
        
        using var scope = Factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        
        var creationResult = await mediator.Send(createdStudent);
        var updatingResult = await mediator.Send(updateStudent1);
        var response = await context.Students.FirstAsync(x => x.Id == studentId);
        
        
        Assert.True(creationResult);
        Assert.True(updatingResult);
        Assert.Equal(response.Sequence, updateStudent1.Sequence);
        Assert.Equal(response.Name, updateStudent1.Data.Name);
        Assert.Equal(response.Email, updateStudent1.Data.Email);
        Assert.Equal(response.PhoneNumber, updateStudent1.Data.PhoneNumber);
    }
}