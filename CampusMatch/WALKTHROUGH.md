# CampusMatch Project Setup, Run & Test Walkthrough

This document provides a complete guide for setting up, running, and testing the CampusMatch application—a Tinder-style student matching platform.

---

## 📁 Project Structure

```
CampusMatch/
├── CampusMatch.Api/          # ASP.NET Core 8 Web API (backend)
├── CampusMatch.Client/       # Windows Forms application (.NET 10)
├── CampusMatch.Shared/       # Shared DTOs library (.NET 8)
└── CampusMatch.sln           # Solution file
```

| Project | Framework | Description |
|---------|-----------|-------------|
| **CampusMatch.Api** | .NET 8 | REST API with JWT auth, SignalR chat, SQL Server database |
| **CampusMatch.Client** | .NET 10 (WinForms) | Desktop client with login, discover, matches, chat, profile |
| **CampusMatch.Shared** | .NET 8 | DTOs shared between API and Client |

---

## 📋 Prerequisites

Before running the project, ensure you have the following installed:

| Requirement | Version | Download |
|-------------|---------|----------|
| **.NET SDK** | 8.0+ (API/Shared), 10.0 (Client) | [Download .NET](https://dotnet.microsoft.com/download) |
| **Visual Studio 2022** | 17.8+ | [Download VS](https://visualstudio.microsoft.com/) |
| **SQL Server LocalDB** | Included with VS | Comes with Visual Studio |

> **Note:** SQL Server LocalDB is automatically installed with Visual Studio's "Data storage and processing" workload.

---

## 🔧 Configuration Reference

### Environment Variables & Configuration Files

All configuration is stored in `appsettings.json` and hardcoded values in source files.

---

### API Configuration (`CampusMatch.Api/appsettings.json`)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CampusMatchDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "CampusMatchSecretKey2024VeryLongAndSecure!@#$",
    "Issuer": "CampusMatch",
    "Audience": "CampusMatchUsers"
  }
}
```

---

### 🔑 All Configuration Values & Hardcoded Items

#### Database Connection
| Setting | Location | Current Value |
|---------|----------|---------------|
| `ConnectionStrings:DefaultConnection` | `appsettings.json` | `Server=(localdb)\\mssqllocaldb;Database=CampusMatchDb;Trusted_Connection=True;MultipleActiveResultSets=true` |

#### JWT Authentication
| Setting | Location | Current Value |
|---------|----------|---------------|
| `Jwt:Key` | `appsettings.json` | `CampusMatchSecretKey2024VeryLongAndSecure!@#$` |
| `Jwt:Issuer` | `appsettings.json` | `CampusMatch` |
| `Jwt:Audience` | `appsettings.json` | `CampusMatchUsers` |
| Token Expiry | `Services/JwtService.cs:40` | `7 days` (hardcoded) |

#### API URLs & Endpoints
| Setting | Location | Current Value |
|---------|----------|---------------|
| API Base URL | `Properties/launchSettings.json` | `http://localhost:5229` |
| HTTPS URL | `Properties/launchSettings.json` | `https://localhost:7190` |
| Client API URL | `CampusMatch.Client/Services/ApiService.cs:28` | `http://localhost:5229` (hardcoded) |
| Client SignalR URL | `CampusMatch.Client/Services/SignalRService.cs:19` | `http://localhost:5229/chathub` (hardcoded) |
| App Base URL (fallback) | `Services/BlobStorageService.cs:65` | `http://localhost:5229` (fallback) |

#### Rate Limiting (Hardcoded in `Program.cs`)
| Limiter | Limit | Window | Location |
|---------|-------|--------|----------|
| Global (per IP) | 100 requests | 1 minute | Lines 53-61 |
| Auth endpoints | 5 requests | 1 minute | Lines 64-69 |
| Swipe endpoints | 50 requests | 1 minute | Lines 72-77 |

#### HTTP Client Settings (Hardcoded in `CampusMatch.Client/Services/ApiService.cs`)
| Setting | Value | Location |
|---------|-------|----------|
| Request Timeout | 30 seconds | Line 29 |
| Max Retries | 3 | Line 14 |
| Retry Delay | 1000 ms | Line 15 |

---

### 🌐 Production-Only Configuration (Azure & Email)

These settings are **only used in production** (when `ASPNETCORE_ENVIRONMENT != Development`):

#### Azure Blob Storage
| Setting | Config Key | Default Value |
|---------|------------|---------------|
| Storage Connection String | `Azure:StorageConnectionString` | `UseDevelopmentStorage=true` (Azurite) |
| Container Name | `Azure:ContainerName` | `photos` |

#### SMTP Email Service
| Setting | Config Key | Default Value |
|---------|------------|---------------|
| SMTP Host | `Email:SmtpHost` | `localhost` |
| SMTP Port | `Email:SmtpPort` | `25` |
| SMTP User | `Email:SmtpUser` | *(none)* |
| SMTP Password | `Email:SmtpPass` | *(none)* |
| From Email | `Email:FromEmail` | `noreply@campusmatch.edu` |
| From Name | `Email:FromName` | `CampusMatch` |

