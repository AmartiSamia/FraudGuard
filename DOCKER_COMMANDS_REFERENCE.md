# 🚀 FraudGuard - Quick Command Reference

**For anyone running the project, here's everything they need to know in 5 minutes:**

---

## ⚡ One-Command Quick Start

```bash
# Clone the project
git clone <repository-url>
cd PFA_Project-main

# Start everything with Docker
docker-compose -f docker-compose.simple.yml up --build

# Wait 2-3 minutes, then open browser to:
# http://localhost
```

That's it! ✅

---

## 📱 Access the Application

Once running, open these in your browser:

| What | URL | Username | Password |
|------|-----|----------|----------|
| **Main App** | http://localhost | admin@fraudguard.com | Admin@123 |
| **API Docs** | http://localhost:5203/swagger | (none needed) | (none needed) |
| **ML Health** | http://localhost:5000/health | (none needed) | (none needed) |
| **Grafana** | http://localhost:3000 | admin | FraudGuard@2024 |

---

## 🛠️ Essential Commands

### Check What's Running

```bash
# See all containers
docker-compose ps

# See logs from everything
docker-compose logs -f

# See logs from one service
docker-compose logs -f api
docker-compose logs -f database
docker-compose logs -f ml
```

### Control Services

```bash
# Start everything
docker-compose up -d --build

# Stop everything
docker-compose stop

# Stop one service
docker-compose stop api

# Restart everything
docker-compose restart

# Restart one service
docker-compose restart api

# View resource usage
docker stats
```

### Stop & Clean Up

```bash
# Stop containers (keeps data)
docker-compose down

# Stop containers and DELETE all data
docker-compose down -v

# Clean up Docker resources
docker system prune -a
```

### Debugging

```bash
# Look inside a container
docker-compose exec api bash
docker-compose exec database /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!"

# Test if services can talk to each other
docker-compose exec api ping database

# See detailed logs
docker-compose logs --tail=100 api
```

---

## 🐛 Common Issues & Fixes

### ❌ "Port already in use"

**On Windows:**
```powershell
netstat -ano | findstr :80
taskkill /PID <number> /F
```

**On Mac/Linux:**
```bash
lsof -i :80
kill -9 <PID>
```

### ❌ "Database connection failed"

```bash
# Just restart the database
docker-compose restart database

# Wait 30 seconds, then try again
```

### ❌ "Docker not found"

Download Docker Desktop: https://www.docker.com/products/docker-desktop

### ❌ "Containers not starting"

```bash
# Rebuild everything
docker-compose down
docker-compose up --build
```

### ❌ "No space left on device"

```bash
# Free up Docker resources
docker system prune -a --volumes
```

---

## 📋 Default Login Credentials

**Admin Account (Full Access):**
```
Email:    admin@fraudguard.com
Password: Admin@123
```

**Demo User Account:**
```
Email:    demo@test.com
Password: demo123
```

---

## 🔄 If You Change Code

### If you modify the API (.NET) code:

```bash
docker-compose build api
docker-compose restart api
```

### If you modify ML code (Python):

```bash
docker-compose build ml
docker-compose restart ml
```

### If you modify UI (Angular) code:

```bash
docker-compose build ui
docker-compose restart ui
```

### If you modify everything:

```bash
docker-compose down
docker-compose up --build
```

---

## 📊 What's Running?

Here's what you get with `docker-compose -f docker-compose.simple.yml`:

- ✅ **Frontend** (Angular) - Port 80
- ✅ **API** (ASP.NET Core) - Port 5203  
- ✅ **ML Model** (Python/Flask) - Port 5000
- ✅ **Database** (SQL Server) - Port 1433
- ✅ **Redis Cache** - Port 6379

Or with full `docker-compose.yml`:

- ✅ All of the above, plus:
- ✅ **Kafka** (Message Queue) - Port 9092
- ✅ **Prometheus** (Metrics) - Port 9090
- ✅ **Grafana** (Dashboards) - Port 3000
- ✅ **Kafka UI** (Management) - Port 8080

---

## 🎯 Next Steps After Starting

1. **Open** http://localhost in browser
2. **Login** with `admin@fraudguard.com` / `Admin@123`
3. **Explore** the admin dashboard
4. **Check** http://localhost:5203/swagger for API docs
5. **Try** a fraud prediction via the UI

---

## 💾 Database Access

Want to access the database directly?

**Connection Details:**
- Server: `localhost:1433` or `localhost,1433`
- Username: `sa`
- Password: `FraudGuard@2024!`
- Database: `FraudDB`

**Using SQL Server Management Studio:**
1. Open SSMS
2. Connect to: `localhost,1433`
3. Use credentials above

**Using Docker:**
```bash
docker-compose exec database /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!" -Q "SELECT * FROM [dbo].[Users]"
```

---

## 📁 Project Structure

```
PFA_Project-main/
├── FraudDetectionAPI/       ← Backend API code
├── FraudDetectionML/        ← ML Service code
├── FraudDetectionUI/        ← Frontend code
├── docker-compose.yml       ← Full setup config
└── docker-compose.simple.yml ← Simple setup config
```

---

## 🆘 Still Having Issues?

1. Check the [COMPLETE_DOCKER_SETUP.md](COMPLETE_DOCKER_SETUP.md) file for detailed troubleshooting
2. View detailed logs: `docker-compose logs -f`
3. Make sure Docker Desktop is running
4. Ensure you have 10GB+ disk space free
5. Try: `docker-compose down -v && docker-compose up --build`

---

## 📚 Full Documentation

For detailed information, see:
- [COMPLETE_DOCKER_SETUP.md](COMPLETE_DOCKER_SETUP.md) - Complete guide with troubleshooting
- [DOCKER_SETUP.md](DOCKER_SETUP.md) - Quick reference
- [QUICK_START.md](QUICK_START.md) - Traditional (non-Docker) setup

---

**Happy Coding! 🎉**
