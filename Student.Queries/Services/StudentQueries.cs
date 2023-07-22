using Grpc.Core;
using MediatR;
using Student.Query.StudentProto;
using StudentQueries.Exceptions.Update;
using StudentQueries.Extensions;

namespace StudentQueries.Services;

public class StudentQueries : Students.StudentsBase
{
    private readonly IMediator _mediator;

    public StudentQueries(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public override async Task<FilterResponse> Filter(FilterRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(request.ToQuery()); 
        
        if (!result.Students.Any()) 
                throw new RpcException(new Status(StatusCode.NotFound, Phrases.NoResultMatchedTheQuery));
        
        return result.ToFilterResponse();
    }

    public override async Task<StudentResult?> Find(FindRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(request.ToQuery())
            ?? throw new RpcException(
                new Status(StatusCode.NotFound, String.Format(Phrases.StudentNotFoundException, request.Id))); 

        return result.ToFindResponse();
    }
}