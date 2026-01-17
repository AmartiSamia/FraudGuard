# 📟 FraudGuard - Quick Reference Card

**Print this or keep it handy!**

---

## 🚀 START APPLICATION

### Windows (Easiest)
```
Double-click: START_FRAUDGUARD.bat
```

### Mac/Linux
```
./START_FRAUDGUARD.sh
```

### Any Platform
```
docker-compose -f docker-compose.simple.yml up --build
```

---

## 🌐 ACCESS POINTS

| App | URL | Login |
|-----|-----|-------|
| Main | http://localhost | See below |
| API | http://localhost:5203/swagger | (none) |
| ML Health | http://localhost:5000/health | (none) |
| DB | localhost:1433 | sa / FraudGuard@2024! |
| Grafana | http://localhost:3000 | admin / FraudGuard@2024 |

---

## 🔐 CREDENTIALS

### Admin
```
Email:    admin@fraudguard.com
Password: Admin@123
```

### User
```
Email:    demo@test.com
Password: demo123
```

---

## ⚡ ESSENTIAL COMMANDS

| What | Command |
|------|---------|
| **Check Status** | `docker-compose ps` |
| **View Logs** | `docker-compose logs -f` |
| **Stop Services** | `docker-compose stop` |
| **Start Services** | `docker-compose up -d --build` |
| **Restart All** | `docker-compose restart` |
| **Restart API Only** | `docker-compose restart api` |
| **Remove Everything** | `docker-compose down -v` |

---

## 🔧 REBUILD AFTER CODE CHANGES

```bash
# API (.NET) changes
docker-compose build api && docker-compose restart api

# ML (Python) changes  
docker-compose build ml && docker-compose restart ml

# UI (Angular) changes
docker-compose build ui && docker-compose restart ui

# Everything
docker-compose build && docker-compose restart
```

---

## 🆘 QUICK FIXES

| Problem | Solution |
|---------|----------|
| **Port in use** | Kill process or change port in docker-compose.yml |
| **DB won't start** | `docker-compose restart database` |
| **API won't connect** | `docker-compose logs api` (check logs) |
| **Blank page** | Clear browser cache (Ctrl+F5) |
| **Out of space** | `docker system prune -a --volumes` |
| **Container crashed** | `docker-compose logs -f <service-name>` |

---

## 📊 SERVICES & PORTS

| Service | Port | Status |
|---------|------|--------|
| Frontend | 80 | ✅ |
| API | 5203 | ✅ |
| ML | 5000 | ✅ |
| Database | 1433 | ✅ |
| Redis | 6379 | ✅ |
| Kafka | 9092 | ✅ |
| Prometheus | 9090 | ✅ |
| Grafana | 3000 | ✅ |

---

## 📝 DOCUMENTATION

| File | Use For |
|------|---------|
| README_DOCKER.md | Overview (start here) |
| DOCKER_COMMANDS_REFERENCE.md | All commands |
| DEPLOYMENT_CHECKLIST.md | Step-by-step setup |
| COMPLETE_DOCKER_SETUP.md | Technical details |

---

## 💾 DATABASE ACCESS

```bash
# Using Docker
docker-compose exec database /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!"

# Using SQL Server Management Studio
Server:   localhost,1433
User:     sa
Password: FraudGuard@2024!
Database: FraudDB
```

---

## 🔍 CONTAINER MANAGEMENT

```bash
# Inside a container shell
docker-compose exec api bash
docker-compose exec ml bash

# View container stats
docker stats

# Follow all logs
docker-compose logs -f

# Follow service logs
docker-compose logs -f api
docker-compose logs -f database
docker-compose logs -f ml
```

---

## ✅ VERIFY SETUP

After starting services:

```bash
# See all containers running
docker-compose ps

# Should show:
# - fraudguard-db (healthy)
# - fraudguard-api (up)
# - fraudguard-ml (up)
# - fraudguard-ui (up)
```

---

## 🎯 WORKFLOWS

### First Time Setup
1. Install Docker Desktop
2. Clone repository
3. Run: `docker-compose -f docker-compose.simple.yml up --build`
4. Wait 2-3 minutes
5. Open: http://localhost
6. Login with credentials above

### Daily Start
1. Run: `docker-compose up -d --build`
2. Access: http://localhost

### Daily Stop
1. Run: `docker-compose stop`

### After Code Changes
1. Edit code
2. Run: `docker-compose build <service> && docker-compose restart <service>`
3. Verify in browser

### Full Cleanup
1. Run: `docker-compose down -v`
2. Next startup will recreate everything

---

## 📞 NEED HELP?

- **Commands:** See DOCKER_COMMANDS_REFERENCE.md
- **Issues:** See COMPLETE_DOCKER_SETUP.md (Troubleshooting)
- **Setup:** See DEPLOYMENT_CHECKLIST.md
- **Overview:** See DOCUMENTATION_INDEX.md

---

## 🎓 QUICK FACTS

- **Setup Time:** 5-10 minutes
- **Startup Time:** 2-3 minutes
- **Disk Space Needed:** 20GB
- **RAM Needed:** 8GB (16GB recommended)
- **Ease:** Very Easy (one command)
- **All Data:** Persists after restart
- **All Services:** Included and configured

---

**Bookmark this page for quick reference!** 📌

---

*Last Updated: January 17, 2026*
