using Infrastructure.Amazon;
using Infrastructure.Amazon.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CommonDependenceInject;

public sealed class AmazonS3BucketDependenceInjectTest
{
    [Fact]
    public void Should_Register_AmazonS3Bucket_With_Manually_Created_Options()
    {
        // Arrange
        var options = Options.Create(new AmazonS3Options
        {
            AccessKey = "fake-access-key",
            SecretAccessKey = "fake-secret-key",
            BucketName = "fake-bucket",
            S3ServiceUrl = "fake-url"
        });

        var services = new ServiceCollection();
        services.AddSingleton<IOptions<AmazonS3Options>>(options);
        services.AddSingleton<IAmazonS3Bucket, AmazonS3Bucket>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var resolvedOptions = serviceProvider.GetService<IOptions<AmazonS3Options>>();
        var amazonS3Bucket = serviceProvider.GetService<IAmazonS3Bucket>();

        // Assert
        Assert.NotNull(resolvedOptions);
        Assert.Equal("fake-access-key", resolvedOptions.Value.AccessKey);
        Assert.Equal("fake-secret-key", resolvedOptions.Value.SecretAccessKey);
        Assert.Equal("fake-bucket", resolvedOptions.Value.BucketName);
        Assert.Equal("fake-url", resolvedOptions.Value.S3ServiceUrl);

        Assert.NotNull(amazonS3Bucket);
        Assert.IsType<AmazonS3Bucket>(amazonS3Bucket);
    }
}
