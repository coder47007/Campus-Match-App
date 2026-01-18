# CampusMatch Production Deployment Guide

This comprehensive guide covers all steps required to deploy CampusMatch to a production environment.

---

## 📋 Table of Contents

1. [Pre-Deployment Checklist](#pre-deployment-checklist)
2. [Code Changes Required](#code-changes-required)
3. [Infrastructure Setup](#infrastructure-setup)
4. [Database Configuration](#database-configuration)
5. [Azure Blob Storage Setup](#azure-blob-storage-setup)
6. [Email Service Configuration](#email-service-configuration)
7. [JWT Security Configuration](#jwt-security-configuration)
8. [Client Application Updates](#client-application-updates)
9. [Environment Configuration](#environment-configuration)
10. [Deployment Options](#deployment-options)
11. [Post-Deployment Verification](#post-deployment-verification)
12. [Monitoring & Logging](#monitoring--logging)
13. [Security Hardening](#security-hardening)
14. [Backup & Recovery](#backup--recovery)

---

## ⚠️ Pre-Deployment Checklist

Before deploying to production, ensure ALL of the following are completed:

- [ ] Remove database reset code from `Program.cs`
- [ ] Generate and configure secure JWT key (256-bit minimum)
- [ ] Set up production SQL Server database
- [ ] Configure Azure Blob Storage for photo uploads
- [ ] Configure SMTP email service
- [ ] Update all hardcoded URLs in client application
- [ ] Configure HTTPS/TLS certificates
- [ ] Set up monitoring and logging
- [ ] Configure rate limiting for production traffic
- [ ] Test all features in staging environment
- [ ] Create database backup strategy
- [ ] Document rollback procedures

---

## 🔧 Code Changes Required

### 1. Remove Database Reset (CRITICAL)

**File:** `CampusMatch.Api/Program.cs`

**Current code (lines 118-124):**
```csharp
// Auto-migrate and seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampusMatchDbContext>();
    db.Database.EnsureDeleted();  // ⚠️ REMOVE THIS LINE
    db.Database.EnsureCreated();
}
```

**Production code:**
```csharp
// Auto-migrate database (production-safe)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampusMatchDbContext>();
    db.Database.Migrate();  // Use migrations instead
}
```

> **CRITICAL:** The `EnsureDeleted()` call will **DELETE ALL DATA** on every restart. This MUST be removed for production.

---

### 2. Update Seed Data Strategy

**File:** `CampusMatch.Api/Data/CampusMatchDbContext.cs`

For production, you have two options:

**Option A: Remove seed data entirely**
- Delete the `SeedData()` method and its call in `OnModelCreating()`
- Create an admin seeding script to run once

**Option B: Make seeding conditional**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // ... entity configurations ...
    
    // Only seed in development
    #if DEBUG
    SeedData(modelBuilder);
    #endif
}
```

---

### 3. Create Database Migrations

Instead of `EnsureCreated()`, use proper migrations:

```powershell
# Navigate to API project
cd CampusMatch.Api

# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migrations (run on deployment)
dotnet ef database update
```

---

## 🏗️ Infrastructure Setup

### Azure App Service (Recommended)

1. **Create Azure Resources:**
   ```powershell
   # Create resource group
   az group create --name CampusMatch-RG --location eastus
   
   # Create App Service Plan
   az appservice plan create --name CampusMatch-Plan --resource-group CampusMatch-RG --sku B1 --is-linux
   
   # Create Web App
   az webapp create --name campusmatch-api --resource-group CampusMatch-RG --plan CampusMatch-Plan --runtime "DOTNET|8.0"
   ```

2. **Configure Deployment:**
   - Enable continuous deployment from GitHub/Azure DevOps
   - Or use ZIP deploy: `az webapp deployment source config-zip`

### Alternative: Docker Deployment

**Dockerfile for API:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CampusMatch.Api/CampusMatch.Api.csproj", "CampusMatch.Api/"]
COPY ["CampusMatch.Shared/CampusMatch.Shared.csproj", "CampusMatch.Shared/"]
RUN dotnet restore "CampusMatch.Api/CampusMatch.Api.csproj"
COPY . .
WORKDIR "/src/CampusMatch.Api"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CampusMatch.Api.dll"]
```

---

## 🗄️ Database Configuration

### Azure SQL Database Setup

1. **Create Azure SQL Server:**
   ```powershell
   az sql server create --name campusmatch-sql --resource-group CampusMatch-RG --location eastus --admin-user sqladmin --admin-password <SecurePassword123!>
   
   az sql db create --resource-group CampusMatch-RG --server campusmatch-sql --name CampusMatchDb --service-objective S0
   ```

2. **Configure Firewall:**
   ```powershell
   # Allow Azure services
   az sql server firewall-rule create --resource-group CampusMatch-RG --server campusmatch-sql --name AllowAzure --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
   ```

3. **Connection String Format:**
   ```
   Server=tcp:campusmatch-sql.database.windows.net,1433;Initial Catalog=CampusMatchDb;Persist Security Info=False;User ID=sqladmin;Password={your_password};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```

### On-Premises SQL Server

**Connection string for SQL Server:**
```
Server=your-server.domain.com;Database=CampusMatchDb;User Id=campusmatch_user;Password={password};MultipleActiveResultSets=True;TrustServerCertificate=True;
```

**Required SQL permissions:**
```sql
CREATE LOGIN campusmatch_user WITH PASSWORD = 'SecurePassword123!';
CREATE USER campusmatch_user FOR LOGIN campusmatch_user;
ALTER ROLE db_datareader ADD MEMBER campusmatch_user;
ALTER ROLE db_datawriter ADD MEMBER campusmatch_user;
GRANT EXECUTE TO campusmatch_user;
```

---

## 📦 Azure Blob Storage Setup

### 1. Create Storage Account

```powershell
# Create storage account
az storage account create --name campusmatchstorage --resource-group CampusMatch-RG --location eastus --sku Standard_LRS

# Get connection string
az storage account show-connection-string --name campusmatchstorage --resource-group CampusMatch-RG
```

### 2. Create Container

```powershell
az storage container create --name photos --account-name campusmatchstorage --public-access blob
```

### 3. Configuration Values

Add to `appsettings.Production.json`:
```json
{
  "Azure": {
    "StorageConnectionString": "DefaultEndpointsProtocol=https;AccountName=campusmatchstorage;AccountKey=<your-key>;EndpointSuffix=core.windows.net",
    "ContainerName": "photos"
  }
}
```

### 4. CORS Configuration (if needed)

```powershell
az storage cors add --services b --methods GET HEAD OPTIONS --origins "*" --allowed-headers "*" --exposed-headers "*" --max-age 3600 --account-name campusmatchstorage
```

---

## 📧 Email Service Configuration

### Option 1: SendGrid (Recommended)

1. **Create SendGrid Account** at https://sendgrid.com

2. **Get API Key** from SendGrid dashboard

3. **Update EmailService.cs** to use SendGrid:
   ```csharp
   // Install package: SendGrid
   using SendGrid;
   using SendGrid.Helpers.Mail;
   
   public class SendGridEmailService : IEmailService
   {
       private readonly string _apiKey;
       
       public SendGridEmailService(IConfiguration config)
       {
           _apiKey = config["SendGrid:ApiKey"];
       }
       
       public async Task SendVerificationEmailAsync(string email, string token)
       {
           var client = new SendGridClient(_apiKey);
           var msg = new SendGridMessage
           {
               From = new EmailAddress("noreply@campusmatch.edu", "CampusMatch"),
               Subject = "Verify your CampusMatch email",
               HtmlContent = $"<a href='https://api.campusmatch.edu/api/auth/verify?token={token}'>Verify Email</a>"
           };
           msg.AddTo(email);
           await client.SendEmailAsync(msg);
       }
   }
   ```

4. **Configuration:**
   ```json
   {
     "SendGrid": {
       "ApiKey": "SG.xxxxxxxxxx"
     }
   }
   ```

### Option 2: SMTP Server

```json
{
  "Email": {
    "SmtpHost": "smtp.office365.com",
    "SmtpPort": "587",
    "SmtpUser": "noreply@yourdomain.com",
    "SmtpPass": "your-password",
    "FromEmail": "noreply@yourdomain.com",
    "FromName": "CampusMatch"
  }
}
```

### Option 3: Amazon SES

```csharp
// Install package: AWSSDK.SimpleEmail
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

public class SesEmailService : IEmailService
{
    private readonly IAmazonSimpleEmailService _ses;
    
    public SesEmailService()
    {
        _ses = new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.USEast1);
    }
    
    // Implementation...
}
```

---

## 🔐 JWT Security Configuration

### Generate Secure Key

```powershell
# PowerShell - Generate 256-bit key
$bytes = New-Object byte[] 32
(New-Object Security.Cryptography.RNGCryptoServiceProvider).GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

Or in C#:
```csharp
using var rng = RandomNumberGenerator.Create();
var key = new byte[32];
rng.GetBytes(key);
Console.WriteLine(Convert.ToBase64String(key));
```

### Production JWT Configuration

```json
{
  "Jwt": {
    "Key": "BASE64_ENCODED_256_BIT_KEY_HERE",
    "Issuer": "https://api.campusmatch.edu",
    "Audience": "https://campusmatch.edu"
  }
}
```

### Token Expiry Considerations

**Current:** 7 days (hardcoded in `JwtService.cs`)

**Recommended for production:**
- Access tokens: 15-60 minutes
- Refresh tokens: 7-30 days

**Update JwtService.cs:**
```csharp
public string GenerateToken(Student student)
{
    // ... existing code ...
    
    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(60), // Changed from 7 days
        signingCredentials: credentials
    );
    
    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

---

## 📱 Client Application Updates

### Update Hardcoded URLs

**File:** `CampusMatch.Client/Services/ApiService.cs` (Line 28)

```csharp
// Current (development)
_http = new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5229"),
    Timeout = TimeSpan.FromSeconds(30)
};

// Production - Option 1: Hardcode production URL
_http = new HttpClient 
{ 
    BaseAddress = new Uri("https://api.campusmatch.edu"),
    Timeout = TimeSpan.FromSeconds(30)
};

// Production - Option 2: Use configuration file
public ApiService(IConfiguration config)
{
    var baseUrl = config["ApiBaseUrl"] ?? "https://api.campusmatch.edu";
    _http = new HttpClient 
    { 
        BaseAddress = new Uri(baseUrl),
        Timeout = TimeSpan.FromSeconds(30)
    };
}
```

**File:** `CampusMatch.Client/Services/SignalRService.cs` (Line 19)

```csharp
// Current
.WithUrl("http://localhost:5229/chathub", options =>

// Production
.WithUrl("https://api.campusmatch.edu/chathub", options =>
```

### Recommended: Use App Settings

Create `CampusMatch.Client/appsettings.json`:
```json
{
  "ApiBaseUrl": "https://api.campusmatch.edu"
}
```

Update `Program.cs` to load configuration:
```csharp
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

var api = new ApiService(config);
```

---

## ⚙️ Environment Configuration

### appsettings.Production.json

Create this file for production-specific settings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "api.campusmatch.edu",
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:campusmatch-sql.database.windows.net,1433;Initial Catalog=CampusMatchDb;..."
  },
  "Jwt": {
    "Key": "YOUR_PRODUCTION_256_BIT_KEY",
    "Issuer": "https://api.campusmatch.edu",
    "Audience": "https://campusmatch.edu"
  },
  "Azure": {
    "StorageConnectionString": "DefaultEndpointsProtocol=https;AccountName=...",
    "ContainerName": "photos"
  },
  "Email": {
    "SmtpHost": "smtp.sendgrid.net",
    "SmtpPort": "587",
    "SmtpUser": "apikey",
    "SmtpPass": "SG.xxxxx",
    "FromEmail": "noreply@campusmatch.edu",
    "FromName": "CampusMatch"
  },
  "App": {
    "BaseUrl": "https://api.campusmatch.edu"
  }
}
```

### Environment Variables (Azure App Service)

Set these in Azure Portal → App Service → Configuration:

| Name | Value |
|------|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__DefaultConnection` | `Server=tcp:...` |
| `Jwt__Key` | `your-256-bit-key` |
| `Azure__StorageConnectionString` | `DefaultEndpointsProtocol=...` |
| `Email__SmtpPass` | `your-smtp-password` |

---

## 🚀 Deployment Options

### Option 1: Azure App Service (Recommended)

```powershell
# Build and publish
dotnet publish -c Release -o ./publish

# Deploy via ZIP
az webapp deployment source config-zip --resource-group CampusMatch-RG --name campusmatch-api --src ./publish.zip
```

### Option 2: Docker + Azure Container Apps

```powershell
# Build image
docker build -t campusmatch-api .

# Push to Azure Container Registry
az acr login --name campusmatchacr
docker tag campusmatch-api campusmatchacr.azurecr.io/campusmatch-api:v1
docker push campusmatchacr.azurecr.io/campusmatch-api:v1

# Deploy to Container Apps
az containerapp create --name campusmatch-api --resource-group CampusMatch-RG --image campusmatchacr.azurecr.io/campusmatch-api:v1
```

### Option 3: IIS (Windows Server)

1. Install .NET 8 Hosting Bundle
2. Create IIS site pointing to publish folder
3. Configure Application Pool (No Managed Code)
4. Set environment variables in web.config

---

## ✅ Post-Deployment Verification

### Health Check Endpoints

Add to `Program.cs`:
```csharp
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
app.MapGet("/health/db", async (CampusMatchDbContext db) =>
{
    try
    {
        await db.Database.CanConnectAsync();
        return Results.Ok(new { status = "healthy", database = "connected" });
    }
    catch (Exception ex)
    {
        return Results.StatusCode(503, new { status = "unhealthy", error = ex.Message });
    }
});
```

### Verification Checklist

1. **API Health:**
   ```powershell
   curl https://api.campusmatch.edu/health
   ```

2. **Database Connectivity:**
   ```powershell
   curl https://api.campusmatch.edu/health/db
   ```

3. **Authentication Flow:**
   - Register new account
   - Login and receive JWT
   - Access protected endpoints

4. **Photo Upload:**
   - Upload photo via `/api/photos/upload`
   - Verify photo appears in Azure Blob Storage
   - Verify photo URL is accessible

5. **Real-time Chat:**
   - Connect two clients
   - Create match
   - Send messages and verify real-time delivery

6. **Email Delivery:**
   - Register new account
   - Check email inbox for verification email

---

## 📊 Monitoring & Logging

### Azure Application Insights

1. **Add package:**
   ```powershell
   dotnet add package Microsoft.ApplicationInsights.AspNetCore
   ```

2. **Configure in Program.cs:**
   ```csharp
   builder.Services.AddApplicationInsightsTelemetry();
   ```

3. **Add to appsettings.Production.json:**
   ```json
   {
     "ApplicationInsights": {
       "ConnectionString": "InstrumentationKey=xxx;IngestionEndpoint=https://..."
     }
   }
   ```

### Structured Logging with Serilog

1. **Add packages:**
   ```powershell
   dotnet add package Serilog.AspNetCore
   dotnet add package Serilog.Sinks.Console
   dotnet add package Serilog.Sinks.File
   ```

2. **Configure in Program.cs:**
   ```csharp
   Log.Logger = new LoggerConfiguration()
       .MinimumLevel.Warning()
       .WriteTo.Console()
       .WriteTo.File("logs/campusmatch-.log", rollingInterval: RollingInterval.Day)
       .CreateLogger();
   
   builder.Host.UseSerilog();
   ```

---

## 🛡️ Security Hardening

### 1. HTTPS Enforcement

```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();
```

### 2. Security Headers

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
    await next();
});
```

### 3. CORS Configuration (Production)

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://campusmatch.edu", "https://www.campusmatch.edu")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### 4. Rate Limiting Adjustments

Consider adjusting rate limits for production traffic:

```csharp
options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        factory: partition => new FixedWindowRateLimiterOptions
        {
            AutoReplenishment = true,
            PermitLimit = 1000,  // Increased for production
            Window = TimeSpan.FromMinutes(1)
        }));
```

---

## 💾 Backup & Recovery

### Database Backup Strategy

**Azure SQL:**
```powershell
# Configure long-term retention
az sql db ltr-policy set --resource-group CampusMatch-RG --server campusmatch-sql --database CampusMatchDb --weekly-retention P4W --monthly-retention P12M
```

**On-Premises:**
```sql
-- Daily backup script
BACKUP DATABASE CampusMatchDb 
TO DISK = 'D:\Backups\CampusMatchDb_YYYYMMDD.bak'
WITH COMPRESSION, INIT;
```

### Blob Storage Backup

Enable soft delete and versioning:
```powershell
az storage blob service-properties delete-policy update --account-name campusmatchstorage --enable true --days-retained 30
```

### Disaster Recovery Checklist

1. **Database Recovery:**
   - Point-in-time restore (Azure SQL)
   - Restore from backup file (on-premises)

2. **Application Recovery:**
   - Keep previous deployment package
   - Document quick rollback procedure

3. **Configuration Recovery:**
   - Store secrets in Azure Key Vault
   - Document all environment variables

---

## 📞 Support Contacts

| Issue | Contact |
|-------|---------|
| Azure Issues | Azure Support Portal |
| Database Problems | DBA Team |
| Application Bugs | Development Team |
| Security Incidents | Security Team |

---

## 📝 Deployment Log Template

```
Deployment Date: YYYY-MM-DD HH:MM
Deployer: Name
Version: v1.x.x
Environment: Production

Pre-deployment Checks:
[ ] Database backup completed
[ ] Previous version archived
[ ] Team notified

Deployment Steps:
1. [ ] Code deployed
2. [ ] Database migrations applied
3. [ ] Configuration verified
4. [ ] Health checks passed
5. [ ] Smoke tests completed

Post-deployment:
[ ] Monitoring verified
[ ] No error spikes
[ ] User-facing features tested

Notes:
- 
```
