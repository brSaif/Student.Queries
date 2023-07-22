using System.Globalization;
using Calzolari.Grpc.AspNetCore.Validation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using StudentQueries.QueryServices.Find;

namespace StudentQueries.Extensions;

public static class GrpcValidationExtensions
{
    public static void AddGrpcWithValidators(this IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.EnableMessageValidation();
            // options.Interceptors.Add<ThreadCultureInterceptor>();
            // options.Interceptors.Add<ApplicationExceptionInterceptor>();
        });
        
        services.AddGrpcValidation();
        services.AddValidator<FindRequestValidator>();
    }
}

public class ThreadCultureInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var headerLanguage = context.RequestHeaders.FirstOrDefault(t => t.Key == "language");

        if (headerLanguage != null)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(headerLanguage.Value);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(headerLanguage.Value);
        }

        return await continuation(request, context);
    }


}

public class ApplicationExceptionInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception exception)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, exception.Message));
        }
    }
}