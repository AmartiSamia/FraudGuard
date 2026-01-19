# 🚀 FraudGuard - Complete Startup Guide

## **Status: Docker Service Stopped**

Your Docker service is currently **STOPPED**. Here's how to start the entire FraudGuard system:

---

## **Step 1: Start Docker Desktop** 

### **Option A: GUI (Easiest)**
1. Click **Start Menu** on Windows
2. Search for **"Docker Desktop"**
3. Click to launch
4. Wait for Docker icon to appear in system tray (bottom right)
5. Proceed to Step 2 below

### **Option B: PowerShell (If not starting)**
```powershell
# Run as Administrator:
Start-Service -Name "Docker for Windows"

# Wait 30 seconds for Docker to start
Start-Sleep -Seconds 30

# Check if running:
Get-Service | Where-Object {$_.Name -like "*docker*"}
# Should show Status = Running
```

---

## **Step 2: Verify Docker is Running**

```powershell
docker --version
docker ps
```

If you see version info and a container list → Docker is ready! ✅

---

## **Step 3: Start FraudGuard System**

Once Docker is running:

```powershell
cd Desktop\PFA_Project-main\PFA_Project-main

# Start all 10 services:
docker-compose up -d

# This will:
# - Pull images if needed (first time)
# - Create containers
# - Start services
# - Takes 2-3 minutes total

# Check status:
docker-compose ps
```

---

## **Step 4: Wait for Everything to Start**

Services startup order:

```
1. SQL Server (30-60 seconds) ⏱️
   └─ Initializing database
   
2. Redis (5 seconds) ⚡
   └─ Cache ready
   
3. Zookeeper (10 seconds) 🐘
   └─ Kafka coordinator
   
4. Kafka (15 seconds) ⚙️
   └─ Message broker ready
   
5. API (10 seconds) 🔷
   └─ Waiting for DB...
   
6. ML Service (5 seconds) 🐍
   └─ XGBoost model loaded
   
7. Frontend (3 seconds) 🅰️
   └─ Angular SPA served
   
8. Prometheus (5 seconds) 📈
   └─ Metrics collector
   
9. Grafana (5 seconds) 📉
   └─ Dashboard ready
   
10. Kafka UI (3 seconds) 🖥️
    └─ Message browser ready

TOTAL: ~3-5 minutes
```

---

## **Step 5: Verify All Services Running**

```powershell
docker-compose ps

# Should show all 10 services with status "Up"
```

Expected output:
```
NAME                    STATUS
fraudguard-database     Up 2 minutes (healthy)
fraudguard-redis        Up 2 minutes (healthy)
fraudguard-zookeeper    Up 2 minutes (healthy)
fraudguard-kafka        Up 2 minutes (healthy)
fraudguard-api          Up 2 minutes (healthy)
fraudguard-ml           Up 2 minutes (healthy)
fraudguard-ui           Up 2 minutes (healthy)
fraudguard-prometheus   Up 2 minutes (healthy)
fraudguard-grafana      Up 2 minutes (healthy)
kafka-ui                Up 2 minutes
```

---

## **Step 6: Access Everything**

Once all services show "Up" and "healthy", open your browser:

| Service | URL | Credentials |
|---------|-----|-------------|
| **Frontend** | http://localhost | - |
| **API** | http://localhost:5203 | - |
| **API Docs** | http://localhost:5203/swagger | - |
| **Grafana** | http://localhost:3000 | admin / FraudGuard@2024 |
| **Prometheus** | http://localhost:9090 | - |
| **Kafka UI** | http://localhost:8080 | - |

---

## **Step 7: Test the System**

### **1. Open Frontend**
```
http://localhost
```
Should see: Angular dashboard ✅

### **2. Test API**
```powershell
Invoke-RestMethod http://localhost:5203/health
```
Should return: `{ "status": "healthy" }` ✅

### **3. Login to Grafana**
```
http://localhost:3000
Username: admin
Password: FraudGuard@2024
```
Should see: Beautiful dashboards ✅

