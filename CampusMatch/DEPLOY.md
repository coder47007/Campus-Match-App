# Backend Deployment Guide

## Deploy to Railway (Free Tier)

### Option 1: One-Click Deploy
1. Go to [railway.app](https://railway.app) and create an account
2. Click "New Project" → "Deploy from GitHub"
3. Connect your GitHub account and select this repository
4. Railway will auto-detect the Dockerfile and deploy

### Option 2: Railway CLI
```bash
# Install Railway CLI
npm install -g @railway/cli

# Login to Railway
railway login

# Initialize project (inside CampusMatch folder)
cd CampusMatch
railway init

# Deploy
railway up
```

### Environment Variables
After deployment, add these environment variables in Railway dashboard:
- `ConnectionStrings__DefaultConnection`: Your Supabase PostgreSQL connection string
- `Jwt__Key`: Your JWT secret key
- `Supabase__Url`: Your Supabase URL
- `Supabase__Key`: Your Supabase key
- `Supabase__BucketName`: photos

### After Deployment
1. Railway will give you a URL like: `https://campusmatch-production.up.railway.app`
2. Update the mobile app's API URL in `CampusMatchMobile/app.config.js`:
   ```javascript
   extra: {
       apiUrl: "https://your-railway-url.up.railway.app",
       // ...
   }
   ```
3. Update `eas.json` with the same URL
4. Rebuild the APK with: `npx eas-cli build --platform android --profile preview`

## Alternative: Render.com (Free)
1. Go to [render.com](https://render.com)
2. Create new Web Service → Connect GitHub
3. Set Build Command: `docker`
4. Set Start Command: (leave default)

## Your Supabase Values
These are already configured in appsettings.json:
- Database: Already connected via connection string
- Storage: photos bucket for images
