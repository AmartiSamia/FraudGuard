# 🐳 FraudGuard - Complete Docker Setup & Deployment Guide

**Last Updated:** January 17, 2026  
**Project:** FraudGuard - Enterprise Fraud Detection Platform

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Quick Start (Recommended)](#quick-start-recommended)
3. [Full Architecture](#full-architecture)
4. [Detailed Setup Instructions](#detailed-setup-instructions)
5. [Access & Login](#access--login)
6. [Docker Commands Reference](#docker-commands-reference)
7. [Troubleshooting](#troubleshooting)
8. [Project Structure](#project-structure)

---

## Prerequisites

### Required Software

Before starting, ensure you have the following installed on your system:

#### 1. **Docker Desktop** (Windows/Mac) or Docker Engine (Linux)
   - **Download:** https://www.docker.com/products/docker-desktop
   - **Requirements:**
     - Windows: Windows 10/11 Pro, Enterprise, or Education
     - 4GB RAM minimum (8GB recommended)
     - 10GB disk space for images and volumes

   **Verify Installation:**
   ```bash
   docker --version
   docker-compose --version
   ```

#### 2. **Git** (to clone the repository)
   - **Download:** https://git-scm.com/download
   
   **Verify Installation:**
   ```bash
   git --version
   ```

#### 3. **Code Editor (Optional)**
   - Visual Studio Code: https://code.visualstudio.com/
   - Or any editor of your choice

---

## Quick Start (Recommended)

### ✅ Method 1: Simple Setup (Best for First-Time Users)

This method starts all essential services with a single command:

```bash
# Step 1: Clone the repository
git clone <your-repository-url>
cd PFA_Project-main

# Step 2: Start all services
docker-compose -f docker-compose.simple.yml up --build

# Wait for approximately 2-3 minutes for all services to start
```

**Expected Output:**
```
fraudguard-db is healthy
fraudguard-api is ready
fraudguard-ml is ready
fraudguard-ui is running
```

### ✅ Method 2: Full Setup (With Monitoring & Message Queue)

For complete enterprise features with Kafka, Redis, Prometheus, and Grafana:

```bash
# Step 1: Clone the repository
git clone <your-repository-url>
cd PFA_Project-main

# Step 2: Start all services with full stack
docker-compose up --build

# Wait for approximately 3-4 minutes for all services to start
```

---

## Full Architecture

When you run the Docker setup, the following services are deployed:

```
┌─────────────────────────────────────────────────────────────┐
│                    FraudGuard Platform                      │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────┐    ┌──────────────┐   ┌──────────────┐   │
│  │   Frontend  │    │     API      │   │   ML Model   │   │
│  │  (Angular)  │◄──►│   (ASP.NET)  │◄──┤   (Python)   │   │
│  │  Port: 80   │    │ Port: 5203   │   │ Port: 5000   │   │
│  └─────────────┘    └──────────────┘   └──────────────┘   │
│         │                   │                    │         │
│         └───────────────────┼────────────────────┘         │
│                             │                              │
│         ┌───────────────────┴────────────────────┐         │
│         │                                        │         │
│    ┌────▼─────┐  ┌───────────┐  ┌────────────┐ │         │
│    │ Database  │  │   Redis   │  │   Kafka    │ │         │
│    │ (SQL Srv) │  │ (Cache)   │  │  (Queue)   │ │         │
│    │ Port 1433 │  │ Port 6379 │  │ Port 9092  │ │         │
│    └───────────┘  └───────────┘  └────────────┘ │         │
│                                                  │         │
│    ┌─────────────────┬──────────────────────┐   │         │
│    │                 │                      │   │         │
│  ┌─▼─────────┐   ┌──▼──────┐   ┌─────────▼─┐  │         │
│  │ Prometheus│   │ Grafana  │   │  Kafka UI │  │         │
│  │ Port 9090 │   │ Port 3000│   │ Port 8080 │  │         │
│  └───────────┘   └──────────┘   └───────────┘  │         │
│                                                  │         │
└──────────────────────────────────────────────────┘         │
```

### Services Overview

| Service | Technology | Port | Purpose | Status |
|---------|-----------|------|---------|--------|
| **UI** | Angular + Nginx | 80 | Web Frontend | ✅ |
| **API** | ASP.NET Core 8 | 5203 | Backend REST API | ✅ |
| **ML** | Python Flask | 5000 | Fraud Detection Model | ✅ |
| **Database** | SQL Server 2022 | 1433 | Data Storage | ✅ |
| **Cache** | Redis 7 | 6379 | Session/Cache Store | ✅ |
| **Message Queue** | Kafka 7.5 | 9092 | Event Streaming | ✅ |
| **Monitoring** | Prometheus | 9090 | Metrics Collection | ✅ |
| **Dashboards** | Grafana | 3000 | Visualization | ✅ |
| **Kafka UI** | Kafka UI | 8080 | Queue Management | ✅ |
| **Zookeeper** | Zookeeper | 2181 | Kafka Coordination | ✅ |

---

## Detailed Setup Instructions

### Step 1: Prerequisites Check

Before proceeding, verify all prerequisites are installed:

```bash
# Check Docker
docker --version
# Expected: Docker version 20.10+

docker-compose --version
# Expected: Docker Compose version 2.0+

# Check Git
git --version
# Expected: git version 2.30+

# Check available disk space
# You need at least 10GB free space
```

### Step 2: Clone the Repository

```bash
# Navigate to where you want to store the project
cd C:\Users\YourUsername\Desktop  # Windows
# OR
cd ~/Desktop  # Mac/Linux

# Clone the repository
git clone https://github.com/yourorg/PFA_Project-main.git
cd PFA_Project-main

# Verify the structure
dir  # Windows
ls   # Mac/Linux
```

**Expected files:**
- `docker-compose.yml` - Full stack configuration
- `docker-compose.simple.yml` - Simplified configuration
- `FraudDetectionAPI/` - Backend API
- `FraudDetectionML/` - ML Service
- `FraudDetectionUI/` - Frontend Application
- `monitoring/` - Prometheus & Grafana configs

### Step 3: Start Services

#### Option A: Simple Setup (Recommended for First Use)

```bash
# Navigate to project root
cd PFA_Project-main

# Start services
docker-compose -f docker-compose.simple.yml up --build

# Output will show:
# [+] Building 45.3s (45/45) FINISHED
# [+] Running 5/5
# fraudguard-db is healthy
# fraudguard-api is ready
# fraudguard-ml is ready
# fraudguard-ui is running
```

**What's started:**
- SQL Server Database
- ASP.NET Core API
- Python ML Service
- Angular Frontend
- Redis Cache

**Don't have docker-compose.simple.yml?** Use the full docker-compose.yml:

```bash
docker-compose up --build
```

#### Option B: Full Setup (With Kafka & Monitoring)

```bash
# Navigate to project root
cd PFA_Project-main

# Start all services
docker-compose up --build

# Wait 3-4 minutes for complete startup
```

**What's started:**
- All services from Simple Setup
- Kafka Message Queue
- Zookeeper (Kafka Coordinator)
- Prometheus (Metrics)
- Grafana (Dashboards)
- Kafka UI (Queue Management)

### Step 4: Verify Services Are Running

Open a new terminal window and check container status:

```bash
# List all running containers
docker-compose ps

# Expected output:
# CONTAINER ID   IMAGE                  PORTS
# xxxxxxx        database               1433->1433
# xxxxxxx        api                    5203->5203
# xxxxxxx        ml                     5000->5000
# xxxxxxx        ui                     80->80
# xxxxxxx        redis:7                6379->6379
# xxxxxxx        kafka:latest           9092->9092
```

Or view container logs:

```bash
# View logs from all containers
docker-compose logs -f

# View specific service logs
docker-compose logs -f api
docker-compose logs -f database
docker-compose logs -f ml
docker-compose logs -f ui
```

---

## Access & Login

Once all services are running, access the platform through your browser:

### URLs & Endpoints

| Application | URL | Purpose | Port |
|-------------|-----|---------|------|
| **Frontend** | http://localhost | Main application UI | 80 |
| **API Swagger** | http://localhost:5203/swagger | API Documentation | 5203 |
| **ML Health** | http://localhost:5000/health | ML Service Status | 5000 |
| **Prometheus** | http://localhost:9090 | Metrics & Queries | 9090 |
| **Grafana** | http://localhost:3000 | Dashboards & Analytics | 3000 |
| **Kafka UI** | http://localhost:8080 | Queue Management | 8080 |
| **Database** | localhost:1433 | SQL Server (internal only) | 1433 |

### Default Login Credentials

#### Admin Account
- **Email:** `admin@fraudguard.com`
- **Password:** `Admin@123`
- **Role:** Administrator
- **Access:** Full admin dashboard, user management, analytics

#### Demo User Account
- **Email:** `demo@test.com`
- **Password:** `demo123`
- **Role:** User
- **Access:** User dashboard, transaction history, alerts

### First Login Steps

1. **Open browser and navigate to:** http://localhost
2. **Click "Login"** on the home page
3. **Enter credentials:**
   - Email: `admin@fraudguard.com`
   - Password: `Admin@123`
4. **Click "Sign In"**
5. **You're logged in!** Navigate to Admin Dashboard to explore

### Sample Features to Try

**As Admin:**
1. Go to Dashboard → View overall statistics
2. Navigate to Transactions → See flagged fraud cases
3. Check Alerts → Manage fraud notifications
4. View Analytics → See fraud trends and patterns

**As User:**
1. Go to Dashboard → See your account summary
2. View Transactions → See your transaction history
3. Check Alerts → See fraud alerts related to your account

---

## Docker Commands Reference

### Essential Commands

#### Starting & Stopping

```bash
# Start all services in background
docker-compose up -d --build

# Start specific service only
docker-compose up -d api
docker-compose up -d database
docker-compose up -d ml

# Stop all services
docker-compose stop

# Stop specific service
docker-compose stop api

# Restart all services
docker-compose restart

# Restart specific service
docker-compose restart api
```

#### Viewing Status & Logs

```bash
# View running containers
docker-compose ps

# View full logs from all containers
docker-compose logs

# View logs from specific service (last 50 lines)
docker-compose logs --tail=50 api

# Follow logs in real-time
docker-compose logs -f

# View logs from multiple services
docker-compose logs -f api database
```

#### Container Management

```bash
# Enter a container shell (for debugging)
docker-compose exec api bash
docker-compose exec database /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!"

# View resource usage
docker stats

# Remove all containers and volumes (DESTRUCTIVE)
docker-compose down -v

# Remove containers only (keeps data)
docker-compose down
```

#### Building & Rebuilding

```bash
# Rebuild images (after code changes)
docker-compose build

# Rebuild specific service
docker-compose build api

# Rebuild without cache
docker-compose build --no-cache
```

#### Cleaning Up

```bash
# Remove stopped containers
docker-compose rm

# Remove dangling images
docker image prune

# Remove unused volumes
docker volume prune

# Deep clean (remove all unused Docker resources)
docker system prune -a
```

### Network Debugging

```bash
# Inspect network
docker network inspect fraudguard-network

# Test connectivity from container
docker-compose exec api ping database

# Check port mappings
netstat -ano | findstr :5203  # Windows
lsof -i :5203  # Mac/Linux
```

---

## Detailed Troubleshooting

### 🔴 Issue 1: Containers Not Starting

**Symptoms:**
- `docker-compose ps` shows containers exited
- Logs show errors

**Solutions:**

```bash
# Check service logs
docker-compose logs api
docker-compose logs database

# Restart services
docker-compose restart

# Rebuild images
docker-compose down
docker-compose up --build

# Check logs in detail
docker-compose logs -f --tail=100
```

### 🔴 Issue 2: Database Connection Failed

**Symptoms:**
- API shows "Connection to database failed"
- Database port in use or not accessible

**Solutions:**

```bash
# Check database is running
docker-compose ps database

# Check database logs
docker-compose logs database

# Wait longer for database startup (60 seconds)
# Database takes time to initialize

# Restart database
docker-compose restart database

# Test connection
docker-compose exec api curl -X GET http://localhost:5203/api/health
```

### 🔴 Issue 3: Port Already in Use

**Symptoms:**
- Error: "bind: address already in use"
- Error: "Address already in use (:80)"

**Solutions (Windows):**

```powershell
# Find what's using the port
netstat -ano | findstr :80
netstat -ano | findstr :5203

# Kill process using port (replace NNNN with PID)
taskkill /PID NNNN /F

# Or change port in docker-compose.yml
# "80:80" → "8080:80"
```

**Solutions (Mac/Linux):**

```bash
# Find what's using the port
lsof -i :80
lsof -i :5203

# Kill process (replace NNNN with PID)
kill -9 NNNN

# Or change port in docker-compose.yml
```

### 🔴 Issue 4: API Can't Connect to Database

**Symptoms:**
- API container running but shows connection errors
- Database connection timeout

**Solutions:**

```bash
# Check database health
docker-compose ps database

# Check API logs
docker-compose logs api

# Verify database is healthy
docker-compose exec database /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!" -Q "SELECT 1"

# Restart both
docker-compose restart database api

# Wait 30 seconds, then check again
```

### 🔴 Issue 5: ML Service Not Loading Model

**Symptoms:**
- ML service running but `/health` returns errors
- "Model failed to load"

**Solutions:**

```bash
# Check ML logs
docker-compose logs ml

# Verify model files exist
docker-compose exec ml ls -la /app/models/

# Check Python dependencies
docker-compose exec ml pip list

# Rebuild ML service
docker-compose build --no-cache ml
docker-compose restart ml
```

### 🔴 Issue 6: UI Not Loading (Blank Page)

**Symptoms:**
- http://localhost shows blank page
- Browser console shows 404 errors

**Solutions:**

```bash
# Check UI logs
docker-compose logs ui

# Verify nginx is running
docker-compose ps ui

# Check Nginx configuration
docker-compose exec ui cat /etc/nginx/conf.d/default.conf

# Restart UI
docker-compose restart ui

# Rebuild UI (if code changed)
docker-compose build --no-cache ui
docker-compose restart ui
```

### 🔴 Issue 7: Insufficient Disk Space

**Symptoms:**
- Build fails with "No space left on device"
- Docker prune fails

**Solutions:**

```bash
# Check available space
# Windows: Check C: drive properties
# Mac/Linux: df -h

# Free up Docker resources
docker system prune -a --volumes

# This removes:
# - Stopped containers
# - Unused images
# - Unused volumes

# If still needed, manually remove images
docker rmi <image-id>
```

### 🔴 Issue 8: Database Initialization Failed

**Symptoms:**
- Database starts but is not initialized
- Tables don't exist

**Solutions:**

```bash
# Check if database initialized
docker-compose exec api curl -X GET http://localhost:5203/api/health

# Check API logs for migration errors
docker-compose logs api

# Rebuild and restart
docker-compose down -v
docker-compose up --build

# Wait 2-3 minutes for initial setup
```

### 📝 How to Get Help

When troubleshooting, collect this information:

```bash
# Save system information
docker --version
docker-compose --version
docker-compose ps > status.txt
docker-compose logs > logs.txt
docker stats --no-stream > resource-usage.txt

# Save configuration
cat docker-compose.yml > compose-config.txt

# Check disk space
df -h > disk-space.txt  # Mac/Linux
# or view C: drive properties (Windows)
```

---

## Project Structure

```
PFA_Project-main/
├── docker-compose.yml                 # Full stack configuration
├── docker-compose.simple.yml          # Simplified configuration
├── COMPLETE_DOCKER_SETUP.md          # This file
├── DOCKER_SETUP.md                   # Quick reference
├── QUICK_START.md                    # Traditional startup guide
│
├── FraudDetectionAPI/                # ASP.NET Core Backend
│   ├── Dockerfile
│   ├── Program.cs
│   ├── appsettings.json
│   ├── FraudDetectionAPI.csproj
│   ├── Controllers/
│   │   ├── UserController.cs
│   │   ├── TransactionController.cs
│   │   ├── FraudAlertController.cs
│   │   ├── AdminController.cs
│   │   └── ...
│   ├── Models/                       # Database models
│   ├── Repositories/                 # Data access layer
│   ├── Services/                     # Business logic
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── DatabaseSeeder.cs
│   └── Migrations/                   # EF Core migrations
│
├── FraudDetectionML/                 # Python ML Service
│   ├── Dockerfile
│   ├── requirements.txt
│   ├── src/
│   │   ├── app.py                    # Flask application
│   │   ├── app_enhanced.py
│   │   └── ...
│   ├── models/                       # Trained ML models
│   ├── data/
│   │   └── creditcard.csv            # Training data
│   └── generate_dataset.py
│
├── FraudDetectionUI/                 # Angular Frontend
│   ├── Dockerfile
│   ├── nginx.conf
│   ├── package.json
│   ├── src/
│   │   ├── app/
│   │   ├── assets/
│   │   └── ...
│   └── dist/                         # Built Angular app
│
└── monitoring/                        # Monitoring stack
    ├── prometheus/
    │   └── prometheus.yml            # Prometheus config
    └── grafana/
        └── provisioning/             # Grafana dashboards
```

---

## Environment Variables

All environment variables are set in `docker-compose.yml`. Here's what each does:

### Database Configuration
```
ACCEPT_EULA=Y                      # Accept SQL Server EULA
SA_PASSWORD=FraudGuard@2024!      # Admin password
MSSQL_PID=Express                 # SQL Server edition
```

### API Configuration
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=database;Database=FraudDB;...
Jwt__Key=<secret-key>
Jwt__Issuer=FraudGuardAPI
Jwt__Audience=FraudGuardUsers
Redis__ConnectionString=redis:6379
Kafka__BootstrapServers=kafka:9092
```

### ML Configuration
```
FLASK_ENV=production
KAFKA_BOOTSTRAP_SERVERS=kafka:9092
REDIS_HOST=redis
REDIS_PORT=6379
```

### Monitoring Configuration
```
GF_SECURITY_ADMIN_USER=admin
GF_SECURITY_ADMIN_PASSWORD=FraudGuard@2024
```

---

## Next Steps

After the application is running:

1. **Explore the UI:** Visit http://localhost
2. **Try the API:** Check http://localhost:5203/swagger
3. **Review ML Model:** Test http://localhost:5000/health
4. **Monitor System:** Open http://localhost:3000 (Grafana)
5. **Configure Custom Rules:** Use Admin Dashboard

---

## Support & Documentation

- **API Documentation:** http://localhost:5203/swagger
- **Database Connection:** Server: localhost:1433, User: sa, Password: FraudGuard@2024!
- **Frontend Source:** FraudDetectionUI/
- **API Source:** FraudDetectionAPI/
- **ML Source:** FraudDetectionML/

---

## Recommended System Requirements

- **OS:** Windows 10+, macOS 10.15+, or Ubuntu 18.04+
- **RAM:** 8GB minimum (16GB recommended)
- **CPU:** 4 cores minimum (8 cores recommended)
- **Disk:** 20GB free space
- **Internet:** Required for initial Docker image downloads

---

**Last Updated:** January 17, 2026  
**Version:** 1.0
