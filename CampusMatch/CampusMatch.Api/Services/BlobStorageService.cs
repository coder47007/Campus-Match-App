namespace CampusMatch.Api.Services;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteAsync(string blobName);
    string GetBlobUrl(string blobName);
}

// NOTE: AzureBlobStorageService removed - not used in production
// Production uses SupabaseStorageService, Development uses LocalFileStorageService


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
