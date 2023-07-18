using Calzolari.Grpc.AspNetCore.Validation;
using StudentQueries.QueryServices.Find;

namespace StudentQueries.Extensions;

public static class GrpcValidationExtensions
{
    public static void AddGrpcWithValidators(this IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.EnableMessageValidation();
        });
        
        services.AddGrpcValidation();
        services.AddValidator<FindRequestValidator>();
    }
}