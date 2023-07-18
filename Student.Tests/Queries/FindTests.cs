using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Student.Query.StudentProto.Test;
using StudentQueries.CreateStudent;
using StudentQueries.Data;
using StudentQueries.Exceptions.Update;
using StudentQueries.Tests.Faker;
using Xunit.Abstractions;

namespace StudentQueries.Tests.Queries;

public class FindTests : TestBase
{
    public FindTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void Find_GivenValidIdAndEntityExists_ReturnTheEntity()
    {

        var userId = Guid.NewGuid();
        
        var grpcClient = new Students.StudentsClient(Channel);
        var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var student = Generate(userId).First();
        context.Students.AddRange(student);
        context.SaveChanges();

        var result = grpcClient.Find(new FindRequest() { Id = userId.ToString() });
        
        Assert.NotNull(result);
        Assert.Equal(Guid.Parse(result.Id), student.Id);
        Assert.Equal(result.Name, student.Name);
        Assert.Equal(result.Email, student.Email);
        Assert.Equal(result.PhoneNumber, student.PhoneNumber);
    }
    
    [Fact]
    public async Task Find_NoEntityExistsForTheGivenValidId_ThrowNotFoundException()
    {
        var grpcClient = new Students.StudentsClient(Channel);
        
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => grpcClient.Find(new FindRequest(){Id = Guid.NewGuid().ToString()}));
        
        Assert.NotNull(exception.Message);
        Assert.Equal(StatusCode.NotFound, exception.StatusCode); 
    }
    
    [Fact]
    public async Task Find_GivenInvalidId_ThrowsInvalidArgumentException()
    {
        var grpcClient = new Students.StudentsClient(Channel);
        
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await grpcClient.FindAsync(new FindRequest(){Id = "invaliduserid"}));
        
        Assert.NotNull(exception.Message);
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode); 
    }

    private IList<Domain.Student> Generate(Guid id, int nbr = 1)
    {
        var  createdStudent = new StudentCreatedFaker()
            .RuleFor(
                x => x.Data,
                new StudentCreatedDataFaker()
                    .RuleFor(x => x.PhoneNumber, "9785641320"))
            .RuleFor(x => x.AggregateId, id)
            .RuleFor(x => x.Sequence, 1)
            .Generate(nbr);

        return createdStudent.Select(Domain.Student.FromCreatedEvent).ToList();
    }
}