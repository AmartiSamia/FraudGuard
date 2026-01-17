# 📘 FraudGuard - Documentation Index & Quick Links

**Your complete guide to running FraudGuard with Docker**

---

## 🚀 Start Here (Choose Your Path)

### ⚡ I want to start in 2 minutes
👉 **Go to:** [DOCKER_COMMANDS_REFERENCE.md](DOCKER_COMMANDS_REFERENCE.md)

**One command:**
```bash
cd PFA_Project-main
docker-compose -f docker-compose.simple.yml up --build
```

---

### 📖 I want detailed step-by-step instructions  
👉 **Go to:** [DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md)

**What you'll get:**
- ✅ Pre-deployment checklist
- ✅ Detailed setup steps
- ✅ Verification procedures
- ✅ Troubleshooting guide
- ✅ Daily operations guide

---

### 📚 I want complete technical documentation
👉 **Go to:** [COMPLETE_DOCKER_SETUP.md](COMPLETE_DOCKER_SETUP.md)

**What you'll get:**
- ✅ Full architecture overview
- ✅ Service descriptions
- ✅ Environment variables
- ✅ Advanced troubleshooting
- ✅ Database access guide
- ✅ System requirements

---

### 🔧 I want quick command reference
👉 **Go to:** [DOCKER_COMMANDS_REFERENCE.md](DOCKER_COMMANDS_REFERENCE.md)

**Commands included:**
- Start/stop services
- View logs
- Debug containers
- Clean up resources
- Database access

---

## 📋 Document Guide

| Document | Purpose | For Whom | Time |
|----------|---------|---------|------|
| [DOCKER_COMMANDS_REFERENCE.md](DOCKER_COMMANDS_REFERENCE.md) | Quick commands and examples | Everyone | 5 min |
| [DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md) | Step-by-step deployment | First-time deployers | 15 min |
| [COMPLETE_DOCKER_SETUP.md](COMPLETE_DOCKER_SETUP.md) | Technical reference | DevOps/Tech leads | 30 min |
| [DOCKER_SETUP.md](DOCKER_SETUP.md) | Quick start guide | Quick reference | 5 min |
| [QUICK_START.md](QUICK_START.md) | Traditional (non-Docker) setup | Development | Varies |

---

## 🎯 What This Project Includes

### Technology Stack

```
Frontend:        Angular 17 + Nginx
Backend:         ASP.NET Core 8
ML Service:      Python 3.11 + Flask + XGBoost
Database:        SQL Server 2022
Caching:         Redis 7
Message Queue:   Kafka 7.5
Monitoring:      Prometheus + Grafana
```

### Services & Ports

| Service | Port | URL | Purpose |
|---------|------|-----|---------|
| Frontend | 80 | http://localhost | Web Application |
| API | 5203 | http://localhost:5203 | REST API |
| ML | 5000 | http://localhost:5000 | Fraud Detection |
| Database | 1433 | localhost:1433 | SQL Server |
| Redis | 6379 | localhost:6379 | Cache |
| Kafka | 9092 | localhost:9092 | Messages |
| Prometheus | 9090 | http://localhost:9090 | Metrics |
| Grafana | 3000 | http://localhost:3000 | Dashboards |

---

## 🔐 Default Credentials

### Web Application

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@fraudguard.com` | `Admin@123` |
| User | `demo@test.com` | `demo123` |

### Database

| Property | Value |
|----------|-------|
| Server | `localhost:1433` or `localhost,1433` |
| User | `sa` |
| Password | `FraudGuard@2024!` |
| Database | `FraudDB` |

### Monitoring (Grafana)

| Property | Value |
|----------|-------|
| URL | http://localhost:3000 |
| User | `admin` |
| Password | `FraudGuard@2024` |

---

## ⚡ Quick Commands

### Start Services
```bash
# Simple setup (recommended)
docker-compose -f docker-compose.simple.yml up --build

# Full setup (with monitoring)
docker-compose up --build

# Run in background
docker-compose up -d --build
```

### Check Status
```bash
# List all containers
docker-compose ps

