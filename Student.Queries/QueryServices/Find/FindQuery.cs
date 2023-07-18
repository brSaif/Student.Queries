using FluentValidation;
using MediatR;
using Student.Query.StudentProto;

namespace StudentQueries.QueryServices.Find;

public record FindQuery(Guid Id) : IRequest<Domain.Student>;

public class FindRequestValidator : AbstractValidator<FindRequest>
{
    public FindRequestValidator()
    {
        RuleFor(x => x.Id)
            .Must(id => Guid.TryParse(id, out _));
    }
}