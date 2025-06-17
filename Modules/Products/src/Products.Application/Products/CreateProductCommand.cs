using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Products.Application.Common.Configurations;
using Products.Application.Common.Interfaces;
using Products.Domain.Entities;
using Shared.Common.Abstraction;
using Shared.Common.Authentication;
using Shared.Common.Exceptions;
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

public class CreateProductCommandHandler(
    IProductDbContext dbContext,
    IOptions<ProductsSettings> productsOptions,
    IOptions<AwsSettings> awsOptions,
    IStorageService storageService,
    IOptions<WebClientSettings> webClientOptions,
    IProductTokenService productTokenService,
    IValidator<CreateProductCommand> validator) : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly ProductsSettings _productsSettings = productsOptions.Value;
    private readonly AwsSettings _awsSettings = awsOptions.Value;
    private readonly WebClientSettings _webClientOptions = webClientOptions.Value;

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new RequestValidationException(validationResult.Errors);
        }

        var productId = Guid.NewGuid();

        // Upload the image if provided
        string? imgUrl = null;
        
        if (request.ImageStream != null && request.ImageStream.Length > 0)
        {
            await storageService.UploadFileAsync(
                bucketName: $"{_productsSettings.ImageBucketName}", // Replace with your bucket name
                key: $"products/{request.ImageName}", // Define the key (path) for the file
                fileStream: request.ImageStream
            );
            // Construct the image URL (optional, depends on your S3 setup)
            imgUrl = $"https://{_productsSettings.ImageBucketName}.s3.{_awsSettings.Region}.amazonaws.com/products/{request.ImageName}";
        }

        // Generate a token for the product which will be used for booking URL
        var expiry = request.StartDate.ToDateTime(request.StartTime);
        var token = productTokenService.GenerateProductToken(productId, expiry);
        var registrationUrl = $"{_webClientOptions.BaseUrl}?et={token}";

        var product = new Product {
            Id = productId,
            CustomerId = request.CustomerId,
            Code = request.Code, 
            Name = request.Name, 
            Description = request.Description,
            ImageUrl = imgUrl ?? "",
            Address = request.Address,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            RegistrationUrl = registrationUrl
        };
        dbContext.Add(product);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            return Result.Failure<Guid>(new Error("Product.CodeExists", "A product with the same code already exists."));
        }
        
        return product.Id;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        if (ex.InnerException.Message.Contains("uq_products_customer_id_code", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        //if (ex.InnerException is PostgresException pgEx)
        //{
        //    return pgEx.SqlState == "23505";
        //}
        return false;
    }
}