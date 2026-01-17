# ⚡ FRAUDGUARD - COMPLETE COMMANDS CHEAT SHEET

**All commands you need to run FraudGuard with all services**

---

## 🚀 STARTUP COMMANDS

### **START EVERYTHING (Full Stack)**

```bash
# Option 1: Windows startup script (easiest)
.\START_FRAUDGUARD.bat

# Option 2: Mac/Linux startup script
chmod +x START_FRAUDGUARD.sh
./START_FRAUDGUARD.sh

# Option 3: Manual with full monitoring services
docker-compose up --build

# Option 4: Background mode (no logs visible)
docker-compose up -d --build

# Option 5: Rebuild specific service
docker-compose build api && docker-compose up -d api
docker-compose build ml && docker-compose up -d ml
docker-compose build ui && docker-compose up -d ui
```

---

### **What Gets Started**

```
✅ fraudguard-db          → SQL Server Database
✅ fraudguard-redis       → Caching Service
✅ fraudguard-zookeeper   → Kafka Coordinator
✅ fraudguard-kafka       → Message Queue
✅ fraudguard-kafka-ui    → Kafka Management
✅ fraudguard-api         → ASP.NET Core API
✅ fraudguard-ml          → Python ML Service
✅ fraudguard-ui          → Angular Dashboard
✅ fraudguard-prometheus  → Metrics Collector
✅ fraudguard-grafana     → Dashboards
```

---

## 🔍 STATUS & LOGS COMMANDS

### **Check All Services Status**

```bash
# See all running containers
docker-compose ps

# See status with more details
docker-compose ps -a

# See only healthy containers
docker-compose ps | grep "healthy"

# Get detailed JSON output
docker-compose ps --format json
```

---

### **View Logs**

```bash
# ALL logs combined
docker-compose logs

# Follow live logs (real-time)
docker-compose logs -f

# Last 100 lines
docker-compose logs --tail=100

# Specific service logs
docker-compose logs api
docker-compose logs ml
docker-compose logs database
docker-compose logs kafka
docker-compose logs redis
docker-compose logs prometheus
docker-compose logs grafana

# Follow specific service logs
docker-compose logs -f api
docker-compose logs -f kafka
docker-compose logs -f ml

# Logs from last hour
docker-compose logs --since 1h

# Logs between timestamps
docker-compose logs --since 2024-01-17T10:00:00 --until 2024-01-17T11:00:00
```

---

## 🛑 STOP & SHUTDOWN COMMANDS

### **Stop Services**

```bash
# Stop all services (keep containers, keep data)
docker-compose stop

# Stop specific service
docker-compose stop api
docker-compose stop kafka
docker-compose stop redis

# Stop and remove containers (keep data/volumes)
docker-compose down

# Stop, remove containers, AND delete data
docker-compose down -v

# Stop and remove containers + images
docker-compose down --rmi all

# Emergency stop (force kill)
docker-compose kill
```

---

## 🔄 RESTART COMMANDS

### **Restart Services**

```bash
# Restart all services
docker-compose restart

# Restart specific service
docker-compose restart api
docker-compose restart kafka
docker-compose restart redis
docker-compose restart ml
docker-compose restart ui
docker-compose restart database

# Restart after code changes (recommended)
docker-compose build api && docker-compose restart api
docker-compose build ml && docker-compose restart ml
docker-compose build ui && docker-compose restart ui

# Full rebuild and restart
docker-compose build && docker-compose restart
```

---

## 🔨 BUILD COMMANDS

### **Build Docker Images**

```bash
# Build all services
docker-compose build

# Build specific service
docker-compose build api
docker-compose build ml
docker-compose build ui

# Build without cache (fresh build)
docker-compose build --no-cache

# Build with progress output
docker-compose build --progress=plain
```

---

## 📊 MONITORING & INSPECTION COMMANDS

### **Container Info**

```bash
# Get container IP address
docker-compose exec api hostname -I

# See resource usage (CPU, Memory)
docker stats

# See container environment variables
docker-compose config

# Inspect specific container
docker-compose exec api env
docker-compose exec ml env
```

---

### **Service Health Checks**

```bash
# Check database health
docker-compose exec database /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!" -Q "SELECT 1"

# Check Redis health
docker-compose exec redis redis-cli ping

# Check Kafka health
docker-compose exec kafka kafka-broker-api-versions --bootstrap-server localhost:9092

# Check API health
curl http://localhost:5203/health

# Check ML health
curl http://localhost:5000/health
```

