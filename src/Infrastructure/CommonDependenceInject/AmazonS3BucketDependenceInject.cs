using Infrastructure.Amazon;
using Infrastructure.Amazon.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.CommonDependenceInject;
public static class AmazonS3BucketDependenceInject
{
    public static void AddAmazonS3BucketConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AmazonS3Options>(configuration.GetSection("AmazonS3Configurations"));
        services.AddSingleton<IAmazonS3Bucket, AmazonS3Bucket>();
    }
}