using Student.Query.StudentProto;
using StudentQueries.QueryServices.Filter;
using StudentQueries.QueryServices.Find;

namespace StudentQueries.Extensions;

public static class QueryExtensions
{
    public static FilterResponse ToFilterResponse(this FilterResult result)
        => new ()
        {
            Page = result.Page,
            Size = result.Size,
            Total = result.Total,
            Students =
            {
                result.Students.Select(x => new StudentResult()
                    {
                        Id = x.Id.ToString(),
                        Name = x.Name,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber
                    }
                )
            }
        };

    public static StudentResult ToFindResponse(this Domain.Student result)
        => new()
        {
            Id = result.Id.ToString(),
            Name = result.Name,
            Email = result.Email,
            PhoneNumber = result.PhoneNumber
        };

    public static FilterQuery ToQuery(this FilterRequest request)
        => new(request.Page, request.Size, request.CreatedAfter.ToDateTime());

    public static FindQuery ToQuery(this FindRequest request)
        => new(Guid.Parse(request.Id));
}