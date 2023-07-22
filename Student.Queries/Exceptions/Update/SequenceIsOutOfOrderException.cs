namespace StudentQueries.Exceptions.Update;

public class SequenceIsOutOfOrderException : StudentException
{
    public SequenceIsOutOfOrderException(Guid AggregateId) : base(string.Format(Phrases.SequenceIsOutOfOrderException, AggregateId))
    {
    }
}