namespace StudentQueries.Exceptions.Update;

public class StudentAlreadyUpdatedException : StudentException
{
    public StudentAlreadyUpdatedException(Guid AggregateId) : base(string.Format(Phrases.StudentAlreadyUpdatedException, AggregateId))
    { }
}