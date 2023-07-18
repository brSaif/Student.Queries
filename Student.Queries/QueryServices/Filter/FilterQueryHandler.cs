using MediatR;
using StudentQueries.Data;

namespace StudentQueries.QueryServices.Filter;

public class FilterQueryHandler : IRequestHandler<FilterQuery, FilterResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public FilterQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FilterResult> Handle(FilterQuery request, CancellationToken cancellationToken)
        => await _unitOfWork.StudentRepository.FilterAsync(request, cancellationToken);
}