namespace StudentQueries.QueryServices.Filter;

public record FilterResult(
    int Page, 
    int Size,
    int Total,
    List<Domain.Student> Students);