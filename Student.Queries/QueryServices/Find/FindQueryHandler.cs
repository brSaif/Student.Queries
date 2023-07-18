using MediatR;
using StudentQueries.Data;

namespace StudentQueries.QueryServices.Find;

public class FindQueryHandler : IRequestHandler<FindQuery, Domain.Student?>
{
    private readonly IUnitOfWork _unitOfWork;

    public FindQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Domain.Student?> Handle(FindQuery request, CancellationToken cancellationToken)
        => await _unitOfWork.StudentRepository.FindAsync(request.Id, cancellationToken);
}