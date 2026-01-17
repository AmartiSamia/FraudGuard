# 🚀 FraudGuard - Clone & Run Guide

**Complete step-by-step instructions for running the project after cloning from GitHub**

---

## 📋 Table of Contents
1. [Prerequisites](#prerequisites)
2. [Clone the Repository](#clone-the-repository)
3. [Quick Start (2 Minutes)](#quick-start-2-minutes)
4. [Detailed Setup (If Manual)](#detailed-setup-if-manual)
5. [Verify Everything Works](#verify-everything-works)
6. [Access the Application](#access-the-application)
7. [Troubleshooting](#troubleshooting)
8. [All Commands Reference](#all-commands-reference)

---

## 📦 Prerequisites

Before you start, install these:

### 1. **Docker Desktop** ⭐ (REQUIRED)
- **Download:** https://www.docker.com/products/docker-desktop
- **Verify installation:**
  ```bash
  docker --version
  ```
  Expected output: `Docker version 20.10+`

### 2. **Git** (If not already installed)
- **Download:** https://git-scm.com/download
- **Verify installation:**
  ```bash
  git --version
  ```
  Expected output: `git version 2.30+`

### 3. **Code Editor (Optional)**
- Visual Studio Code: https://code.visualstudio.com/

### 4. **System Requirements**
- **RAM:** 8GB minimum (16GB recommended)
- **Disk Space:** 20GB free
- **CPU:** 4 cores minimum
- **OS:** Windows 10+, macOS 10.15+, Ubuntu 18.04+

---

## 🔄 Clone the Repository

Open your terminal/PowerShell and run:

```bash
# Navigate to where you want to store the project
cd Desktop

# Clone the repository
git clone https://github.com/AmartiSamia/FraudGuard.git

# Navigate into the project
cd FraudGuard

# Verify the structure
dir                    # Windows
ls                     # Mac/Linux
```

You should see these files/folders:
```
docker-compose.yml
docker-compose.simple.yml
FraudDetectionAPI/
FraudDetectionML/
FraudDetectionUI/
START_FRAUDGUARD.bat    (Windows)
START_FRAUDGUARD.sh     (Mac/Linux)
README_DOCKER.md
QUICK_REFERENCE_CARD.md
```

---

## 🚀 Quick Start (2 Minutes)

**Choose ONE option based on your OS:**

### **Option 1A: Windows (Easiest!)**

```bash
# Just double-click this file:
START_FRAUDGUARD.bat

# That's it! It will:
# 1. Check Docker is installed
# 2. Pull Docker images
# 3. Start all services
# 4. Show you the credentials and URLs
```

### **Option 1B: Mac/Linux (Easiest!)**

```bash
# Make the script executable
chmod +x START_FRAUDGUARD.sh

# Run it
./START_FRAUDGUARD.sh

# That's it! It will:
# 1. Check Docker is installed
# 2. Pull Docker images
# 3. Start all services
# 4. Show you the credentials and URLs
```

### **Option 2: Manual (Any Platform)**

```bash
# Navigate to project directory
cd FraudGuard

# Start the application
docker-compose -f docker-compose.simple.yml up --build

# Wait 2-3 minutes for all services to start
# You'll see messages like "fraudguard-db is healthy" when ready
```

**Stop it anytime with:** `Ctrl+C`

---

## ⏳ Wait for Startup

After running the startup command, you should see:

```
[+] Building 45.3s (45/45) FINISHED
[+] Running 5/5
fraudguard-db is healthy ✅
fraudguard-api is ready ✅
fraudguard-ml is ready ✅
fraudguard-ui is running ✅
```

**Do NOT proceed until you see these messages!**

---

## 🔧 Detailed Setup (If Manual)

### Step 1: Verify Prerequisites

```bash
# Check Docker
docker --version

# Check Docker Compose
docker-compose --version

# Check Git
git --version

# All should show version 2.0+
```

### Step 2: Navigate to Project

```bash
cd FraudGuard
```

### Step 3: Start Services

**Simple Setup (Recommended):**
```bash
docker-compose -f docker-compose.simple.yml up --build
```

**Full Setup (With Monitoring):**
```bash
docker-compose up --build
```

### Step 4: Wait for Startup

Watch the logs. Wait until you see all services are healthy.

### Step 5: Verify in New Terminal

Open a **new terminal window** and run:

```bash
docker-compose ps
```

You should see all containers with status "Up":
```
CONTAINER ID   IMAGE      PORTS
xxxxxxx        database   1433->1433  Up
xxxxxxx        api        5203->5203  Up
xxxxxxx        ml         5000->5000  Up
xxxxxxx        ui         80->80      Up
xxxxxxx        redis      6379->6379  Up
```

---

## ✅ Verify Everything Works

### Check Container Status

```bash
docker-compose ps
```

All containers should show status "Up" (not "Exit" or "Error")

### Check Logs

```bash
# View all logs
docker-compose logs

# View specific service
docker-compose logs api
docker-compose logs database
docker-compose logs ml
```

### Test API Health

```bash
# In a terminal, run:
curl http://localhost:5203/health

# Or open in browser:
# http://localhost:5203/swagger
```

### Test ML Health

```bash
# In a terminal, run:
curl http://localhost:5000/health

# Or open in browser:
# http://localhost:5000/health
```

---

## 🌐 Access the Application

### URLs

| Service | URL | Purpose |
|---------|-----|---------|
| **Application** | http://localhost | Main web app |
| **API Docs** | http://localhost:5203/swagger | API documentation |
| **ML Health** | http://localhost:5000/health | ML service status |
| **Database** | localhost:1433 | SQL Server (internal) |
| **Grafana** | http://localhost:3000 | Dashboards (full setup only) |

### Login Credentials

**Admin Account:**
```
Email:    admin@fraudguard.com
Password: Admin@123
```

**Demo User:**
```
Email:    demo@test.com
Password: demo123
```

### First Login Steps

1. Open browser: **http://localhost**
2. Click **"Login"**
3. Enter:
   - Email: `admin@fraudguard.com`
   - Password: `Admin@123`
4. Click **"Sign In"**
5. You're in! 🎉

---

## 🆘 Troubleshooting

### ❌ "Docker not found"

**Solution:**
```bash
# Install Docker Desktop from:
https://www.docker.com/products/docker-desktop

# Restart your terminal and verify:
docker --version
```

### ❌ "Port already in use"

**Windows:**
```powershell
# Find what's using the port
netstat -ano | findstr :80
netstat -ano | findstr :5203

# Kill the process (replace NNNN with the process ID)
taskkill /PID NNNN /F
```

**Mac/Linux:**
```bash
# Find what's using the port
lsof -i :80
lsof -i :5203

# Kill the process (replace NNNN with PID)
kill -9 NNNN
```

### ❌ "Database connection failed"

**Solution:**
```bash
# The database takes time to start
# Wait 60 seconds, then restart it:
docker-compose restart database

# Check logs:
docker-compose logs database
```

### ❌ "Containers not starting"

**Solution:**
```bash
# Stop everything
docker-compose down

# Rebuild and start
docker-compose up --build
```

### ❌ "Blank page in browser"

**Solution:**
1. Clear your browser cache: `Ctrl+F5` (Windows) or `Cmd+Shift+R` (Mac)
2. Close and reopen the browser
3. Try accessing again: http://localhost

### ❌ "Out of disk space"

**Solution:**
```bash
# Free up Docker resources
docker system prune -a --volumes

# This removes unused images and containers
```

### ❌ Still Having Issues?

1. Check the [README_DOCKER.md](README_DOCKER.md) file
2. See [DOCKER_COMMANDS_REFERENCE.md](DOCKER_COMMANDS_REFERENCE.md) for commands
3. Check [COMPLETE_DOCKER_SETUP.md](COMPLETE_DOCKER_SETUP.md) for detailed help
4. Run: `docker-compose logs -f` to see all logs

---

## 📋 All Commands Reference

### **START Services**
```bash
# Option 1: Simple setup
docker-compose -f docker-compose.simple.yml up --build

# Option 2: Full setup
docker-compose up --build

# Option 3: Run in background
docker-compose up -d --build
```

### **STOP Services**
```bash
# Stop all (keeps data)
docker-compose stop

# Stop and remove containers (keeps data)
docker-compose down

# Stop and DELETE all data
docker-compose down -v
```

### **CHECK Status**
```bash
# List all containers
docker-compose ps

# View all logs
docker-compose logs -f

# View specific service
docker-compose logs -f api
docker-compose logs -f database
docker-compose logs -f ml
```

### **RESTART Services**
```bash
# Restart all
docker-compose restart

# Restart specific service
docker-compose restart api
docker-compose restart database
docker-compose restart ml
```

### **AFTER CODE CHANGES**
```bash
# Rebuild API (.NET)
docker-compose build api && docker-compose restart api

# Rebuild ML (Python)
docker-compose build ml && docker-compose restart ml

# Rebuild UI (Angular)
docker-compose build ui && docker-compose restart ui

# Or rebuild everything
docker-compose build && docker-compose restart
```

### **CLEANUP & MAINTENANCE**
```bash
# View resource usage
docker stats

# View container info
docker-compose ps -a

# Remove unused Docker resources
docker system prune -a --volumes

# View logs with limited lines
docker-compose logs --tail=50
```

### **DEBUGGING**
```bash
# Enter a container shell
docker-compose exec api bash
docker-compose exec database /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!"

# Test connectivity
docker-compose exec api ping database

# View service logs
docker-compose logs api
```

---

## 📚 Documentation Files

After cloning, you'll have these helpful files:

| File | Purpose | Read Time |
|------|---------|-----------|
| **CLONE_AND_RUN.md** | This file - how to clone and run | 10 min |
| **README_DOCKER.md** | Main Docker guide | 10 min |
| **QUICK_REFERENCE_CARD.md** | One-page command reference | 3 min |
| **DOCKER_COMMANDS_REFERENCE.md** | All commands with examples | 10 min |
| **DEPLOYMENT_CHECKLIST.md** | Step-by-step checklist | 20 min |
| **COMPLETE_DOCKER_SETUP.md** | Full technical reference | 30 min |

---

## 🎯 What Gets Installed

When you run the startup command, these services are automatically started:

### Basic Setup (Simple Mode)
- ✅ **Frontend** - Angular web app (Port 80)
- ✅ **API** - ASP.NET Core backend (Port 5203)
- ✅ **ML Service** - Python fraud detection (Port 5000)
- ✅ **Database** - SQL Server 2022 (Port 1433)
- ✅ **Redis** - Cache layer (Port 6379)

### Full Setup (Full Mode)
- All of the above, plus:
- ✅ **Kafka** - Message queue (Port 9092)
- ✅ **Prometheus** - Metrics (Port 9090)
- ✅ **Grafana** - Dashboards (Port 3000)
- ✅ **Zookeeper** - Kafka coordinator (Port 2181)

**Database is initialized automatically with sample data!**

---

## 🔐 Database Connection

If you need direct database access:

**Using Docker:**
```bash
docker-compose exec database /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!"
```

**Using SQL Server Management Studio:**
- Server: `localhost:1433` or `localhost,1433`
- Username: `sa`
- Password: `FraudGuard@2024!`
- Database: `FraudDB`

---

## ✨ Common Workflows

### Workflow 1: First Time Setup
```bash
# Step 1: Clone
git clone https://github.com/AmartiSamia/FraudGuard.git
cd FraudGuard

# Step 2: Start
docker-compose -f docker-compose.simple.yml up --build

# Step 3: Wait for startup (2-3 minutes)

# Step 4: Access
# Open: http://localhost
# Login: admin@fraudguard.com / Admin@123
```

### Workflow 2: Daily Start/Stop
```bash
# Start
docker-compose up -d --build

# Stop
docker-compose stop
```

### Workflow 3: Fix Issues
```bash
# See what's wrong
docker-compose logs -f

# Restart problematic service
docker-compose restart api

# Or restart everything
docker-compose restart
```

### Workflow 4: Make Code Changes
```bash
# Edit your code in the appropriate folder

# Rebuild affected service
docker-compose build api && docker-compose restart api

# Or all services
docker-compose build && docker-compose restart
```

---

## 🎓 Tips & Tricks

1. **Keep logs running in a terminal:**
   ```bash
   docker-compose logs -f
   ```
   This shows all logs in real-time

2. **Check which containers are running:**
   ```bash
   docker-compose ps
   ```

3. **Stop services without losing data:**
   ```bash
   docker-compose stop
   ```

4. **Reset everything and start fresh:**
   ```bash
   docker-compose down -v
   docker-compose up --build
   ```

5. **View resource usage:**
   ```bash
   docker stats
   ```

6. **Quick access to documentation:**
   - Quick commands: `QUICK_REFERENCE_CARD.md`
   - More details: `DOCKER_COMMANDS_REFERENCE.md`
   - Full reference: `COMPLETE_DOCKER_SETUP.md`

---

## ⏱️ Time Estimates

| Activity | Time |
|----------|------|
| Clone repository | 1-2 minutes |
| Install Docker (if needed) | 5-10 minutes |
| Start services | 1 minute |
| Wait for startup | 2-3 minutes |
| First login | 30 seconds |
| **Total time to working app** | **10-20 minutes** |

---

## ✅ Success Checklist

After following these steps, you should have:

- ✅ Docker Desktop installed
- ✅ Repository cloned
- ✅ All services running (`docker-compose ps` shows all "Up")
- ✅ Frontend accessible at http://localhost
- ✅ Can login with admin@fraudguard.com / Admin@123
- ✅ API docs available at http://localhost:5203/swagger
- ✅ ML service responding at http://localhost:5000/health
- ✅ Database connection working

---

## 🆘 Need More Help?

**For quick commands:** See `QUICK_REFERENCE_CARD.md`

**For specific issues:** See `COMPLETE_DOCKER_SETUP.md` → Troubleshooting

**For step-by-step:** See `DEPLOYMENT_CHECKLIST.md`

**For technical details:** See `COMPLETE_DOCKER_SETUP.md`

---

## 🚀 You're All Set!

**Next Steps:**

1. ✅ Explore the admin dashboard
2. ✅ Check API docs at http://localhost:5203/swagger
3. ✅ Test ML service at http://localhost:5000/health
4. ✅ Customize fraud detection rules
5. ✅ Import your own data

---

**Questions or issues? Check the documentation files or troubleshooting section above!**

---

**Last Updated:** January 17, 2026  
**Project:** FraudGuard - Enterprise Fraud Detection Platform  
**Status:** ✅ Ready for Use