> **Tip:** In development mode, emails are **logged to console** instead of being sent. Check the terminal output for verification tokens.

---

### 🧪 Seed Data (Test Accounts)

The database is seeded with test accounts upon startup. All accounts have the same password:

| Email | Password | Name | Role |
|-------|----------|------|------|
| `emma@university.edu` | `password123` | Emma Wilson | Female Student |
| `james@university.edu` | `password123` | James Chen | Male Student |
| `sofia@university.edu` | `password123` | Sofia Rodriguez | Female Student |
| `alex@university.edu` | `password123` | Alex Thompson | Male Student |
| `olivia@university.edu` | `password123` | Olivia Park | Female Student |
| `marcus@university.edu` | `password123` | Marcus Johnson | Male Student |

> **Warning:** The database is **reset on every API startup** (`db.Database.EnsureDeleted()` in `Program.cs:122`). This is for development only!

---

## 🚀 Setup & Run Instructions

### Step 1: Clone & Open Solution

```powershell
# Navigate to project directory
cd c:\Users\salim\OneDrive\Desktop\RAD\CampusMatch

# Open in Visual Studio
start CampusMatch.sln
```

### Step 2: Restore NuGet Packages

```powershell
dotnet restore
```

### Step 3: Build the Solution

```powershell
dotnet build
```

### Step 4: Run the API Server

```powershell
cd CampusMatch.Api
dotnet run
```

The API will start at:
- **HTTP**: `http://localhost:5229`
- **Swagger UI**: `http://localhost:5229/swagger`

> **Important:** Keep the API running in a separate terminal while running the client.

### Step 5: Run the Client Application

In a **new terminal**:

```powershell
cd CampusMatch.Client
dotnet run
```

Or run from Visual Studio by setting **CampusMatch.Client** as the startup project.

---

## 🧪 Testing the Application

### Manual Testing Flow

1. **Launch API** → Verify Swagger loads at `http://localhost:5229/swagger`
2. **Launch Client** → Login form appears
3. **Login** with seed account: `emma@university.edu` / `password123`
4. **Discover** → Browse other student profiles
5. **Swipe** → Like/dislike students
6. **Matches** → View mutual likes
7. **Chat** → Send messages to matches (real-time via SignalR)
8. **Profile** → Edit bio, photos, interests

### API Testing via Swagger

1. Navigate to `http://localhost:5229/swagger`
2. Test endpoints directly:
   - **POST** `/api/auth/login` - Get JWT token
   - **GET** `/api/profiles/discover` - Browse profiles (requires auth)
   - **POST** `/api/swipes` - Create swipe

### Testing SignalR (Real-time Chat)

1. Login with two different accounts in separate client instances
2. Match both accounts by swiping right on each other
3. Open chat → Messages should appear instantly without refresh

---

## 📊 API Endpoints Reference

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/register` | Create new account | ❌ |
| POST | `/api/auth/login` | Login & get JWT | ❌ |
| POST | `/api/auth/forgot-password` | Request password reset | ❌ |
| GET | `/api/profiles/me` | Get current user profile | ✅ |
| PUT | `/api/profiles/me` | Update profile | ✅ |
| GET | `/api/profiles/discover` | Get swipable profiles | ✅ |
| GET | `/api/profiles/interests` | List all interests | ✅ |
| POST | `/api/swipes` | Create swipe | ✅ |
| GET | `/api/matches` | Get all matches | ✅ |
| DELETE | `/api/matches/{id}` | Unmatch | ✅ |
| GET | `/api/messages/{matchId}` | Get chat messages | ✅ |
| POST | `/api/messages` | Send message | ✅ |
| POST | `/api/photos/upload` | Upload photo | ✅ |
| DELETE | `/api/photos/{id}` | Delete photo | ✅ |
| POST | `/api/reports` | Report user | ✅ |
| POST | `/api/reports/block` | Block user | ✅ |

---

## 🔧 Troubleshooting

### Common Issues

| Issue | Solution |
|-------|----------|
| **"Connection refused"** | Ensure API is running on port 5229 |
| **"401 Unauthorized"** | JWT token expired or missing - re-login |
| **"Database error"** | LocalDB not installed or service not running |
| **Client can't connect** | Check hardcoded URL in `ApiService.cs` matches API |

### Checking LocalDB Status

```powershell
sqllocaldb info MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

### Viewing Development Email Logs

When running in Development mode, check the API console output for email tokens:

```
DEV EMAIL: Verification for user@email.com, Token: abc123...
```

---

## 📝 Production Deployment

For detailed production deployment instructions, see [PRODUCTION_DEPLOYMENT.md](PRODUCTION_DEPLOYMENT.md).
