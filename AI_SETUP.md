# AI Integration Setup Instructions

## GitHub Models Configuration

This application uses GitHub Models for AI-powered obituary description generation.

### Prerequisites

1. **GitHub Personal Access Token** - Required for API authentication (linked to github account used)
2. **Model Access** - Ensure the GitHub account (we are using our own account) has access to GitHub Models

### Environment Setup

The application loads secrets from a `.env` file in the project root (must be created to run application & stays locally only). **This file is already gitignored.**

#### For Local Development:

The `.env` file is already created with your token. No additional setup required.

#### For Azure Deployment:

Add these environment variables to your Azure App Service:

```
GITHUB_TOKEN=your_github_personal_access_token_here
GITHUB_MODEL_NAME=gpt-4o
```

**Steps for Azure:**
1. Go to Azure Portal → Your App Service
2. Navigate to **Configuration** → **Application settings**
3. Add the environment variables above
4. Click **Save** and **Restart** the app service

### How It Works

1. **Create Obituary**: Biography field is now required
2. **AI Enhancement**: Click "Generate Description with AI" or "Enhance Description with AI"
3. **Smart Button**: Button text changes based on whether biography is empty or has content
4. **Error Handling**: Shows specific error messages for timeouts and failures

### API Endpoint

- **POST** `/api/ObituariesApi/enhance-description`
- **Auth**: Requires JWT token (user must be logged in)
- **Body**: `{ "FullName": "Person Name", "CurrentDescription": "optional existing text" }`
- **Response**: `{ "enhancedDescription": "AI generated/enhanced text" }`

### Security Notes

- The `.env` file is excluded from Git commits
- GitHub token is only used server-side
- API calls require user authentication
- Rate limiting is handled by GitHub Models service