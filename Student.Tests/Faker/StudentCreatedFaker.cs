using StudentQueries.CreateStudent;

namespace StudentQueries.Tests.Faker;

public class StudentCreatedFaker : PrivateFaker<StudentCreated>
{
    public StudentCreatedFaker()
    {
        UsePrivateConstructor();
        
        RuleFor(r => r.AggregateId, f => f.Random.Guid());
        RuleFor(r => r.Sequence, 1);
    }
}

public class StudentCreatedDataFaker : PrivateFaker<StudentCreatedData>
{
    public StudentCreatedDataFaker()
    {
        UsePrivateConstructor();
        
        RuleFor(r => r.Name, f => f.Person.FullName);
        RuleFor(r => r.Email, f => f.Person.Email);
        RuleFor(r => r.PhoneNumber, f => f.Random.AlphaNumeric(9));
    } 
}