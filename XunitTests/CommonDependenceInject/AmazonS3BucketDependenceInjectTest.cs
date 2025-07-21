using Despesas.Infrastructure.Amazon;
using Despesas.Infrastructure.Amazon.Abstractions;
using Despesas.WebApi.CommonDependenceInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CommonDependenceInject;

public sealed class AmazonS3BucketDependenceInjectTest
{
    [Fact]
    public void AddAmazonS3BucketConfigurations_Should_Register_Required_Services()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            { "AmazonS3Configurations:AccessKey", "fake-access-key" },
            { "AmazonS3Configurations:SecretKey", "fake-secret-key" },
            { "AmazonS3Configurations:BucketName", "fake-bucket" }        
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddAmazonS3BucketConfigurations(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<IOptions<AmazonS3Options>>();
        Assert.NotNull(options);
        Assert.Equal("fake-access-key", options.Value.AccessKey);
        Assert.Equal("fake-secret-key", options.Value.SecretAccessKey);
        Assert.Equal("fake-bucket", options.Value.BucketName);
        
        var amazonS3Bucket = serviceProvider.GetService<IAmazonS3Bucket>();
        Assert.NotNull(amazonS3Bucket);
        Assert.IsType<AmazonS3Bucket>(amazonS3Bucket);
    }
}
