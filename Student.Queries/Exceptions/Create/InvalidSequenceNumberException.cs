namespace StudentQueries.Exceptions.Create;

public class InvalidSequenceNumberException : StudentException
{
    public InvalidSequenceNumberException(int sequence) 
        : base(string.Format(Phrases.InvalidSequenceNumberException, sequence))
    {
    }
}