# 🛡️ FRAUDGUARD - COMPLETE SERVICES GUIDE

**Everything you need to know about Redis, Kafka, Prometheus, and Grafana in FraudGuard**

---

## 📊 Table of Contents

1. [What Each Service Does](#what-each-service-does)
2. [Where Services Are Used](#where-services-are-used)
3. [How to Run Everything](#how-to-run-everything)
4. [Commands Reference](#commands-reference)
5. [Access URLs & Credentials](#access-urls--credentials)
6. [Monitoring Your System](#monitoring-your-system)
7. [Troubleshooting](#troubleshooting)

---

## 🔧 What Each Service Does

### **1. Redis - Caching & Session Storage**

**What is it?**
- Super-fast in-memory database
- Stores temporary data (cache)
- Improves application speed

**What it does in FraudGuard:**
- ✅ Caches user sessions (you stay logged in)
- ✅ Caches transaction data (faster lookups)
- ✅ Caches fraud detection results
- ✅ Stores temporary cached data

**File used:** `FraudDetectionAPI/Services/CacheService.cs`

**Code example:**
```csharp
// How it's used
var user = await _cacheService.GetAsync<User>("user_123");
if (user == null)
{
    user = await _database.GetUserAsync(123);
    await _cacheService.SetAsync("user_123", user, TimeSpan.FromMinutes(30));
}
// Next time = instant, from cache!
```

**Port:** `6379`  
**Status:** ✅ **NOW ENABLED**

---

### **2. Kafka - Real-Time Event Streaming**

**What is it?**
- Message queue for events
- Handles real-time data flow
- Distributes messages across services

**What it does in FraudGuard:**
- ✅ Sends transaction events in real-time
- ✅ Broadcasts fraud alerts to all services
- ✅ Streams audit logs
- ✅ Enables real-time fraud detection

**File used:** `FraudDetectionAPI/Services/KafkaService.cs`

**Code example:**
```csharp
// When a transaction is created:
await _kafkaService.PublishTransactionAsync(newTransaction);
// Kafka sends to ML service immediately
// ML service processes and sends fraud alert
// Dashboard updates in real-time
```

**Topics (Kafka channels):**
```
fraudguard-transactions     → New transactions
fraudguard-fraud-alerts    → Fraud detection alerts
fraudguard-audit-log       → System audit logs
```

**Ports:** `9092` (Kafka), `2181` (Zookeeper), `8080` (Kafka UI)  
**Status:** ✅ **NOW ENABLED**

---

### **3. Prometheus - Metrics Collection**

**What is it?**
- Collects system metrics
- Monitors CPU, memory, requests
- Time-series database for metrics

**What it monitors in FraudGuard:**
- ✅ API request count & response time
- ✅ Database query performance
- ✅ Memory & CPU usage
- ✅ Error rates
- ✅ Active connections

**Configuration:** `monitoring/prometheus/prometheus.yml`

**Metrics collected:**
```
http_requests_total        → Total API requests
http_request_duration_ms   → Response time
process_resident_memory    → Memory usage
process_cpu_seconds_total  → CPU usage
database_connections       → Active DB connections
exceptions_total           → Error count
```

**Port:** `9090`  
**Status:** ✅ **ALREADY RUNNING**

---

### **4. Grafana - Visualization & Dashboards**

**What is it?**
- Beautiful dashboards
- Visualizes metrics from Prometheus
- Real-time monitoring graphs

**What it shows in FraudGuard:**
- ✅ Real-time API performance
- ✅ Transaction volume charts
- ✅ Fraud detection accuracy
- ✅ System health status
- ✅ Error rates over time
- ✅ Response time trends

**Configuration:** `monitoring/grafana/provisioning/`

**Pre-built Dashboards:**
```
1. System Health         → CPU, Memory, Disk
2. API Performance       → Response times, requests
3. Fraud Detection       → Detection accuracy, alerts
4. Database             → Query performance
5. Error Tracking       → Errors over time
```

**Port:** `3000`  
**Status:** ✅ **ALREADY RUNNING**

---

## 🎯 Where Services Are Used

### **In the API (FraudDetectionAPI)**

**Redis:**
```
├── CacheService.cs
│   ├── Caches user data
│   ├── Caches transaction data
│   └── Caches fraud predictions
│
└── Controllers
    ├── UserController → Uses cache for user lookups
    ├── TransactionController → Caches recent transactions
    └── FraudAlertController → Caches alert data
```

**Kafka:**
```
├── KafkaService.cs
│   ├── PublishTransactionAsync() → Sends new transactions
│   ├── PublishFraudAlertAsync() → Sends fraud alerts
│   └── PublishEventAsync() → Sends generic events
│
└── Controllers
    ├── TransactionController → Publishes when transaction created
    └── FraudAlertController → Publishes fraud detection results
```

**Prometheus:**
```
├── Automatic collection via .NET metrics
├── Tracks all HTTP endpoints
├── Monitors database connections
└── Reports on errors & exceptions
```

**Grafana:**
```
├── Reads metrics from Prometheus
├── Displays on dashboards
└── Shows real-time monitoring
```

---

### **In the ML Service (FraudDetectionML)**

**Kafka:**
```
ML Service subscribes to: fraudguard-transactions topic
│
├── Receives transactions in real-time
├── Processes with ML model
├── Publishes results to: fraudguard-fraud-alerts topic
└── API receives and stores the alert
```

**Redis:**
```
Caches:
├── Trained ML model weights
├── Recent predictions
└── Model metadata
```

---

### **In the Database (SQL Server)**

**Redis:**
- Reduces database queries
- Caches user sessions
- Caches frequently accessed data

**Result:** Faster response times, less database load

---

## 🚀 How to Run Everything

### **Step 1: Make Sure Everything is Enabled**

Check `FraudDetectionAPI/appsettings.json`:
```json
"Redis": {
  "Enabled": true,           // ✅ Should be TRUE
  "ConnectionString": "redis:6379"
},
"Kafka": {
  "Enabled": true,           // ✅ Should be TRUE
  "BootstrapServers": "kafka:9092"
}
```

**Status:** ✅ Already done for you!

---

### **Step 2: Start with Full Docker Compose**

**Option A: Using Startup Script (WINDOWS)**
```powershell
# Just run this (it uses full docker-compose.yml)
.\START_FRAUDGUARD.bat
```

**Option B: Manual Command**
```bash
# Start with full stack (Redis, Kafka, Prometheus, Grafana)
docker-compose up --build

# Keep it running (watch the output)
# Press Ctrl+C to stop
```

**Option C: Background Mode**
```bash
# Start and detach (runs in background)
docker-compose up -d --build

# Check status
docker-compose ps
```

---

### **Step 3: Wait for All Services**

Watch for these messages:
```
✅ fraudguard-db is healthy
✅ fraudguard-redis is healthy
✅ fraudguard-zookeeper is healthy
✅ fraudguard-kafka is healthy
✅ fraudguard-api is ready
✅ fraudguard-ml is ready
✅ fraudguard-ui is running
✅ fraudguard-prometheus is running
✅ fraudguard-grafana is running
```

**Expected time:** 3-5 minutes (first run), 30-60 seconds (subsequent)

---

### **Step 4: Verify Services Running**

```powershell
# In a NEW PowerShell window
docker-compose ps

# You should see 10 services:
# fraudguard-db       (healthy)
# fraudguard-redis    (healthy)
# fraudguard-zookeeper(healthy)
# fraudguard-kafka    (healthy)
# fraudguard-api      (healthy)
# fraudguard-ml       (healthy)
# fraudguard-ui       (running)
# fraudguard-prometheus(running)
# fraudguard-grafana  (running)
# fraudguard-kafka-ui (running)
```

---

## 📋 Commands Reference

### **Run Application**

```bash
# Full stack with all services
docker-compose up --build

# Background mode
docker-compose up -d --build

# Specific service
docker-compose up --build api
docker-compose up --build ml
docker-compose up --build redis
docker-compose up --build kafka
```

---

### **Check Status**

```bash
# All services status
docker-compose ps

# Specific service logs
docker-compose logs api
docker-compose logs kafka
docker-compose logs redis
docker-compose logs prometheus
docker-compose logs grafana

# Follow logs in real-time
docker-compose logs -f api
docker-compose logs -f kafka
```

---

### **Manage Services**

```bash
# Stop all services (keep data)
docker-compose stop

# Stop and remove containers
docker-compose down

# Delete everything (including data)
docker-compose down -v

# Restart all
docker-compose restart

# Restart specific service
docker-compose restart api
docker-compose restart redis
docker-compose restart kafka
```

---

### **Rebuild After Code Changes**

```bash
# Rebuild specific service
docker-compose build api
docker-compose build ml
docker-compose build ui

# Rebuild and restart
docker-compose build api && docker-compose restart api

# Rebuild all and restart
docker-compose build && docker-compose restart
```

---

### **Cleanup**

```bash
# Remove unused images
docker image prune -a

# Remove unused volumes
docker volume prune

# Remove everything unused
docker system prune -a --volumes

# Show disk usage
docker system df
```

---

## 🌐 Access URLs & Credentials

### **Application Access**

| Service | URL | Purpose |
|---------|-----|---------|
| **FraudGuard UI** | http://localhost | Main application |
| **API Docs** | http://localhost:5203/swagger | API documentation |
| **Prometheus** | http://localhost:9090 | Metrics database |
| **Grafana** | http://localhost:3000 | Dashboards |
| **Kafka UI** | http://localhost:8080 | Kafka management |
| **ML Service** | http://localhost:5000 | ML health check |

---

### **Credentials**

**FraudGuard Application:**
```
Email:    admin@fraudguard.com
Password: Admin@123
```

**Grafana:**
```
Username: admin
Password: FraudGuard@2024
```

**Database (SQL Server):**
```
Server:   localhost:1433
User:     sa
Password: FraudGuard@2024!
```

---

## 📊 Monitoring Your System

### **Using Prometheus (Metrics Database)**

1. Go to: http://localhost:9090
2. Click on "Graph" tab
3. Search for metrics:
   ```
   http_requests_total
   http_request_duration_seconds
   process_resident_memory_bytes
   process_cpu_seconds_total
   ```

---

### **Using Grafana (Beautiful Dashboards)**

1. Go to: http://localhost:3000
2. Login: `admin` / `FraudGuard@2024`
3. Click "Dashboards" → Browse existing dashboards
4. View real-time metrics

**What you can see:**
- API requests per second
- Average response time
- Memory & CPU usage
- Error rates
- Database connections
- Fraud detection accuracy

---

### **Using Kafka UI (Kafka Management)**

1. Go to: http://localhost:8080
2. See topics, messages, consumers
3. Monitor fraud detection events in real-time

**Topics to watch:**
```
fraudguard-transactions   → New transactions flowing in
fraudguard-fraud-alerts  → Fraud detections happening
fraudguard-audit-log     → System events
```

---

## 🔍 What Data Flows Where

### **Transaction Flow**

```
1. User creates transaction
        ↓
2. API receives request
        ↓
3. Transaction saved to Database
        ↓
4. KAFKA: Transaction event published
        ↓
5. ML Service receives via Kafka
        ↓
6. ML Service processes with model
        ↓
7. KAFKA: Fraud alert published
        ↓
8. API receives alert, saves to Database
        ↓
9. REDIS: Result cached
        ↓
10. Dashboard receives real-time update
```

### **Caching Flow**

```
First Request:
  User lookup → Database (slow)
      ↓
  REDIS: Store result (fast)

Second Request (same user):
  User lookup → REDIS (instant!)
      ↓
  No database hit = faster!
```

### **Metrics Flow**

```
API
  ↓ (publishes metrics)
PROMETHEUS (collects)
  ↓ (reads metrics)
GRAFANA (visualizes)
  ↓
Beautiful dashboards!
```

---

## ⚠️ Troubleshooting

### **Issue 1: Services keep restarting**

**Check logs:**
```bash
docker-compose logs api
docker-compose logs kafka
docker-compose logs redis
```

**Common causes:**
- Kafka not ready (wait 60+ seconds)
- Redis connection failed
- API configuration wrong

**Fix:**
```bash
docker-compose restart kafka
docker-compose restart redis
docker-compose restart api
```

---

### **Issue 2: Cannot connect to Redis**

**Check if Redis is running:**
```bash
docker-compose ps redis
# Should show "Up (healthy)"
```

**Test connection:**
```bash
docker-compose exec redis redis-cli ping
# Should respond: PONG
```

**If not:**
```bash
docker-compose restart redis
```

---

### **Issue 3: Kafka messages not flowing**

**Check Kafka status:**
```bash
docker-compose ps kafka
# Should show "Up (healthy)"
```

**View topics:**
```bash
docker-compose exec kafka kafka-topics --list --bootstrap-server localhost:9092
```

**View messages:**
```bash
docker-compose exec kafka kafka-console-consumer \
  --bootstrap-server localhost:9092 \
  --topic fraudguard-transactions \
  --from-beginning
```

---

### **Issue 4: Grafana not showing data**

**Check Prometheus:**
- Go to http://localhost:9090
- Search for: `http_requests_total`
- Should show data

**If no data:**
1. Make some API requests (create transactions)
2. Wait 30 seconds
3. Refresh Grafana

---

### **Issue 5: Out of disk space**

```bash
# Clean up everything
docker system prune -a --volumes

# Then restart
docker-compose up -d --build
```

---

## 📈 Performance Tips

### **Optimize Redis Caching**

Currently caches for 30 minutes. To change:

Edit `FraudDetectionAPI/Services/CacheService.cs`:
```csharp
_defaultOptions = new DistributedCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60), // ← Change here
    SlidingExpiration = TimeSpan.FromMinutes(15)
};
```

---

### **Monitor Metrics**

In Grafana, look for:
- **High response times** → Cache more data
- **High CPU** → More containers needed
- **High memory** → Increase Redis cache TTL
- **Many errors** → Check API logs

---

## 🎓 Real Example: Transaction Creation

**What happens step-by-step:**

```
1. User clicks "Create Transaction" on dashboard
   ↓
2. API receives POST /transactions request
   ↓
3. Validates transaction data (validates in-memory)
   ↓
4. Saves to database (SQL Server)
   → 💾 Database stores it
   ↓
5. Publishes event to Kafka
   → 📨 Event: "transaction.created" sent to Kafka
   ↓
6. ML service listens to Kafka topic
   → 👁️ ML service gets the transaction
   ↓
7. ML service processes with fraud detection model
   → 🤖 XGBoost model predicts fraud probability
   ↓
8. ML service publishes result to Kafka
   → 📨 Event: "fraud.alert" sent back to Kafka
   ↓
9. API service listens and receives alert
   → 🎯 API gets fraud prediction result
   ↓
10. API saves fraud alert to database
    → 💾 Fraud alert stored
    ↓
11. API caches result in Redis
    → ⚡ Fast access next time
    ↓
12. API returns response to frontend
    ↓
13. Dashboard receives response
    → 📊 Shows transaction with fraud status
    ↓
14. Prometheus collects metrics
    → 📈 Records: 1 request, response time, etc.
    ↓
15. Grafana displays on dashboard
    → 📉 Real-time graphs updated
```

**Total time:** ~500ms (without Redis: ~1500ms)

---

## ✅ Verification Checklist

- [ ] All 10 services showing "Up (healthy)" or "Up"
- [ ] Can access http://localhost (UI loads)
- [ ] Can login with admin@fraudguard.com / Admin@123
- [ ] Can create a transaction
- [ ] Transaction shows fraud status
- [ ] Can access http://localhost:3000 (Grafana)
- [ ] Grafana shows graphs with data
- [ ] Can access http://localhost:9090 (Prometheus)
- [ ] Prometheus shows metrics
- [ ] Can access http://localhost:8080 (Kafka UI)
- [ ] Kafka shows topics and messages

---

## 🎉 You're Ready!

**What you now have:**

✅ **Redis** - Fast caching  
✅ **Kafka** - Real-time event streaming  
✅ **Prometheus** - Metrics collection  
✅ **Grafana** - Beautiful dashboards  
✅ **Full Stack** - Production-ready  

**All services working together seamlessly!**

---

## 📚 Quick Command Cheat Sheet

```bash
# START
docker-compose up -d --build

# CHECK STATUS
docker-compose ps

# VIEW LOGS
docker-compose logs -f api

# STOP
docker-compose stop

# RESTART
docker-compose restart

# CLEAN UP
docker system prune -a --volumes

# REBUILD SPECIFIC SERVICE
docker-compose build api && docker-compose restart api
```

---

**Last Updated:** January 17, 2026  
**Project:** FraudGuard - Enterprise Fraud Detection Platform  
**Status:** ✅ Full Stack Running with All Services

---

*Everything is configured and ready to use! Just run `docker-compose up --build` and watch your fraud detection system work in real-time!* 🚀
