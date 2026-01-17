# ✅ FraudGuard Deployment Checklist

**Complete walkthrough for deploying FraudGuard with Docker**

---

## Pre-Deployment Checklist

### ☐ System Requirements
- [ ] Operating System: Windows 10/11, macOS, or Linux
- [ ] RAM: At least 8GB (16GB recommended)
- [ ] Disk Space: At least 20GB free
- [ ] Internet Connection: Required for downloading Docker images
- [ ] Administrator access to install software

### ☐ Software Installation
- [ ] Docker Desktop installed from https://www.docker.com/products/docker-desktop
- [ ] Git installed from https://git-scm.com/download
- [ ] Command line/terminal access available
- [ ] Editor (VS Code recommended) installed

### ☐ Verify Installations

**Run these commands in terminal/PowerShell:**

```bash
docker --version
docker-compose --version
git --version
```

**Expected Output:**
- Docker version should be 20.10 or higher
- Docker Compose should be 2.0 or higher
- Git should be 2.30 or higher

---

## Deployment Steps

### Step 1: Prepare System
- [ ] Close unnecessary applications (to free RAM)
- [ ] Ensure Docker Desktop is running
  - Windows/Mac: Check system tray for Docker icon
  - Linux: Run `sudo systemctl start docker`
- [ ] Check free disk space: 20GB+ required

### Step 2: Get the Code
- [ ] Open terminal/PowerShell
- [ ] Navigate to desired directory
  ```bash
  cd Desktop  # or another location
  ```
- [ ] Clone the repository
  ```bash
  git clone <repository-url>
  cd PFA_Project-main
  ```
- [ ] Verify files exist
  ```bash
  # Should see these files:
  # docker-compose.yml
  # docker-compose.simple.yml
  # FraudDetectionAPI/
  # FraudDetectionML/
  # FraudDetectionUI/
  ```

### Step 3: Start the Application

**Option A: Simple Setup (Recommended)**
- [ ] Run this command:
  ```bash
  docker-compose -f docker-compose.simple.yml up --build
  ```
- [ ] Wait for output showing all containers are healthy
- [ ] Expected time: 2-3 minutes
- [ ] Look for:
  ```
  fraudguard-db is healthy
  fraudguard-api is ready
  fraudguard-ml is ready
  fraudguard-ui is running
  ```

**Option B: Full Setup (With Monitoring)**
- [ ] Run this command:
  ```bash
  docker-compose up --build
  ```
- [ ] Wait for all services to start
- [ ] Expected time: 3-4 minutes

### Step 4: Verify Services Are Running
- [ ] Open new terminal/PowerShell window
- [ ] Run:
  ```bash
  docker-compose ps
  ```
- [ ] Verify output shows these containers as "Up":
  - [ ] fraudguard-db (database)
  - [ ] fraudguard-api (API)
  - [ ] fraudguard-ml (ML service)
  - [ ] fraudguard-ui (Frontend)
  - [ ] fraudguard-redis (Cache)
  - [ ] Others (if using full setup)

### Step 5: Access the Application
- [ ] Open web browser
- [ ] Navigate to: http://localhost
- [ ] Should see FraudGuard login page
- [ ] Try these URLs:
  - [ ] http://localhost - Main app
  - [ ] http://localhost:5203/swagger - API documentation
  - [ ] http://localhost:5000/health - ML service health

### Step 6: Login & Test
- [ ] Click "Login" on home page
- [ ] Enter credentials:
  - Email: `admin@fraudguard.com`
  - Password: `Admin@123`
- [ ] Click "Sign In"
- [ ] Should see admin dashboard
- [ ] Verify these pages load:
  - [ ] Dashboard - Shows statistics
  - [ ] Transactions - Lists transactions
  - [ ] Fraud Alerts - Shows alerts
  - [ ] Analytics - Displays charts

