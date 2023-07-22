using MediatR;
using StudentQueries.Data;
using StudentQueries.Domain;
using StudentQueries.Exceptions.Create;
using StudentQueries.Services;
using StudentQueries.UpdateStudent;

namespace StudentQueries.CreateStudent;

public class StudentCreatedHandler : IRequestHandler<MessageBody<StudentCreatedData>, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StudentUpdatedHandler> _logger;

    public StudentCreatedHandler(IUnitOfWork unitOfWork,
        ILogger<StudentUpdatedHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<bool> Handle(MessageBody<StudentCreatedData> request, CancellationToken cancellationToken)
    {
            if (request.Sequence > 1)
            {
                _logger.LogInformation($"Invalid sequence number");
                throw new InvalidSequenceNumberException(request.Sequence);
            }

            var student = await _unitOfWork.StudentRepository.GetByIdAsync(request.AggregateId);


            if (student is not null)
            {
                _logger.LogInformation($"A student with '{request.AggregateId}' already exist");
                throw new StudentAlreadyExistException(request.AggregateId);
            }
        
            var student2 = Domain.Student.FromCreatedEvent(request);
            await _unitOfWork.StudentRepository.AddAsync(student2);
            await _unitOfWork.SaveChangesAsync();
            return true;
    }
}