### **4. Create Test Transaction (via API)**
```powershell
$transaction = @{
    amount = 500
    merchant = "Amazon"
    device = "iPhone"
    location = "New York"
} | ConvertTo-Json

Invoke-RestMethod -Uri http://localhost:5203/api/transactions `
  -Method POST `
  -ContentType "application/json" `
  -Body $transaction
```

Should return: Transaction ID ✅

---

## **Troubleshooting**

### **Issue: Docker won't start**
```powershell
# Check if WSL2 is installed:
wsl --list --verbose

# If not, install WSL2:
# Open PowerShell as Admin and run:
wsl --install
```

### **Issue: Port already in use**
```powershell
# Find what's using port 80:
netstat -ano | findstr :80

# Kill process (replace PID):
taskkill /PID 1234 /F

# Then retry docker-compose up
```

### **Issue: Containers keep crashing**
```powershell
# Check logs:
docker-compose logs api
docker-compose logs ml
docker-compose logs database

# Restart all:
docker-compose restart

# Or hard reset:
docker-compose down -v
docker-compose up -d
```

### **Issue: Can't connect to database**
```powershell
# Check database is healthy:
docker-compose ps | findstr database

# Check logs:
docker-compose logs database

# Wait longer (SQL Server takes 60+ seconds first time)
Start-Sleep -Seconds 60
docker-compose restart api
```

---

## **What's Running?**

✅ **10 Docker Containers:**
- 🗄️ SQL Server 2022 (Database)
- 🔴 Redis 7 (Cache)
- 🐘 Zookeeper (Kafka coordinator)
- ⚙️ Kafka 7.5 (Message broker)
- 🔷 ASP.NET Core 8 API
- 🐍 Python Flask ML
- 🅰️ Angular 17 Frontend
- 📈 Prometheus (Metrics)
- 📉 Grafana (Dashboards)
- 🖥️ Kafka UI (Message browser)

✅ **All Ports Open:**
- 80 (Frontend)
- 3000 (Grafana)
- 5203 (API)
- 5000 (ML)
- 6379 (Redis)
- 9090 (Prometheus)
- 9092 (Kafka)
- 8080 (Kafka UI)
- 1433 (Database)
- 2181 (Zookeeper)

✅ **10+ Features Ready:**
- Real-time fraud detection
- ML predictions (98% accuracy)
- Live dashboards
- Message streaming
- Caching
- Monitoring
- Audit trails
- Full API

---

## **Quick Command Reference**

```powershell
# Start everything:
docker-compose up -d

# Check status:
docker-compose ps

# View logs (all):
docker-compose logs -f

# View specific service logs:
docker-compose logs -f api
docker-compose logs -f ml
docker-compose logs -f database

# Restart all:
docker-compose restart

# Restart specific service:
docker-compose restart api

# Stop everything (keep data):
docker-compose stop

# Stop and remove containers (keep data):
docker-compose down

# Full reset (delete everything):
docker-compose down -v

# Resource usage:
docker stats
```

---

## **Success! 🎉**

Once everything is running:

1. **Frontend**: http://localhost ✅
2. **Grafana**: http://localhost:3000 ✅
3. **API Docs**: http://localhost:5203/swagger ✅
4. **Prometheus**: http://localhost:9090 ✅
5. **Kafka UI**: http://localhost:8080 ✅

Your **FraudGuard Enterprise Fraud Detection System** is live! 🚀

---

## **Next Steps**

1. **Explore Grafana Dashboards**
   - Executive Summary
   - Real-Time Operations
   - Fraud Analytics
   - Infrastructure Monitoring

2. **Test Fraud Detection**
   - Create transactions via API
   - See real-time fraud scores
   - Check ML predictions
   - View alerts in dashboard

3. **Monitor Metrics**
   - API response times
   - Cache hit rates
   - ML inference latency
   - System health

4. **Review Documentation**
   - README.md
   - TECHNOLOGY_EXPLANATION.md
   - QUICK_SETUP_GUIDE.md
   - USE_CASES.md

---

**Questions? Check the documentation files or review TECHNOLOGY_EXPLANATION.md for complete details!**
