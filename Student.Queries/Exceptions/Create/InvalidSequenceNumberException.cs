namespace StudentQueries.Exceptions.Create;

public class InvalidSequenceNumberException : StudentException
{
    public InvalidSequenceNumberException(int sequence) 
        : base($"Invalid sequence number '{sequence}'")
    {
    }
}