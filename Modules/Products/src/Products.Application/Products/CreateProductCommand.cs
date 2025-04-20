using MediatR;
using Microsoft.Extensions.Options;
using Products.Application.Common.Configurations;
using Products.Application.Common.Interfaces;
using Products.Domain.Entities;
using Shared.Common.Abstraction;
using Shared.Common.Authentication;
using Shared.Common.Storage;

namespace Products.Application.Products;

public sealed record CreateProductCommand(
    Guid CustomerId,
    string Code,
    string Name,
    string Description,
    Stream ImageStream,
    string ImageName,
    string Address,
    DateOnly StartDate,
    DateOnly EndDate,
    TimeOnly StartTime,
    TimeOnly EndTime) : IRequest<Result<Guid>>;

public class CreateProductCommandHandler(IProductDbContext dbContext,
    IOptions<ProductsSettings> productsOptions,
    IOptions<AwsSettings> awsOptions,
    IStorageService storageService) : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly ProductsSettings _productsSettings = productsOptions.Value;
    private readonly AwsSettings _awsSettings = awsOptions.Value;

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var productId = Guid.NewGuid();
        // Upload the image if provided
        string? imgUrl = null;
        
        if (request.ImageStream != null)
        {
            await storageService.UploadFileAsync(
                bucketName: $"{_productsSettings.ImageBucketName}", // Replace with your bucket name
                key: $"products/{request.ImageName}", // Define the key (path) for the file
                fileStream: request.ImageStream
            );
            // Construct the image URL (optional, depends on your S3 setup)
            imgUrl = $"https://{_productsSettings.ImageBucketName}.s3.{_awsSettings.Region}.amazonaws.com/products/{request.ImageName}";
        }

        var product = new Product {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Code = request.Code, 
            Name = request.Name, 
            Description = request.Description,
            ImageUrl = imgUrl,
            Address = request.Address,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };
        dbContext.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return product.Id;
    }
}

