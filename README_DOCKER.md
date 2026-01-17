# 🛡️ FraudGuard - Enterprise Fraud Detection Platform

**Complete Docker Setup & Deployment Guide**

[![Docker](https://img.shields.io/badge/Docker-Ready-blue?style=flat&logo=docker)](https://www.docker.com/)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=flat&logo=.net)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-17-red?style=flat&logo=angular)](https://angular.io/)
[![Python](https://img.shields.io/badge/Python-3.11-green?style=flat&logo=python)](https://www.python.org/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-orange?style=flat&logo=microsoft-sql-server)](https://www.microsoft.com/en-us/sql-server/)

---

## 🚀 Quick Start (30 seconds)

### Windows Users
```bash
# Double-click this file:
START_FRAUDGUARD.bat
```

### Mac/Linux Users
```bash
chmod +x START_FRAUDGUARD.sh
./START_FRAUDGUARD.sh
```

### Manual Start (All Platforms)
```bash
git clone <your-repository-url>
cd PFA_Project-main
docker-compose -f docker-compose.simple.yml up --build
```

**Then open:** http://localhost

---

## 📚 Documentation

**Choose your path:**

| Document | Purpose | Time |
|----------|---------|------|
| **[DOCUMENTATION_INDEX.md](DOCUMENTATION_INDEX.md)** | Start here - Overview of all docs | 5 min |
| **[DOCKER_COMMANDS_REFERENCE.md](DOCKER_COMMANDS_REFERENCE.md)** | Essential commands & troubleshooting | 10 min |
| **[DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md)** | Step-by-step deployment guide | 20 min |
| **[COMPLETE_DOCKER_SETUP.md](COMPLETE_DOCKER_SETUP.md)** | Full technical reference | 30 min |

---

## 🎯 What You Get

A complete fraud detection platform with:

- ✅ **Modern Frontend** - Angular 17 with admin & user dashboards
- ✅ **Powerful Backend** - ASP.NET Core 8 REST API
- ✅ **ML Engine** - Python Flask with XGBoost fraud detection
- ✅ **Database** - SQL Server 2022 with sample data
- ✅ **Cache Layer** - Redis for performance
- ✅ **Message Queue** - Kafka for event streaming
- ✅ **Monitoring** - Prometheus & Grafana dashboards
- ✅ **All Containerized** - Run anywhere with Docker

---

## 🔐 Default Credentials

### Web Application
```
Admin:    admin@fraudguard.com / Admin@123
User:     demo@test.com / demo123
```

### Database
```
Server:   localhost:1433
User:     sa
Password: FraudGuard@2024!
```

### Monitoring (Grafana)
```
URL:      http://localhost:3000
User:     admin
Password: FraudGuard@2024
```

---

## 📊 System Architecture

```
┌─────────────────────────────────────────────────┐
│           Web Browser - Port 80                 │
│          (Angular Frontend)                     │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│    ASP.NET Core API - Port 5203                 │
│    (Controllers, Services, Repositories)        │
└────────────────┬────────────────────────────────┘
         ┌───────┴───────┐
         │               │
    ┌────▼────┐     ┌────▼──────┐
    │Database │     │   ML      │
    │SQL Srv  │     │  Flask    │
    │1433     │     │  5000     │
    └────┬────┘     └────┬──────┘
         │               │
    ┌────▼───────────────▼────┐
    │  Redis + Kafka           │
    │  (Cache & Events)        │
    └──────────────────────────┘
```

---

## ⚡ Essential Commands

```bash
# Start services
docker-compose -f docker-compose.simple.yml up --build

# Check status
docker-compose ps

# View logs
docker-compose logs -f

# Stop services
docker-compose stop

# Stop and remove data
docker-compose down -v

# Restart specific service
docker-compose restart api
docker-compose restart database
docker-compose restart ml
```

---

## 🌐 Access Points

| Service | URL | Purpose |
|---------|-----|---------|
| **Application** | http://localhost | Main web app |
| **API Docs** | http://localhost:5203/swagger | API documentation |
| **ML Health** | http://localhost:5000/health | ML service status |
| **Database** | localhost:1433 | SQL Server |
| **Grafana** | http://localhost:3000 | Dashboards |
| **Prometheus** | http://localhost:9090 | Metrics |

---

## 📋 System Requirements

### Minimum
- RAM: 8GB
- Disk: 20GB free
- CPU: 4 cores
- OS: Windows 10+, macOS 10.15+, Ubuntu 18.04+

### Recommended
- RAM: 16GB
- Disk: 40GB free
- CPU: 8+ cores
- OS: Windows 11, macOS 12+, Ubuntu 22.04+

---

## 🎓 First Time Setup

### Step 1: Prerequisites
```bash
# Verify installations
docker --version      # Should be 20.10+
docker-compose --version  # Should be 2.0+
git --version         # Should be 2.30+
```

### Step 2: Clone Repository
```bash
git clone <your-repository-url>
cd PFA_Project-main
```

### Step 3: Start Services
```bash
docker-compose -f docker-compose.simple.yml up --build
# Wait 2-3 minutes for everything to start
```

### Step 4: Access Application
```
Open browser: http://localhost
Login with:
  Email: admin@fraudguard.com
  Password: Admin@123
```

---

## 🔧 Project Structure

```
PFA_Project-main/
├── 📘 Documentation (you are here)
│   ├── DOCUMENTATION_INDEX.md ⭐ Start here
│   ├── COMPLETE_DOCKER_SETUP.md
│   ├── DEPLOYMENT_CHECKLIST.md
│   └── DOCKER_COMMANDS_REFERENCE.md
│
├── 🐳 Docker Configuration
│   ├── docker-compose.yml (full stack)
│   ├── docker-compose.simple.yml (basic)
│   ├── START_FRAUDGUARD.bat (Windows)
│   └── START_FRAUDGUARD.sh (Mac/Linux)
│
├── 🔌 FraudDetectionAPI/
│   ├── Controllers/ (API endpoints)
│   ├── Models/ (Database models)
│   ├── Services/ (Business logic)
│   ├── Data/ (Database context)
│   └── Dockerfile
│
├── 🐍 FraudDetectionML/
│   ├── src/app.py (Flask server)
│   ├── models/ (Trained ML models)
│   ├── data/ (Training data)
│   └── Dockerfile
│
├── 🎨 FraudDetectionUI/
│   ├── src/app/ (Angular components)
│   ├── src/assets/ (Static files)
│   ├── nginx.conf (Web server config)
│   └── Dockerfile
│
└── 📊 monitoring/
    ├── prometheus/ (Metrics collection)
    └── grafana/ (Dashboards)
```

---

## 🆘 Troubleshooting

### Port Already in Use
```bash
# Windows
netstat -ano | findstr :80
taskkill /PID <number> /F

# Mac/Linux
lsof -i :80
kill -9 <PID>
```

### Database Connection Failed
```bash
docker-compose restart database
# Wait 60 seconds for SQL Server to initialize
```

### Out of Disk Space
```bash
docker system prune -a --volumes
```

### Full troubleshooting guide
👉 See [COMPLETE_DOCKER_SETUP.md](COMPLETE_DOCKER_SETUP.md#detailed-troubleshooting)

---

## 📖 Where to Go Next

### I want to start immediately
👉 **[DOCKER_COMMANDS_REFERENCE.md](DOCKER_COMMANDS_REFERENCE.md)** - All commands in 5 minutes

### I want step-by-step instructions
👉 **[DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md)** - Checklist with verification

### I want complete technical details
👉 **[COMPLETE_DOCKER_SETUP.md](COMPLETE_DOCKER_SETUP.md)** - Full reference manual

### I want to understand the project
👉 **[DOCUMENTATION_INDEX.md](DOCUMENTATION_INDEX.md)** - Complete overview

---

## ✨ Features

### Admin Dashboard
- View overall fraud statistics
- Manage suspicious transactions
- Review fraud alerts
- Analyze fraud trends
- User management
- System analytics

### User Dashboard
- View transaction history
- Check fraud alerts
- See account statistics
- Download reports
- Manage personal data

### Real-time Fraud Detection
- ML model evaluates every transaction
- Instant fraud probability scoring
- Configurable alert thresholds
- Automated suspicious transaction flagging
- Detailed fraud reasoning

### Advanced Analytics
- 30-day fraud trends
- Geographical fraud patterns
- Device-based analysis
- Time-based patterns
- Risk profiling

---

## 🛡️ Security

- ✅ JWT Authentication
- ✅ Role-Based Access Control (RBAC)
- ✅ HTTPS/TLS Support
- ✅ Encrypted Database Connections
- ✅ Secure Password Hashing
- ✅ SQL Injection Protection
- ✅ XSS Protection
- ✅ CSRF Protection

---

## 📞 Support & Help

### Quick Reference
- **Commands:** [DOCKER_COMMANDS_REFERENCE.md](DOCKER_COMMANDS_REFERENCE.md)
- **Issues:** [COMPLETE_DOCKER_SETUP.md#troubleshooting](COMPLETE_DOCKER_SETUP.md#detailed-troubleshooting)
- **Setup:** [DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md)

### API Documentation
- **Swagger UI:** http://localhost:5203/swagger (after starting)
- **ML Health:** http://localhost:5000/health
- **Database:** SQL Server on port 1433

### Getting Help
1. Check the relevant documentation file above
2. Review troubleshooting section in [COMPLETE_DOCKER_SETUP.md](COMPLETE_DOCKER_SETUP.md)
3. Check Docker logs: `docker-compose logs -f`
4. Verify system requirements are met

---

## 🎯 Next Steps

1. ✅ **Setup** - Choose quick start script above
2. ✅ **Access** - Open http://localhost in browser
3. ✅ **Login** - Use credentials provided
4. ✅ **Explore** - Try the admin dashboard
5. ✅ **Customize** - Configure rules and thresholds
6. ✅ **Deploy** - Use in production with real data

---

## 📝 Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Frontend | Angular | 17 |
| Backend | ASP.NET Core | 8 |
| ML Engine | Python + Flask | 3.11 |
| Database | SQL Server | 2022 |
| Cache | Redis | 7 |
| Queue | Kafka | 7.5 |
| Monitoring | Prometheus | 2.47 |
| Dashboards | Grafana | 10.1 |
| Container | Docker | 20.10+ |
| Orchestration | Docker Compose | 2.0+ |

---

## 📄 License

[Add your license here]

---

## 👥 Contributing

[Add contribution guidelines here]

---

## 📞 Contact

[Add contact information here]

---

## 🎉 Ready to Get Started?

### Choose your startup method:

**Option 1: Double-click (Easiest)**
- Windows: `START_FRAUDGUARD.bat`
- Mac/Linux: `START_FRAUDGUARD.sh`

**Option 2: Command line**
```bash
docker-compose -f docker-compose.simple.yml up --build
```

**Option 3: Full details**
- See [DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md)

---

**Last Updated:** January 17, 2026  
**Project Status:** ✅ Complete & Ready for Deployment  
**Docker Configuration:** ✅ Production-Ready

---

For detailed information, see **[DOCUMENTATION_INDEX.md](DOCUMENTATION_INDEX.md)**

*FraudGuard - Intelligent Fraud Detection Platform* 🛡️
