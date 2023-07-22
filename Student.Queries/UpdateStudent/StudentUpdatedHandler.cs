using MediatR;
using StudentQueries.Data;
using StudentQueries.Exceptions.Update;
using StudentQueries.Services;

namespace StudentQueries.UpdateStudent;

public class StudentUpdatedHandler : IRequestHandler<MessageBody<StudentUpdatedData>, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StudentUpdatedHandler> _logger;

    public StudentUpdatedHandler(
        IUnitOfWork unitOfWork,
        ILogger<StudentUpdatedHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(MessageBody<StudentUpdatedData> request, CancellationToken cancellationToken)
    {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(request.AggregateId);
            if (student is null)
            {
                _logger.LogInformation($"Student with aggregate id: '{request.AggregateId}' not found");
                throw new StudentNotFoundException(request.AggregateId);
            };

            if (request.Sequence - 1 < student.Sequence)
            {
                _logger.LogInformation($"Update request id: '{request.AggregateId}' Already treated");
                throw new StudentAlreadyUpdatedException(request.AggregateId);
            }

        
            if (student.Sequence != request.Sequence - 1)
            {
                _logger.LogInformation($"Update request id: '{request.AggregateId}' cannot be treated due to the event being out of order");
                throw new SequenceIsOutOfOrderException(request.AggregateId);
            }

            student.Apply(request);

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Student with aggregate id: '{request.AggregateId}' successfully updated");
            return true;


    }

}