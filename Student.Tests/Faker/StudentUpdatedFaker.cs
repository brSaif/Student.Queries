using StudentQueries.UpdateStudent;

namespace StudentQueries.Tests.Faker;

public class StudentUpdatedFaker: PrivateFaker<StudentUpdated>
{
    public StudentUpdatedFaker()
    {
        UsePrivateConstructor();
        
        RuleFor(r => r.AggregateId, f => f.Random.Guid());
        RuleFor(r => r.Sequence, 1);
    }
}

public class StudentUpdatedDataFaker : PrivateFaker<StudentUpdatedData>
{
    public StudentUpdatedDataFaker()
    {
        UsePrivateConstructor();
        
        RuleFor(r => r.Name, f => f.Person.FullName);
        RuleFor(r => r.Email, f => f.Person.Email);
        RuleFor(r => r.PhoneNumber, f => f.Random.AlphaNumeric(9));
    } 
}