using MediatR;

namespace StudentQueries.QueryServices.Filter;

public record FilterQuery(
    int Page, 
    int Size,
    DateTime? CreatedAfter) 
    : IRequest<FilterResult>
{
    public int Skip => Size * (Page - 1);
}