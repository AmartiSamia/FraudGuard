# FraudGuard Architecture - Eraser.io Prompt (OPTIMIZED)

## COMPACT VERSION - Copy This:

```
Create a clean, modern system architecture diagram for "FraudGuard - Fraud Detection Platform".

🐳 DOCKER ORCHESTRATION FOUNDATION (Wraps Everything)
   - Docker Engine (Container Runtime)
   - Docker Compose (Service Orchestration)
   - 10 Services in Isolated Containers
   - Shared Network: fraudguard-network

LAYOUT: 5 horizontal layers top-to-bottom

LAYER 1 - CLIENT:
Browser [🌐 Light Blue] → NGINX Container [🐳 Port 80]

LAYER 2 - APPLICATION:
ASP.NET Core API Container [🐳 🔷 Port 5203] Healthy ✅

LAYER 3 - DATA & SERVICES (parallel containers):
- SQL Server Container [🐳 🗄️ Port 1433] Healthy ✅
  
- Redis Cache Container [🐳 🔴 Port 6379] Healthy ✅
  
- Kafka Container [🐳 ⚙️ Port 9092] + Zookeeper Container [🐳 🐘 Port 2181] Healthy ✅
  
- Python ML Container [🐳 🐍 Port 5000] Healthy ✅

LAYER 4 - MONITORING:
Prometheus Container [🐳 📈 Port 9090] → Grafana Container [🐳 📉 Port 3000]

LAYER 5 - INFRASTRUCTURE:
🐳 Docker Compose manages:
- Container networking
- Volume persistence
- Environment variables
- Health checks
- Auto-restart policies
- Resource limits

CONNECTIONS:
- Browser → NGINX → API [Blue - HTTP/REST]
- API → Database [Orange - SQL]
- API → Redis [Orange - Cache]
- API ↔ Kafka ↔ ML [Green - Events]
- All → Prometheus → Grafana [Red - Metrics]

COLORS:
Frontend: #E3F2FD (light blue)
API: #1565C0 (deep blue)
Data/Cache: #FF9800 (orange)
Messaging: #388E3C (green)
ML: #7B1FA2 (purple)
Monitoring: #D32F2F (red)
Docker: #2496ED (docker blue)

CONTAINERS (10 Total):
1. 🐳 fraudguard-database (SQL Server)
2. 🐳 fraudguard-redis (Cache)
3. 🐳 fraudguard-zookeeper (Kafka coordinator)
4. 🐳 fraudguard-kafka (Message broker)
5. 🐳 fraudguard-api (Backend API)
6. 🐳 fraudguard-ml (ML Service)
7. 🐳 fraudguard-ui (Frontend)
8. 🐳 fraudguard-prometheus (Metrics)
9. 🐳 fraudguard-grafana (Dashboards)
10. 🐳 kafka-ui (Message browser)

USE: 
- Draw Docker as foundation/border around all layers
- Label each service with 🐳 icon
- Show port numbers
- Use "Healthy ✅" indicators
- Color-code by function
- Show shared Docker network connecting all
```

---

## ULTRA-COMPACT VERSION - If Above is Still Large:

```
FraudGuard Enterprise Fraud Detection System Architecture

4-LAYER ARCHITECTURE:

LAYER 1: 🌐 Client
Browser → NGINX (Port 80)

LAYER 2: 🔷 API
ASP.NET Core 8 (Port 5203)

LAYER 3: Parallel Services
├─ 🗄️ SQL Server (Port 1433)
├─ 🔴 Redis Cache (Port 6379)
├─ ⚙️ Kafka + 🐘 Zookeeper (9092, 2181)
└─ 🐍 Python ML (Port 5000)

LAYER 4: 📈 Monitoring
Prometheus (9090) → 📉 Grafana (3000)

FLOWS:
REST/HTTP: Browser ↔ NGINX ↔ API [Blue]
Data: API ↔ Database/Cache [Orange]
Events: API ↔ Kafka ↔ ML [Green]
Metrics: Services → Prometheus → Grafana [Red]

KEY STATS:
✅ API Response: <50ms (cached), <100ms (DB)
✅ Cache: 70-85% hit rate, 3-5x faster
✅ ML: 98% accuracy, <50ms inference
✅ Kafka: 3 topics, 1000 msg/sec throughput
✅ Monitoring: 15-sec intervals, 8+ metrics

All 10 services in Docker containers
```

---

## RECOMMENDATION:
Use the **COMPACT VERSION** - it's clean, fits on one screen, shows all 10 services with colors and relationships clearly without clutter.