### Step 7: Test Key Features
- [ ] Navigate to "Transactions" section
- [ ] [ ] Should see sample transactions
- [ ] Check "Fraud Alerts"
- [ ] [ ] Should see any flagged transactions
- [ ] Go to "Analytics"
- [ ] [ ] Should see graphs and statistics

---

## Post-Deployment Verification

### ✅ All Systems Operational
- [ ] Frontend loads without errors (http://localhost)
- [ ] Can login with provided credentials
- [ ] API Swagger page shows all endpoints (http://localhost:5203/swagger)
- [ ] ML health check passes (http://localhost:5000/health)
- [ ] Database contains sample data
- [ ] Dashboard shows statistics

### ✅ Services Communication
- [ ] API communicates with database
- [ ] ML service responds to requests
- [ ] Frontend connects to API
- [ ] Cache (Redis) is working
- [ ] Kafka (if full setup) is operational

### ✅ Data Integrity
- [ ] Sample users exist in database
- [ ] Sample transactions are seeded
- [ ] Admin account can be logged into
- [ ] User data is retrievable

---

## Troubleshooting During Deployment

### ❌ Docker Not Found
**Problem:** "docker: command not found"
- [ ] Install Docker Desktop: https://www.docker.com/products/docker-desktop
- [ ] Restart terminal after installation
- [ ] Run `docker --version` to verify

### ❌ Port Already in Use
**Problem:** "bind: address already in use" on port 80, 5203, etc.

**Windows Solution:**
```powershell
netstat -ano | findstr :80
taskkill /PID <number> /F
```

**Mac/Linux Solution:**
```bash
lsof -i :80
kill -9 <PID>
```

- [ ] Free the port using commands above
- [ ] Try starting again

### ❌ Containers Exit Immediately
**Problem:** Containers start but immediately stop

- [ ] Check logs: `docker-compose logs api`
- [ ] Look for error messages
- [ ] Common fixes:
  ```bash
  docker-compose down
  docker-compose build --no-cache
  docker-compose up --build
  ```

### ❌ Database Not Initializing
**Problem:** API can't connect to database

- [ ] Check database status: `docker-compose ps database`
- [ ] Restart database: `docker-compose restart database`
- [ ] Wait 60 seconds for SQL Server to fully initialize
- [ ] Check logs: `docker-compose logs database`

### ❌ Frontend Shows Blank Page
**Problem:** http://localhost shows nothing or 404

- [ ] Check UI logs: `docker-compose logs ui`
- [ ] Rebuild UI:
  ```bash
  docker-compose build ui
  docker-compose restart ui
  ```
- [ ] Clear browser cache (Ctrl+F5)

### ❌ Out of Disk Space
**Problem:** "No space left on device"

- [ ] Check available space
- [ ] Clean up Docker:
  ```bash
  docker system prune -a --volumes
  ```
- [ ] This removes unused images and containers

### ❌ Memory Issues
**Problem:** System runs slow, containers crashing

- [ ] Check Docker memory limit in Docker Desktop settings
- [ ] Recommend: 8GB+ RAM allocated to Docker
- [ ] Close unnecessary applications
- [ ] Restart Docker Desktop

### ❌ Network Connectivity
**Problem:** Services can't reach each other

- [ ] Verify network exists:
  ```bash
  docker network ls
  docker network inspect fraudguard-network
  ```
- [ ] Check service names are correct in docker-compose.yml
- [ ] Restart all services:
  ```bash
  docker-compose restart
  ```

---

## Daily Operation

### Starting the Application
```bash
# Navigate to project directory
cd PFA_Project-main

# Start services in background
docker-compose -f docker-compose.simple.yml up -d --build

# Or for full setup:
docker-compose up -d --build

# Wait 1-2 minutes, then access http://localhost
```

### Checking Status
```bash
# See all containers
docker-compose ps

# View logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f api
```

### Stopping the Application
```bash
# Stop all containers (keeps data)
docker-compose stop

# Stop and remove containers (keeps data)
docker-compose down

# Stop and DELETE all data
docker-compose down -v
```

### After Code Changes
```bash
# Rebuild affected services
docker-compose build api    # After API code changes
docker-compose build ml     # After ML code changes
docker-compose build ui     # After UI code changes

# Restart services
docker-compose restart api
```

---

## Database Management

### Accessing the Database

**Via SQL Server Management Studio:**
- Server: `localhost,1433` (or `localhost:1433`)
- Username: `sa`
- Password: `FraudGuard@2024!`
- Database: `FraudDB`

**Via Command Line:**
```bash
docker-compose exec database /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!"

# Then run SQL commands:
USE FraudDB;
SELECT * FROM [dbo].[Users];
```

### Backup Database
```bash
# Create backup
docker-compose exec database /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!" -Q "BACKUP DATABASE [FraudDB] TO DISK=N'/var/opt/mssql/backup/FraudDB_backup.bak'"
```

### Reset Database
```bash
# WARNING: This deletes all data!
docker-compose down -v
docker-compose up --build
```

---

## Performance Optimization

### Recommended Docker Desktop Settings
- **Memory:** 8GB or more
- **CPUs:** 4 or more
- **Disk Space:** 20GB+
- **Swap:** 2GB or more

### Monitor Resource Usage
```bash
# Real-time resource monitoring
docker stats

# Check disk usage
docker system df
```

### Cleanup Old Resources
```bash
# Remove unused images
docker image prune -a

# Remove unused volumes
docker volume prune

# Remove unused networks
docker network prune

# Complete cleanup
docker system prune -a --volumes
```

---

## Backup & Restore

### Backup Everything
```bash
# Backup database
docker-compose exec database /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!" -Q "BACKUP DATABASE [FraudDB] TO DISK=N'/var/opt/mssql/backup/FraudDB_backup.bak'"

# Backup volumes
docker run --rm -v fraudguard-sqlserver-data:/data -v $(pwd):/backup alpine tar czf /backup/sqlserver-backup.tar.gz -C /data .
```

### Restore Database
```bash
# Copy backup file to container
docker cp backup.bak fraudguard-db:/var/opt/mssql/backup/

# Restore
docker-compose exec database /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!" -Q "RESTORE DATABASE [FraudDB] FROM DISK=N'/var/opt/mssql/backup/backup.bak'"
```

---

## Success Criteria

✅ **Deployment is successful when:**

- [ ] All containers are running (`docker-compose ps` shows "Up" for all)
- [ ] Frontend accessible at http://localhost
- [ ] Can login with provided credentials
- [ ] API Swagger accessible at http://localhost:5203/swagger
- [ ] ML health check passes at http://localhost:5000/health
- [ ] Database contains sample data
- [ ] Dashboard loads and shows statistics
- [ ] No errors in logs (`docker-compose logs`)
- [ ] Network connectivity verified between services
- [ ] System responsive and stable

---

## Next Steps

After successful deployment:

1. **Configure Custom Rules** - Set up fraud detection rules in Admin panel
2. **Import Real Data** - Load production data into database
3. **Train ML Model** - Retrain model with your own data
4. **Monitor Performance** - Check Grafana dashboards at http://localhost:3000
5. **Setup Backups** - Implement regular database backups
6. **Configure Alerts** - Setup email/webhook notifications
7. **User Training** - Train team on using the platform

---

## Support

- **Issue with Docker?** Check [COMPLETE_DOCKER_SETUP.md](COMPLETE_DOCKER_SETUP.md)
- **Command reference?** See [DOCKER_COMMANDS_REFERENCE.md](DOCKER_COMMANDS_REFERENCE.md)
- **API Documentation?** Visit http://localhost:5203/swagger (after starting)
- **Database help?** See Database Management section above

---

**Deployment Date:** ________________  
**Deployed By:** ________________  
**Notes:** ________________

---

*Last Updated: January 17, 2026*
