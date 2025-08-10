// scripts/setup-dev-environment.ps1
# Purpose: PowerShell script to set up development environment
Write-Host "Setting up Gaming Library Development Environment..." -ForegroundColor Green

# Check if MongoDB is running
Write-Host "Checking MongoDB..." -ForegroundColor Yellow
try {
    $mongoStatus = mongo --eval "db.runCommand('ping')" --quiet 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ MongoDB is running" -ForegroundColor Green
    } else {
        Write-Host "❌ MongoDB is not running. Please start MongoDB first." -ForegroundColor Red
        Write-Host "   Install MongoDB: https://docs.mongodb.com/manual/installation/" -ForegroundColor Yellow
        exit 1
    }
} catch {
    Write-Host "❌ MongoDB is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

# Restore NuGet packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to restore packages" -ForegroundColor Red
    exit 1
}

# Build the solution
Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed" -ForegroundColor Red
    exit 1
}

# Run tests
Write-Host "Running unit tests..." -ForegroundColor Yellow
dotnet test tests/Unit/GamingLibrary.Domain.Tests/GamingLibrary.Domain.Tests.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Unit tests failed" -ForegroundColor Red
    exit 1
}

# Create logs directory
Write-Host "Creating logs directory..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path "src/Presentation/GamingLibrary.Api/logs"

Write-Host "✅ Development environment setup complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. cd src/Presentation/GamingLibrary.Api" -ForegroundColor White
Write-Host "2. dotnet run" -ForegroundColor White
Write-Host "3. Open https://localhost:7001 for Swagger UI" -ForegroundColor White

# .github/workflows/ci.yml
# Purpose: GitHub Actions CI/CD pipeline
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

env:
  DOTNET_VERSION: '8.0.x'

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      mongodb:
        image: mongo:7.0
        ports:
          - 27017:27017
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Test
      run: dotnet test --no-build --verbosity normal --configuration Release
      env:
        MongoDB__ConnectionString: mongodb://localhost:27017
        MongoDB__DatabaseName: GamingLibraryTest

  build-and-push:
    needs: test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Publish
      run: dotnet publish src/Presentation/GamingLibrary.Api/GamingLibrary.Api.csproj -c Release -o ./publish
    
    - name: Upload artifacts
      uses: actions/upload-artifact@v3
      with:
        name: gaming-library-api
        path: ./publish

# README.md
# Gaming Library - Professional Portfolio Project

A showcase gaming platform featuring two interactive games designed to demonstrate technical skills for interviews and portfolio presentations.

## 🎮 Games Featured

### "Deploy the Cat"
A DevOps-themed puzzle game where players navigate CI/CD pipelines while a mischievous cat tries to sabotage deployments.
- **Skills Demonstrated**: DevOps knowledge, deployment processes, problem-solving
- **Gameplay**: Click-and-drag pipeline components, overcome cat interference
- **Metrics Tracked**: Successful deployments, pipeline completion time, cat interventions

### "Git Blaster"  
A fast-paced shooter where players use Git commands as weapons against bad development practices.
- **Skills Demonstrated**: Git mastery, version control expertise, quick thinking
- **Gameplay**: Target enemies with appropriate Git commands, build accuracy streaks
- **Metrics Tracked**: Command accuracy, response time, Git vocabulary usage

## 🏗️ Architecture

Built using **Clean Architecture** principles with industry-standard patterns:

- **Domain Layer**: Pure business logic, entities, value objects
- **Application Layer**: CQRS with MediatR, use cases, validation
- **Infrastructure Layer**: MongoDB repositories, external services  
- **Presentation Layer**: ASP.NET Core Web API, SignalR hubs

### Key Design Patterns
- **CQRS** (Command Query Responsibility Segregation)
- **Repository Pattern** with specifications
- **Domain-Driven Design** (DDD) with domain events
- **Result Pattern** for error handling
- **MediatR** for request/response handling

## 🛠️ Technology Stack

### Backend
- **.NET 8** - Latest LTS version
- **ASP.NET Core** - Web API with advanced features
- **MongoDB** - Document database with optimized queries
- **SignalR** - Real-time communication