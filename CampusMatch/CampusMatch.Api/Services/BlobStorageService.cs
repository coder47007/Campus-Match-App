using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CampusMatch.Api.Services;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteAsync(string blobName);
    string GetBlobUrl(string blobName);
}

public class AzureBlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly string _containerUrl;
    
    public AzureBlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["Azure:StorageConnectionString"] 
            ?? "UseDevelopmentStorage=true";  // Azurite for local dev
        var containerName = configuration["Azure:ContainerName"] ?? "photos";
        
        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        
        _containerUrl = _containerClient.Uri.ToString();
    }
    
    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        var blobName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var blobClient = _containerClient.GetBlobClient(blobName);
        
        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders 
        { 
            ContentType = contentType 
        });
        
        return blobName;
    }
    
    public async Task DeleteAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }
    
    public string GetBlobUrl(string blobName)
    {
        return $"{_containerUrl}/{blobName}";
    }
}

// Fallback local storage for development without Azure
public class LocalFileStorageService : IBlobStorageService
{
    private readonly string _uploadPath;
    private readonly string _baseUrl;
    
    public LocalFileStorageService(IWebHostEnvironment env, IConfiguration configuration)
    {
        _uploadPath = Path.Combine(env.ContentRootPath, "uploads");
        _baseUrl = configuration["App:BaseUrl"] ?? "http://localhost:5229";
        
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }
    
    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        var blobName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var filePath = Path.Combine(_uploadPath, blobName);
        
        using var fs = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fs);
        
        return blobName;
    }
    
    public Task DeleteAsync(string blobName)
    {
        var filePath = Path.Combine(_uploadPath, blobName);
        if (File.Exists(filePath))
            File.Delete(filePath);
        return Task.CompletedTask;
    }
    
    public string GetBlobUrl(string blobName)
    {
        return $"{_baseUrl}/uploads/{blobName}";
    }
}
