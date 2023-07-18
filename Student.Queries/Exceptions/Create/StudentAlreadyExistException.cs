namespace StudentQueries.Exceptions.Create;

public class StudentAlreadyExistException : StudentException
{
    public StudentAlreadyExistException(Guid AggregateId) 
        : base($"A student with '{AggregateId}' already exist")
    {
    }
}