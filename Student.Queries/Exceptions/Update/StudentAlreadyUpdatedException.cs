namespace StudentQueries.Exceptions.Update;

public class StudentAlreadyUpdatedException : StudentException
{
    public StudentAlreadyUpdatedException(Guid AggregateId) : base($"Update request for student with aggregate id: '{AggregateId}' Already treated")
    { }
}