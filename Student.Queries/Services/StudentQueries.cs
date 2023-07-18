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
                throw new RpcException(new Status(StatusCode.NotFound, "No entities matched your queries"));
        
        return result.ToFilterResponse();
    }

    public override async Task<StudentResult?> Find(FindRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(request.ToQuery())
            ?? throw new RpcException(
                new Status(StatusCode.NotFound, $"Student with aggregate id: '{request.Id}' not found")); 

        return result.ToFindResponse();
    }
}