---

## 💾 DATABASE COMMANDS

### **Database Backup**

```bash
# Backup database
docker-compose exec database /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!" -Q "BACKUP DATABASE FraudDB TO DISK='/var/opt/mssql/backup/FraudDB.bak'"

# Create backup folder
docker-compose exec database mkdir -p /var/opt/mssql/backup

# List backups
docker-compose exec database ls /var/opt/mssql/backup
```

---

### **Database Access**

```bash
# Connect to database (SQL commands)
docker-compose exec database /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "FraudGuard@2024!"

# List all databases
SELECT name FROM sys.databases;

# See tables
USE FraudDB;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;

# See data
SELECT * FROM Users;
SELECT * FROM Transactions;
SELECT * FROM FraudAlerts;

# Exit
EXIT
```

---

## 🧠 KAFKA COMMANDS

### **List Topics**

```bash
docker-compose exec kafka kafka-topics \
  --list \
  --bootstrap-server localhost:9092
```

---

### **Create Topic (if needed)**

```bash
docker-compose exec kafka kafka-topics \
  --create \
  --topic my-topic \
  --bootstrap-server localhost:9092 \
  --partitions 1 \
  --replication-factor 1
```

---

### **View Messages**

```bash
# View all messages from beginning
docker-compose exec kafka kafka-console-consumer \
  --bootstrap-server localhost:9092 \
  --topic fraudguard-transactions \
  --from-beginning

# View only new messages
docker-compose exec kafka kafka-console-consumer \
  --bootstrap-server localhost:9092 \
  --topic fraudguard-fraud-alerts

# View with format
docker-compose exec kafka kafka-console-consumer \
  --bootstrap-server localhost:9092 \
  --topic fraudguard-transactions \
  --formatter kafka.tools.DefaultMessageFormatter \
  --property print.key=true \
  --property print.value=true
```

---

### **Send Test Message**

```bash
docker-compose exec kafka kafka-console-producer \
  --broker-list localhost:9092 \
  --topic fraudguard-transactions \
  --property "parse.key=true" \
  --property "key.separator=:" \
  
# Then type: key1:{"value":"test"}
```

---

## 🏠 REDIS COMMANDS

### **Connect to Redis**

```bash
docker-compose exec redis redis-cli
```

---

### **Redis Commands**

```bash
# Ping Redis
PING

# Get all keys
KEYS *

# Get specific key
GET fraudguard_user_123

# Set key
SET mykey myvalue

# Delete key
DEL mykey

# See all data
KEYS fraudguard_*

# Get memory info
INFO memory

# Flush cache (delete all data)
FLUSHALL

# See database size
DBSIZE

# Exit
EXIT
```

---

## 🧹 CLEANUP COMMANDS

### **Clean Up Docker Resources**

```bash
# Remove unused images
docker image prune -a

# Remove unused volumes
docker volume prune

# Remove unused networks
docker network prune

# Remove unused containers
docker container prune

# Remove ALL unused resources
docker system prune -a --volumes

# Show disk usage
docker system df
```

---

### **Delete Specific Volumes**

```bash
# List volumes
docker volume ls

# Remove volume
docker volume rm fraudguard-redis-data
docker volume rm fraudguard-kafka-data
docker volume rm fraudguard-sqlserver-data

# Remove all FraudGuard volumes
docker volume rm fraudguard-* -f
```

---

## 🌐 API COMMANDS

### **Test API Endpoints**

```bash
# Get health
curl http://localhost:5203/health

# Get users
curl -X GET http://localhost:5203/api/users/1

# Create transaction
curl -X POST http://localhost:5203/api/transactions \
  -H "Content-Type: application/json" \
  -d '{"amount": 100, "description": "Test"}'

# Swagger docs
curl http://localhost:5203/swagger/index.html
```

---

## 📈 PROMETHEUS COMMANDS

### **Query Metrics**

**Via HTTP:**
```bash
# Get all metrics
curl http://localhost:9090/api/v1/query?query=http_requests_total

# Get time series data
curl 'http://localhost:9090/api/v1/query_range?query=http_request_duration_seconds&start=1609459200&end=1609545600&step=3600'
```

**Via Web UI:**
1. Go to http://localhost:9090
2. Enter query in search box:
```
http_requests_total
http_request_duration_seconds
process_resident_memory_bytes
process_cpu_seconds_total
```

