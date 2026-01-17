#!/bin/bash

# =============================================================================
# FraudGuard - Docker Startup Script (Mac/Linux)
# =============================================================================
# Usage: chmod +x START_FRAUDGUARD.sh && ./START_FRAUDGUARD.sh
# =============================================================================

echo ""
echo "========================================"
echo "  FraudGuard - Docker Setup"
echo "========================================"
echo ""

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "ERROR: Docker is not installed"
    echo ""
    echo "Please install Docker from: https://www.docker.com/products/docker-desktop"
    echo ""
    exit 1
fi

echo "[1/5] Docker found: $(docker --version)"

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    echo "ERROR: Docker Compose is not installed"
    echo ""
    echo "Please install Docker Desktop which includes Docker Compose"
    echo ""
    exit 1
fi

echo "[2/5] Docker Compose found: $(docker-compose --version)"

# Check if Docker daemon is running
if ! docker ps &> /dev/null; then
    echo "ERROR: Docker daemon is not running"
    echo ""
    echo "Please start Docker Desktop and try again"
    echo ""
    exit 1
fi

echo "[3/5] Docker daemon is running"
echo ""
echo "[4/5] Starting FraudGuard services..."
echo ""
echo "Pulling latest images and building containers..."
echo "This may take 2-3 minutes on first run..."
echo ""

# Start services
docker-compose -f docker-compose.simple.yml up --build

echo ""
echo "[5/5] Startup complete!"
echo ""
echo "========================================"
echo "  Access the Application"
echo "========================================"
echo ""
echo "Frontend:   http://localhost"
echo "API Docs:   http://localhost:5203/swagger"
echo "ML Health:  http://localhost:5000/health"
echo ""
echo "========================================"
echo "  Login Credentials"
echo "========================================"
echo ""
echo "Admin:"
echo "  Email:    admin@fraudguard.com"
echo "  Password: Admin@123"
echo ""
echo "User:"
echo "  Email:    demo@test.com"
echo "  Password: demo123"
echo ""
echo "========================================"
echo ""
echo "To stop the services, press Ctrl+C"
echo ""
