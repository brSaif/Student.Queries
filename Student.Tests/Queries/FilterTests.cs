using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Student.Query.StudentProto.Test;
using StudentQueries.Data;
using StudentQueries.Tests.Faker;
using Xunit.Abstractions;

namespace StudentQueries.Tests.Queries;

public class FilterTests : TestBase
{
    public FilterTests(ITestOutputHelper output) : base(output)
    {
    }
    
    [Fact]
    public async Task Filter_WhenNoEntityMatchesTheQueryFilter_ThrowNotFoundException()
    {
        var grpcClient = new Students.StudentsClient(Channel);
        
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await grpcClient.FilterAsync(new FilterRequest(){CreatedAfter = DateTime.UtcNow.ToTimestamp(), Page = 1, Size = 1}));
        
        Assert.NotNull(exception.Message);
        Assert.Equal(StatusCode.NotFound, exception.StatusCode); 
    }
    
    [Theory]
    [InlineData(15, 1, 33)]
    [InlineData(15, 3, 5)]
    [InlineData(12, 3, 5)]
    public async Task Filter_WhenTheQueryMatchesTheData_ReturnTheExpectedResult(
        int studentNbr, int page, int size)
    {
        var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var students = Generate(studentNbr);
        await context.Students.AddRangeAsync(students);
        await context.SaveChangesAsync();

        var grpcClient = new Students.StudentsClient(Channel);
        
        var result = await grpcClient.FilterAsync(new FilterRequest()
        {
            CreatedAfter = DateTime.Parse("07/07/2023").ToUniversalTime().ToTimestamp(), Page = page, Size = size
        });
        var expectedList = await context.Students.AsQueryable()
            .Where(c => c.CreatedAt >= DateTime.Parse("07/07/2023"))
            .Skip(size * (page - 1))
            .Take(size)
            .ToListAsync(CancellationToken.None);
        
        Assert.NotEmpty(result.Students);
        Assert.Equal(expectedList.Count, result.Students.Count);
        Assert.Equal(expectedList.First().Id.ToString(), result.Students.First().Id);
        Assert.Equal(expectedList.First().Name, result.Students.First().Name);
        Assert.Equal(expectedList.First().Email, result.Students.First().Email);
        Assert.Equal(expectedList.First().PhoneNumber, result.Students.First().PhoneNumber);
    }
    
    private IList<Domain.Student> Generate( int nbr = 1)
    {
        var  createdStudent = new StudentCreatedFaker()
            .RuleFor(
                x => x.Data,
                new StudentCreatedDataFaker()
                    .RuleFor(x => x.PhoneNumber, "9785641320"))
            .RuleFor(x => x.Sequence, 1)
            .Generate(nbr);

        return createdStudent.Select(Domain.Student.FromCreatedEvent).ToList();
    }
}