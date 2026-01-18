using System.Net.Http.Headers;

namespace CampusMatch.Api.Services;

public class SupabaseStorageService : IBlobStorageService
{
    private readonly HttpClient _httpClient;
    private readonly string _bucketName;
    private readonly string _projectUrl;
    private readonly string _serviceKey;

    public SupabaseStorageService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("SupabaseStorage");
        _bucketName = configuration["Supabase:BucketName"] ?? "photos";
        _projectUrl = configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase URL is required");
        _serviceKey = configuration["Supabase:Key"] ?? throw new InvalidOperationException("Supabase Key is required");
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        var blobName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        
        // Supabase Storage API: POST /storage/v1/object/{bucket}/{path}
        var url = $"{_projectUrl}/storage/v1/object/{_bucketName}/{blobName}";
        
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _serviceKey);
        // Also need API key in header for some Supabase configs? usually Bearer is enough for service_role/anon
        // But the docs say ApiKey header is also good practice.
        request.Headers.Add("apikey", _serviceKey); 

        using var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to upload to Supabase: {response.StatusCode} - {error}");
        }

        return blobName;
    }

    public async Task DeleteAsync(string blobName)
    {
        // DELETE /storage/v1/object/{bucket}/{path}
        var url = $"{_projectUrl}/storage/v1/object/{_bucketName}/{blobName}";
        
        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _serviceKey);
        request.Headers.Add("apikey", _serviceKey);

        var response = await _httpClient.SendAsync(request);
        // We can ignore 404s
    }

    public string GetBlobUrl(string blobName)
    {
        // Public URL: {projectUrl}/storage/v1/object/public/{bucket}/{path}
        return $"{_projectUrl}/storage/v1/object/public/{_bucketName}/{blobName}";
    }
}
