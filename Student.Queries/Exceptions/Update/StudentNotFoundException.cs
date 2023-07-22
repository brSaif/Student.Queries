namespace StudentQueries.Exceptions.Update;

public class StudentNotFoundException : StudentException
{
    public StudentNotFoundException(Guid AggregateId) : base(string.Format(Phrases.StudentNotFoundException, AggregateId))
    {
    }
}