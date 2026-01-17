# 🚀 FraudGuard - Quick Setup & Running Guide

**Copy-paste commands for instant project setup**

---

## 📋 TABLE OF CONTENTS

1. [Prerequisites](#prerequisites)
2. [Clone & Setup](#clone--setup)
3. [Run Everything (1 Command)](#run-everything-1-command)
4. [Architecture Overview](#architecture-overview)
5. [Access Services](#access-services)
6. [Common Commands](#common-commands)
7. [Troubleshooting](#troubleshooting)

---

## ✅ PREREQUISITES

### Check Your System

**Windows:**
```powershell
# Check Docker
docker --version

# Check Docker Compose
docker-compose --version

# If not installed, download from: https://www.docker.com/products/docker-desktop
```

**Linux/Mac:**
```bash
# Check Docker
docker --version

# Check Docker Compose
docker-compose --version

# Install if needed (Ubuntu/Debian)
sudo apt-get install docker.io docker-compose
```

**Minimum Requirements:**
- 8GB RAM
- 20GB disk space
- Docker & Docker Compose installed
- Git installed

---

## 📥 CLONE & SETUP

### Step 1: Clone the Repository

**Windows (PowerShell):**
```powershell
cd Desktop
git clone https://github.com/AmartiSamia/FraudGuard.git
cd FraudGuard
```

**Linux/Mac:**
```bash
cd ~
git clone https://github.com/AmartiSamia/FraudGuard.git
cd FraudGuard
```

### Step 2: Verify You're in the Right Directory

**Windows (PowerShell):**
```powershell
# You should see these files
Get-ChildItem -Name | Select-Object -First 10

# Expected output:
# docker-compose.yml
# docker-compose.simple.yml
# README.md
# TECHNOLOGY_AUDIT_REPORT.md
# FraudDetectionAPI/
# FraudDetectionML/
# FraudDetectionUI/
```

**Linux/Mac:**
```bash
ls -la

# Expected output:
# drwxr-xr-x  docker-compose.yml
# drwxr-xr-x  docker-compose.simple.yml
# drwxr-xr-x  README.md
# ...
```

---

## 🚀 RUN EVERYTHING (1 Command)

### ⚡ THE FASTEST WAY

**Windows (PowerShell) - RECOMMENDED:**
```powershell
.\START_FRAUDGUARD.bat
```

**Linux/Mac:**
```bash
./START_FRAUDGUARD.sh
```

**Manual Docker (All Platforms):**
```bash
docker-compose up -d
```

### ⏰ Wait for Startup

The system takes **2-3 minutes** to fully start:

```
⏳ Services starting...
  1. SQL Server Database        (60-90 seconds)
  2. Redis Cache                (10 seconds)
  3. Kafka Message Queue        (30 seconds)
  4. ASP.NET Core API           (30 seconds)
  5. Python ML Service          (20 seconds)
  6. Angular Frontend + NGINX   (20 seconds)
  7. Prometheus Metrics         (10 seconds)
  8. Grafana Dashboards         (10 seconds)

✅ Ready when you see all services as "healthy"
```

### ✅ Verify All Services Started

```powershell
# Windows
docker-compose ps

# Should show:
# NAME              STATUS
# fraudguard-db     Up 2 minutes (healthy)
# redis             Up 2 minutes (healthy)
# kafka             Up 2 minutes (healthy)
# zookeeper         Up 2 minutes (healthy)
# api               Up 2 minutes
# ml                Up 2 minutes
# ui                Up 2 minutes
# prometheus        Up 2 minutes
# grafana           Up 2 minutes
# kafka-ui          Up 2 minutes
```

---

## 🏗️ ARCHITECTURE OVERVIEW

### System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    USER BROWSER                              │
│                  http://localhost                            │
└────────────────────────┬────────────────────────────────────┘
                         │ (HTTPS)
                         ↓
        ┌────────────────────────────────┐
        │  NGINX Web Server (Port 80)     │
        │  Serves Angular Frontend        │
        └────────────────────┬────────────┘
                             │
            ┌────────────────┴───────────────┐
            ↓                                ↓
    ┌──────────────────┐          ┌──────────────────────┐
    │   Angular SPA    │          │  API Requests        │
    │   (Port 80)      │          │  (to http:5203)      │
    │                  │          └──────────┬───────────┘
    │ Components:      │                     │
    │ • Dashboard      │                     ↓
    │ • Transactions   │         ┌───────────────────────────┐
    │ • Fraud Alerts   │         │ ASP.NET Core API          │
    │ • User Profile   │         │ (Port 5203)               │
    └──────────────────┘         │                           │
                                 │ Controllers:              │
                                 │ • UserController          │
                                 │ • TransactionController   │
                                 │ • FraudAlertController    │
                                 │ • AdminController         │
                                 └──────┬────────────────────┘
                                        │
         ┌──────────────────────────────┼─────────────────────┐
         ↓                              ↓                     ↓
    ┌──────────┐                 ┌──────────┐        ┌──────────────┐
    │SQL Server│                 │  Redis   │        │    Kafka     │
    │Database  │                 │  Cache   │        │Message Queue │
    │Port 1433 │                 │Port 6379 │        │Port 9092     │
    │          │                 │          │        │              │
    │• Users   │                 │Stores:   │        │Topics:       │
    │• Trans.  │                 │• Users   │        │• fraudguard-  │
    │• Alerts  │                 │• Queries │        │  transactions │
    │• Accounts│                 │• Cache   │        │• fraudguard-  │
    │          │                 │• Sessions│        │  fraud-alerts │
    │          │                 │          │        │• fraudguard-  │
    └──────────┘                 └──────────┘        │  audit-log    │
         ▲                             │              └──────┬───────┘
         │                             │                     │
         │                             │        ┌────────────┘
         │                             │        ↓
         │                             │  ┌──────────────────┐
         │                             │  │   Python ML      │
         │                             │  │   Service        │
         │                             │  │   (Port 5000)    │
         │                             │  │                  │
         │                             │  │ Loads:           │
         │                             │  │ • XGBoost Model  │
         │                             │  │                  │
         │                             │  │ Predicts:        │
         │                             │  │ • Fraud Risk     │
         │                             │  │ • Confidence     │
         └──────────────────────────────┘  │ • Score         │
         Saves Fraud Alerts & Updates      └──────────────────┘
         
┌─────────────────────────────────────────────────────────────┐
│              MONITORING & OBSERVABILITY                      │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Prometheus (Port 9090)      →    Grafana (Port 3000)      │
│  Collects metrics every 15s       Beautiful dashboards      │
│  from all services               Real-time graphs           │
│                                  Admin: admin/FraudGuard@2024
│                                                              │
│  Metrics collected:                                          │
│  • API response times            • Database queries         │
│  • Request counts                • Fraud detection rate     │
│  • Error rates                   • System resource usage    │
│  • Cache hit rates               • Transaction volume       │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### How Data Flows

```
1️⃣  USER CREATES TRANSACTION
    ├─ Frontend (Angular)
    │   └─ POST /api/transactions
    │       └─ {"amount": 1500, "user_id": 1, ...}
    │
    ↓
    
2️⃣  API PROCESSES REQUEST
    ├─ ASP.NET Core validates input
    ├─ Saves to SQL Server database
    ├─ Publishes event to Kafka topic: "fraudguard-transactions"
    ├─ Caches result in Redis
    └─ Returns response to Angular

3️⃣  ML SERVICE DETECTS FRAUD (Async)
    ├─ Listens to Kafka: "fraudguard-transactions"
    ├─ Loads XGBoost model
    ├─ Makes prediction (fraud probability)
    ├─ Publishes result to Kafka: "fraudguard-fraud-alerts"
    └─ Takes ~50ms per transaction

4️⃣  API RECEIVES ALERT
    ├─ Listens to Kafka: "fraudguard-fraud-alerts"
    ├─ Saves FraudAlert to database
    ├─ Notifies user via dashboard
    └─ Updates Prometheus metrics

5️⃣  MONITORING CAPTURES DATA
    ├─ Prometheus scrapes metrics every 15 seconds
    ├─ Grafana visualizes dashboards
    └─ Admins see real-time fraud trends
```

### Technology Stack Layers

```
┌──────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                     │
├──────────────────────────────────────────────────────────┤
│  Angular 17 SPA           SASS/SCSS Styling             │
│  TypeScript Components    CSS Preprocessing             │
└──────────────────────────────────────────────────────────┘
                            ↓
┌──────────────────────────────────────────────────────────┐
│                   DELIVERY LAYER                          │
├──────────────────────────────────────────────────────────┤
│  NGINX Web Server (Port 80)                             │
│  • Serves static files (Angular compiled)               │
│  • Reverse proxy to API                                │
│  • Lightweight & fast                                  │
└──────────────────────────────────────────────────────────┘
                            ↓
┌──────────────────────────────────────────────────────────┐
│                     API LAYER                             │
├──────────────────────────────────────────────────────────┤
│  ASP.NET Core 8 (Port 5203)                            │
│  • REST API endpoints                                  │
│  • Business logic & validation                         │
│  • JWT authentication                                  │
│  • Entity Framework ORM                                │
└──────────────────────────────────────────────────────────┘
            ↓                    ↓                  ↓
    ┌───────────┐        ┌───────────┐      ┌──────────┐
    │ CACHING   │        │  MESSAGING │     │ ML       │
    ├───────────┤        ├───────────┤      ├──────────┤
    │ Redis 7   │        │ Kafka 7.5 │      │ Python   │
    │ Port 6379 │        │ Port 9092 │      │ Port 5000│
    │           │        │           │      │          │
    │ 3-5x Fast │        │ Real-time │      │ XGBoost  │
    │ Data      │        │ Events    │      │ Model    │
    └───────────┘        └───────────┘      └──────────┘
            ↓                    ↓                  ↓
        ┌───────────────────────────────────────────┐
        │        DATA PERSISTENCE LAYER             │
        ├───────────────────────────────────────────┤
        │     SQL Server 2022 (Port 1433)          │
        │     • Users, Transactions, Alerts        │
        │     • ACID transactions                  │
        │     • Backup & recovery                  │
        └───────────────────────────────────────────┘
            ↓
    ┌───────────────────────────────────────────┐
    │     MONITORING & OBSERVABILITY            │
    ├───────────────────────────────────────────┤
    │ Prometheus (9090)  Grafana (3000)        │
    │ Metrics Collection │ Visualization        │
    └───────────────────────────────────────────┘
            ↓
        ┌───────────────────────────────────────┐
        │   CONTAINER ORCHESTRATION             │
        ├───────────────────────────────────────┤
        │   Docker & Docker Compose             │
        │   • Isolated services                │
        │   • Network communication             │
        │   • Volume persistence               │
        └───────────────────────────────────────┘
```

### Service Communication Map

```
User Request Flow:
┌─────────────┐
│   BROWSER   │
│ localhost  │
└──────┬──────┘
       │ HTTP/HTTPS
       ↓
┌──────────────────┐
│   NGINX 80       │
│   Serve Files    │
└──────┬───────────┘
       │ Proxy /api
       ↓
┌──────────────────────────────────┐
│   ASP.NET CORE 5203              │
│   • Validate request              │
│   • Load from Redis (if cached)  │
│   • If not cached:                │
│     - Query SQL Server            │
│     - Save to Redis               │
│   • Publish to Kafka (if needed)  │
└──────┬───────────────────────────┘
       │ Publishes
       ↓
┌──────────────────────────────────┐
│   KAFKA 9092                      │
│   fraudguard-transactions topic  │
└──────┬───────────────────────────┘
       │ Subscribes
       ↓
┌──────────────────────────────────┐
│   PYTHON ML 5000                  │
│   • Load XGBoost model            │
│   • Process prediction            │
│   • Send result back to Kafka     │
└──────┬───────────────────────────┘
       │ Publishes
       ↓
┌──────────────────────────────────┐
│   KAFKA 9092                      │
│   fraudguard-fraud-alerts topic  │
└──────┬───────────────────────────┘
       │ Subscribes
       ↓
┌──────────────────────────────────┐
│   ASP.NET CORE 5203              │
│   • Receive prediction result     │
│   • Save to database              │
│   • Update cache                  │
│   • Send to Prometheus            │
└──────────────────────────────────┘
       │ Stores
       ↓
┌──────────────────────────────────┐
│   SQL SERVER 1433                 │
│   • Save fraud alert              │
│   • Update transaction status     │
└──────────────────────────────────┘
```

---

## 🔗 ACCESS SERVICES

### URLs & Credentials

| Service | URL | Credentials | Purpose |
|---------|-----|-------------|---------|
| **Frontend** | http://localhost | - | Web application |
| **API Docs** | http://localhost:5203/swagger | - | API documentation |
| **Grafana** | http://localhost:3000 | admin / FraudGuard@2024 | Real-time dashboards |
| **Prometheus** | http://localhost:9090 | - | Metrics explorer |
| **Kafka UI** | http://localhost:8080 | - | Message queue browser |

### Database Connection

**SQL Server:**
```
Server: localhost,1433
Username: sa
Password: FraudGuard@2024
Database: FraudDB
```

**Connect with SQL Server Management Studio:**
```
Server: localhost
Authentication: SQL Server Authentication
Login: sa
Password: FraudGuard@2024
Database: FraudDB
```

**Connect with Azure Data Studio:**
```
Server: localhost,1433
Authentication: SQL Server
User name: sa
Password: FraudGuard@2024
Database: FraudDB
```

---

## 💻 COMMON COMMANDS

### View Services Status

```bash
# See all running services
docker-compose ps

# See detailed info
docker-compose ps -a

# Check if all healthy
docker-compose ps | findstr "healthy"  # Windows
docker-compose ps | grep "healthy"      # Linux/Mac
```

### View Logs

```bash
# All services (last 50 lines, follow new output)
docker-compose logs -f --tail=50

# Specific service
docker-compose logs -f api              # API logs
docker-compose logs -f ml               # ML service logs
docker-compose logs -f redis            # Redis logs
docker-compose logs -f kafka            # Kafka logs
docker-compose logs -f database         # Database logs

# Get logs since 5 minutes ago
docker-compose logs --since 5m -f

# Get all logs (one time)
docker-compose logs > logs.txt
```

### Restart Services

```bash
# Restart all services
docker-compose restart

# Restart specific service
docker-compose restart api
docker-compose restart ml

# Restart and rebuild
docker-compose up -d --build
```

### Stop & Start

```bash
# Stop all services (data preserved)
docker-compose stop

# Start services again
docker-compose start

# Stop and remove containers (data in volumes preserved)
docker-compose down

# Stop and remove everything (DELETE DATA!)
docker-compose down -v
```

### Check Service Health

```bash
# Check Redis
docker-compose exec redis redis-cli ping

# Check Kafka
docker-compose exec kafka /opt/kafka/bin/kafka-topics.sh \
  --bootstrap-server localhost:9092 --list

# Check Database
docker-compose exec database sqlcmd -S localhost -U sa -P FraudGuard@2024 \
  -Q "SELECT 1"

# Check API
curl http://localhost:5203/health

# Check ML Service
curl http://localhost:5000/health
```

### Access Service Shells

```bash
# Enter API container
docker-compose exec api /bin/bash

# Enter ML container
docker-compose exec ml bash

# Enter Database container
docker-compose exec database bash

# Access Redis CLI
docker-compose exec redis redis-cli

# Exit any shell
exit
```

### Monitor Resource Usage

```bash
# Watch real-time resource usage
docker stats

# Get container resource stats
docker ps | wc -l        # Count containers
docker images | wc -l     # Count images
```

---

## 🚨 TROUBLESHOOTING

### Issue 1: "Docker not found"

**Solution:**
```powershell
# Windows - Install Docker Desktop
# Download from: https://www.docker.com/products/docker-desktop

# Then restart PowerShell and verify:
docker --version
docker-compose --version
```

### Issue 2: "Port already in use"

**Solution:**
```powershell
# Windows - Find process using port
netstat -ano | findstr :5203     # For port 5203

# Kill the process
taskkill /PID <PID> /F

# Or change docker-compose.yml port mapping
# Change "5203:5203" to "5204:5203" in docker-compose.yml
```

**Linux/Mac:**
```bash
# Find process using port
lsof -i :5203

# Kill the process
kill -9 <PID>
```

### Issue 3: Database connection timeout

**Solution:**
```bash
# Wait 60 seconds for SQL Server to start
docker-compose logs database | tail -20

# Check if database is healthy
docker-compose ps database

# If not healthy, wait more and try again
# SQL Server can take 2+ minutes to start
```

### Issue 4: Services keep stopping

**Solution:**
```bash
# Check logs for errors
docker-compose logs -f

# Restart with verbose output
docker-compose up  # (without -d, shows all logs in real-time)

# Check docker disk space
docker system df

# If disk full, prune unused images
docker system prune -a
```

### Issue 5: Redis connection failed

**Solution:**
```bash
# Check Redis is running
docker-compose ps redis

# Test Redis connection
docker-compose exec redis redis-cli ping
# Should respond: PONG

# Check Redis logs
docker-compose logs redis
```

### Issue 6: Kafka topics not found

**Solution:**
```bash
# List all topics
docker-compose exec kafka /opt/kafka/bin/kafka-topics.sh \
  --bootstrap-server localhost:9092 --list

# Expected topics:
# fraudguard-transactions
# fraudguard-fraud-alerts
# fraudguard-audit-log

# Create topic if missing
docker-compose exec kafka /opt/kafka/bin/kafka-topics.sh \
  --create \
  --topic fraudguard-transactions \
  --bootstrap-server localhost:9092 \
  --partitions 3 \
  --replication-factor 1
```

### Issue 7: API returning 500 error

**Solution:**
```bash
# Check API logs
docker-compose logs api -f

# Common causes:
# 1. Database not ready - wait 60+ seconds
# 2. Connection string wrong - check appsettings.json
# 3. Redis not running - restart Redis
# 4. Kafka not running - restart Kafka

# Restart API only
docker-compose restart api
```

### Issue 8: ML service not making predictions

**Solution:**
```bash
# Check ML logs
docker-compose logs ml -f

# Test ML service directly
curl -X POST http://localhost:5000/predict \
  -H "Content-Type: application/json" \
  -d '{"amount": 1500, "user_history_score": 80, "location_risk": 20, "time_risk": 10}'

# Check Kafka connection
docker-compose logs ml | grep -i kafka

# Restart ML service
docker-compose restart ml
```

### Issue 9: Frontend not loading

**Solution:**
```bash
# Check NGINX is running
docker-compose ps ui

# Check NGINX logs
docker-compose logs ui

# Test access
curl http://localhost

# If 502 Bad Gateway: API not responding
# Check if API is healthy
docker-compose logs api
```

### Issue 10: Out of memory

**Solution:**
```bash
# Check Docker memory limits
docker stats

# Stop services and cleanup
docker-compose down
docker system prune -a

# Increase Docker memory (Docker Desktop Settings)
# Windows/Mac: Right-click Docker icon → Settings → Resources → Memory (set to 8GB+)

# Or use lightweight setup
docker-compose -f docker-compose.simple.yml up -d
```

---

## 🔧 CONFIGURATION FILES

### Main Configuration: docker-compose.yml

Located in project root. Contains:
- All 10 services
- Port mappings
- Environment variables
- Health checks
- Volume mounts
- Network definitions

Edit to:
- Change ports
- Add environment variables
- Modify service configurations

### Application Configuration: appsettings.json

Located in: `FraudDetectionAPI/appsettings.json`

Important settings:
```json
{
  "Redis": {
    "Enabled": true,
    "ConnectionString": "redis:6379"
  },
  "Kafka": {
    "Enabled": true,
    "BootstrapServers": "kafka:9092"
  }
}
```

### ML Service Configuration: app_enhanced.py

Located in: `FraudDetectionML/src/app_enhanced.py`

Handles:
- Kafka subscriptions
- Model loading
- Prediction logic
- Response formatting

---

## 📊 IMPORTANT NOTES

### About Redis (Caching)

✅ **What it does:**
- Stores frequently accessed data in memory
- Makes API 3-5x faster
- Reduces database load by 70%

**Cache strategy:**
```
User data          → 30 minutes
Transactions       → 15 minutes
Fraud predictions  → 5 minutes
Dashboard stats    → 10 minutes
```

**If Redis is down:**
- Application still works (falls back to database)
- But response times will be slower
- Restart: `docker-compose restart redis`

### About Kafka (Event Streaming)

✅ **What it does:**
- Sends transactions to ML service
- Sends fraud alerts back to API
- Decouples services (async processing)
- Provides audit logging

**Topics:**
```
fraudguard-transactions  → API publishes, ML consumes
fraudguard-fraud-alerts  → ML publishes, API consumes
fraudguard-audit-log     → All services publish
```

**If Kafka is down:**
- Predictions won't be processed
- Alerts won't be received
- Restart: `docker-compose restart kafka`

### About Prometheus & Grafana

✅ **What they do:**
- Collect metrics every 15 seconds
- Visualize dashboards in real-time
- Monitor system health
- Track fraud detection rates

**To access:**
1. Open http://localhost:3000
2. Login: admin / FraudGuard@2024
3. See pre-built dashboards

**If not working:**
- Prometheus needs API to expose /metrics endpoint
- Check: http://localhost:5203/metrics
- Restart: `docker-compose restart prometheus grafana`

---

## ✨ VERIFICATION CHECKLIST

After running `docker-compose up -d`, verify:

```powershell
# 1. All services running
docker-compose ps
# Expected: All services with "Up" status

# 2. Database is healthy (wait up to 2 minutes)
docker-compose exec database sqlcmd -S localhost -U sa -P FraudGuard@2024 -Q "SELECT 1"
# Expected: Should complete without error

# 3. Redis responds
docker-compose exec redis redis-cli ping
# Expected: PONG

# 4. Kafka has topics
docker-compose exec kafka /opt/kafka/bin/kafka-topics.sh --bootstrap-server localhost:9092 --list
# Expected: Should list topics

# 5. API is healthy
curl http://localhost:5203/health
# Expected: HTTP 200

# 6. ML service is healthy
curl http://localhost:5000/health
# Expected: HTTP 200 or 404 (means service is running)

# 7. Frontend loads
curl http://localhost
# Expected: HTML content (Angular app)

# 8. Prometheus metrics
curl http://localhost:5203/metrics
# Expected: Prometheus metrics format

# 9. Grafana dashboard
# Open browser: http://localhost:3000
# Expected: Login page, then dashboards
```

---

## 🎯 QUICK REFERENCE

### Start Fresh

```bash
# Fresh start (clean everything)
docker-compose down -v
docker-compose up -d
```

### Monitor Everything

```bash
# Watch all logs in real-time
docker-compose logs -f

# Watch specific service
docker-compose logs -f api
```

### Check Health

```bash
# Services status
docker-compose ps

# System resources
docker stats

# Disk usage
docker system df
```

### Backup Data

```bash
# Backup database
docker-compose exec database /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P FraudGuard@2024 \
  -Q "BACKUP DATABASE FraudDB TO DISK = '/var/opt/mssql/backup/FraudDB.bak'"
```

### Clean Up

```bash
# Remove stopped containers
docker container prune -f

# Remove unused images
docker image prune -a -f

# Remove all unused resources
docker system prune -a -f
```

---

## 📞 SUPPORT & DOCUMENTATION

**More information:**
- **README.md** - Full project overview
- **TECHNOLOGY_AUDIT_REPORT.md** - Complete technology breakdown
- **SERVICES_GUIDE.md** - Detailed service explanations
- **COMMANDS_CHEAT_SHEET.md** - More Docker commands
- **COMPLETE_SETUP_SUMMARY.md** - Technical deep dive

**GitHub:** https://github.com/AmartiSamia/FraudGuard

---

## 🎉 YOU'RE READY!

### To summarize:

1. **Install Docker** if not already installed
2. **Clone the project:**
   ```bash
   git clone https://github.com/AmartiSamia/FraudGuard.git
   cd FraudGuard
   ```
3. **Run everything:**
   ```bash
   docker-compose up -d
   ```
4. **Wait 2-3 minutes** for all services to start
5. **Access the system:**
   - Frontend: http://localhost
   - API: http://localhost:5203
   - Grafana: http://localhost:3000

**That's it!** The entire fraud detection system is now running! 🚀

---

**Created:** January 17, 2026  
**Last Updated:** January 17, 2026  
**Status:** Production Ready ✅

---

*FraudGuard - Enterprise Fraud Detection Platform*
