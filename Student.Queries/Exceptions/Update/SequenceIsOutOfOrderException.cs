namespace StudentQueries.Exceptions.Update;

public class SequenceIsOutOfOrderException : StudentException
{
    public SequenceIsOutOfOrderException(Guid AggregateId) : base($"Update request id: '{AggregateId}' cannot be treated due to the event being out of order")
    {
    }
}