namespace StudentQueries.Exceptions.Create;

public class StudentAlreadyExistException : StudentException
{
    public StudentAlreadyExistException(Guid AggregateId) 
        : base(string.Format(Phrases.StudentAlreadyExistException, AggregateId))
    {
    }
}