---

## 🎨 GRAFANA COMMANDS

### **Access Grafana**

```bash
# URL
http://localhost:3000

# Default credentials
Username: admin
Password: FraudGuard@2024

# Add data source to Prometheus
1. Click Settings → Data Sources
2. Add Prometheus
3. URL: http://prometheus:9090
4. Save
```

---

## 🔐 ENVIRONMENT VARIABLES

### **Check Configuration**

```bash
# View API config
docker-compose exec api env | grep -E "Redis|Kafka|ConnectionString"

# View ML config
docker-compose exec ml env | grep -E "Kafka|Redis"
```

---

## 📱 PORTS REFERENCE

```bash
# Test connectivity to ports
curl http://localhost:80          # UI
curl http://localhost:5203        # API
curl http://localhost:5000        # ML
curl http://localhost:1433        # Database
curl http://localhost:6379        # Redis
curl http://localhost:9092        # Kafka
curl http://localhost:2181        # Zookeeper
curl http://localhost:9090        # Prometheus
curl http://localhost:3000        # Grafana
curl http://localhost:8080        # Kafka UI
```

---

## 🐛 DEBUGGING COMMANDS

### **Troubleshoot Services**

```bash
# Check if containers are actually running
docker ps

# Get container ID
docker-compose ps | grep api

# Inspect container
docker inspect container_id

# Check network
docker network ls
docker network inspect fraudguard-network

# Check volumes
docker volume ls
docker volume inspect fraudguard-redis-data

# See what's listening on ports
netstat -ano | findstr :80
netstat -ano | findstr :5203
```

---

## 🔄 FULL REBUILD (Nuclear Option)

### **Start Fresh**

```bash
# STOP everything
docker-compose down

# REMOVE all volumes (delete data)
docker volume rm fraudguard-* -f

# REMOVE all images
docker rmi frauddetectionapi:latest frauddetectionml:latest frauddetectionui:latest -f

# START fresh
docker-compose up --build

# Wait 5 minutes for everything to initialize
```

---

## 📋 COMMON WORKFLOWS

### **Workflow 1: Development Changes**

```bash
# Edit code in FraudDetectionAPI/
# Then:
docker-compose build api
docker-compose restart api

# Check logs
docker-compose logs -f api
```

---

### **Workflow 2: Test New Feature**

```bash
# Run the system
docker-compose up -d --build

# Wait for healthy status
docker-compose ps

# Test manually
curl http://localhost:5203/api/test

# View live logs
docker-compose logs -f api
```

---

### **Workflow 3: Performance Testing**

```bash
# Monitor resources while running
docker stats

# Check response times in Prometheus
curl http://localhost:9090/api/v1/query?query=http_request_duration_seconds

# View graphs in Grafana
# http://localhost:3000
```

---

## 🎯 Quick Commands Reference

```bash
# START
docker-compose up -d --build

# STATUS
docker-compose ps

# LOGS
docker-compose logs -f api

# REBUILD
docker-compose build && docker-compose restart

# STOP
docker-compose stop

# DELETE
docker-compose down -v

# CLEAN
docker system prune -a --volumes
```

---

## 📚 Access Points After Startup

```
Application UI      → http://localhost
API Documentation  → http://localhost:5203/swagger
Prometheus          → http://localhost:9090
Grafana             → http://localhost:3000
Kafka UI            → http://localhost:8080
ML Health           → http://localhost:5000/health
Database            → localhost:1433 (requires SQL client)
```

---

## ✅ Commands Checklist

**Daily Use:**
- [ ] `docker-compose ps` - Check status
- [ ] `docker-compose logs -f api` - View logs
- [ ] `docker-compose restart api` - Restart service

**Maintenance:**
- [ ] `docker-compose build && docker-compose restart` - Full rebuild
- [ ] `docker system prune -a` - Clean up
- [ ] `docker-compose down -v` - Nuclear reset

**Monitoring:**
- [ ] http://localhost:3000 - Grafana dashboards
- [ ] http://localhost:9090 - Prometheus metrics
- [ ] http://localhost:8080 - Kafka messages

---

**Last Updated:** January 17, 2026  
**Project:** FraudGuard - Enterprise Fraud Detection Platform  
**Status:** All Services Active & Ready

---

*Save this file for quick reference!* ⭐