# View logs
docker-compose logs -f

# View specific service
docker-compose logs -f api
```

### Stop Services
```bash
# Stop all (keeps data)
docker-compose stop

# Stop and remove (keeps data)
docker-compose down

# Stop and DELETE all data
docker-compose down -v
```

### After Code Changes
```bash
# Rebuild and restart specific service
docker-compose build api && docker-compose restart api
docker-compose build ml && docker-compose restart ml
docker-compose build ui && docker-compose restart ui

# Or rebuild everything
docker-compose build && docker-compose restart
```

---

## 📊 Architecture Overview

```
┌─ CLIENT LAYER ─────────────────────────────────────────┐
│                                                         │
│  Browser (http://localhost)                             │
│           ↓                                             │
│  Angular Frontend (Nginx) - Port 80                    │
│                                                         │
└─ PRESENTATION LAYER ──────────────────────────────────┘
                     ↓ HTTP/REST
┌─ APPLICATION LAYER ────────────────────────────────────┐
│                                                         │
│  ASP.NET Core API - Port 5203                          │
│         ↓         ↓         ↓                           │
│    Repository  Service  Controller                     │
│                                                         │
└─ BUSINESS LAYER ──────────────────────────────────────┘
         ↓                        ↓                        │
┌────────────────┐     ┌──────────────────┐              │
│                │     │                  │              │
│  Database      │     │  ML Service      │              │
│  SQL Server    │     │  Flask/XGBoost   │              │
│  Port 1433     │     │  Port 5000       │              │
│                │     │                  │              │
└────────────────┘     └──────────────────┘              │
         ↓                        ↓                        │
│  Data Persistence        ML Predictions                 │
│  Fraud Records          Fraud Scores                    │
│  User Data              Confidence Level                │
│  Transactions                                            │
└─────────────────────────────────────────────────────────┘
         ↓ (Async)               ↓ (Async)
┌─────────────────┐     ┌──────────────────┐
│  Redis Cache    │     │  Kafka Queue     │
│  Port 6379      │     │  Port 9092       │
│  Session Store  │     │  Event Streaming │
└─────────────────┘     └──────────────────┘
         ↓                        ↓
│  Response Caching            Real-time Alerts
│  Session Management          Fraud Notifications
└─────────────────────────────────────────────────────────┘

┌─ MONITORING LAYER ─────────────────────────────────────┐
│                                                         │
│  Prometheus - Metrics Collection (Port 9090)           │
│         ↓                                              │
│  Grafana - Dashboards (Port 3000)                     │
│         ↓                                              │
│  Real-time Monitoring & Alerts                        │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## 📁 Project Structure

```
PFA_Project-main/
│
├── 📄 docker-compose.yml              # Full stack config
├── 📄 docker-compose.simple.yml       # Simple config
├── 📄 COMPLETE_DOCKER_SETUP.md        # Full guide ⭐
├── 📄 DEPLOYMENT_CHECKLIST.md         # Checklist ⭐
├── 📄 DOCKER_COMMANDS_REFERENCE.md    # Commands ⭐
├── 📄 This file (README Index)         # This file
│
├── 📦 FraudDetectionAPI/              # Backend
│   ├── Dockerfile
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Controllers/                   # API endpoints
│   ├── Models/                        # Database models
│   ├── Services/                      # Business logic
│   ├── Repositories/                  # Data access
│   └── Migrations/                    # Database versions
│
├── 🐍 FraudDetectionML/               # ML Service
│   ├── Dockerfile
│   ├── requirements.txt
│   ├── src/
│   │   ├── app.py                     # Flask server
│   │   └── app_enhanced.py
│   ├── models/                        # Trained models
│   └── data/                          # Training data
│
├── 🎨 FraudDetectionUI/               # Frontend
│   ├── Dockerfile
│   ├── nginx.conf
│   ├── package.json
│   ├── src/
│   │   ├── app/
│   │   ├── assets/
│   │   └── environments/
│   └── dist/                          # Built app
│
└── 📊 monitoring/                     # Monitoring
    ├── prometheus/
    │   └── prometheus.yml
    └── grafana/
        └── provisioning/
```

---

## 🆘 Troubleshooting

### Quick Fixes

| Problem | Quick Fix |
|---------|-----------|
| Port in use | See [COMPLETE_DOCKER_SETUP.md](COMPLETE_DOCKER_SETUP.md#-issue-3-port-already-in-use) |
| DB not starting | `docker-compose restart database` + wait 60s |
| API won't connect | Check: `docker-compose logs api` |
| Blank frontend page | Clear cache (Ctrl+F5) and try again |
| Out of space | `docker system prune -a --volumes` |
| Containers crashing | `docker-compose down && docker-compose up --build` |

### Detailed Help

👉 **For detailed troubleshooting:** See [COMPLETE_DOCKER_SETUP.md - Troubleshooting](COMPLETE_DOCKER_SETUP.md#detailed-troubleshooting)

👉 **For quick commands:** See [DOCKER_COMMANDS_REFERENCE.md - Common Issues](DOCKER_COMMANDS_REFERENCE.md#-common-issues--fixes)

---

## ✅ System Requirements

### Minimum
- OS: Windows 10, macOS 10.15+, Ubuntu 18.04+
- RAM: 8GB
- Disk: 20GB free
- CPU: 4 cores

### Recommended
- OS: Windows 11, macOS 12+, Ubuntu 22.04+
- RAM: 16GB
- Disk: 40GB free
- CPU: 8+ cores

---

## 🎓 Learning Resources

### For Beginners
1. Start with [DOCKER_COMMANDS_REFERENCE.md](DOCKER_COMMANDS_REFERENCE.md)
2. Follow [DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md)
3. Reference [COMPLETE_DOCKER_SETUP.md](COMPLETE_DOCKER_SETUP.md) as needed

### For Experienced Developers
1. Review [docker-compose.yml](docker-compose.yml) architecture
2. Check [FraudDetectionAPI/Program.cs](FraudDetectionAPI/Program.cs) for setup
3. Review [FraudDetectionML/src/app.py](FraudDetectionML/src/app.py) for ML service
4. Customize monitoring in [monitoring/](monitoring/)

### For DevOps/SRE
1. All Docker configurations are in root directory
2. Health checks configured in docker-compose.yml
3. Volume management documented in COMPLETE_DOCKER_SETUP.md
4. Backup/restore procedures included

---

## 🔗 Useful Links

### Official Documentation
- Docker: https://docs.docker.com/
- Docker Compose: https://docs.docker.com/compose/
- ASP.NET Core: https://learn.microsoft.com/en-us/aspnet/core/
- Angular: https://angular.io/docs

### In-Project Resources
- API Docs: http://localhost:5203/swagger (after starting)
- ML Health: http://localhost:5000/health
- Dashboard: http://localhost:3000 (Grafana - full setup only)

---

## 📞 Support

### Getting Help

**If something doesn't work:**

1. **Check logs:** `docker-compose logs -f`
2. **Try the fix:** See [COMPLETE_DOCKER_SETUP.md](COMPLETE_DOCKER_SETUP.md)
3. **Search commands:** [DOCKER_COMMANDS_REFERENCE.md](DOCKER_COMMANDS_REFERENCE.md)
4. **Follow checklist:** [DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md)

### Reporting Issues

Include:
- Your OS (Windows/Mac/Linux)
- Docker version output: `docker --version`
- Error logs: `docker-compose logs` (full output)
- Steps to reproduce

---

## 🎯 Next Steps

1. ✅ Choose your documentation path above
2. ✅ Clone the repository: `git clone <url>`
3. ✅ Start services: `docker-compose -f docker-compose.simple.yml up --build`
4. ✅ Open browser: http://localhost
5. ✅ Login with provided credentials
6. ✅ Explore the application

---

## 📝 Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | Jan 17, 2026 | Initial release with complete Docker setup |

---

**Happy Fraud Detection! 🛡️**

*For more information, see the specific documentation files listed at the top of this page.*
