@echo off
REM =============================================================================
REM FraudGuard - Docker Startup Script (Windows)
REM =============================================================================
REM Usage: Double-click this file or run in command prompt
REM =============================================================================

echo.
echo ========================================
echo   FraudGuard - Docker Setup
echo ========================================
echo.

REM Check if Docker is installed
docker --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Docker is not installed or not in PATH
    echo.
    echo Please install Docker Desktop from: https://www.docker.com/products/docker-desktop
    echo.
    pause
    exit /b 1
)

echo [1/5] Docker found: Checking Docker Compose...
docker-compose --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Docker Compose is not installed
    echo.
    echo Please install Docker Desktop which includes Docker Compose
    echo.
    pause
    exit /b 1
)

echo [2/5] All prerequisites OK
echo.
echo Checking Docker Desktop...
docker ps >nul 2>&1
if errorlevel 1 (
    echo ERROR: Docker Desktop is not running
    echo.
    echo Please start Docker Desktop and try again
    echo.
    pause
    exit /b 1
)

echo [3/5] Docker Desktop is running
echo.
echo [4/5] Starting FraudGuard services...
echo.
echo Pulling latest images and building containers...
echo This may take 2-3 minutes on first run...
echo.

REM Start services
docker-compose -f docker-compose.simple.yml up --build

echo.
echo [5/5] Startup complete!
echo.
echo ========================================
echo   Access the Application
echo ========================================
echo.
echo Frontend:   http://localhost
echo API Docs:   http://localhost:5203/swagger
echo ML Health:  http://localhost:5000/health
echo.
echo ========================================
echo   Login Credentials
echo ========================================
echo.
echo Admin:
echo   Email:    admin@fraudguard.com
echo   Password: Admin@123
echo.
echo User:
echo   Email:    demo@test.com
echo   Password: demo123
echo.
echo ========================================
echo.
echo To stop the services, press Ctrl+C
echo.
pause
