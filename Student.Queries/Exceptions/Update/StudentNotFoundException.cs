namespace StudentQueries.Exceptions.Update;

public class StudentNotFoundException : StudentException
{
    public StudentNotFoundException(Guid AggregateId) : base($"Student with aggregate id: '{AggregateId}' not found")
    {
    }
}