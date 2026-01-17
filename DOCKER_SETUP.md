# 🚀 FraudGuard - Quick Start with Docker

## Prerequisites

Before you begin, make sure you have installed:
- **Docker Desktop** (Windows/Mac) or **Docker Engine** (Linux)
- **Docker Compose** (included with Docker Desktop)

Download Docker Desktop: https://www.docker.com/products/docker-desktop

## 🏃 Quick Start (One Command)

### Option 1: Simple Setup (Recommended for first use)
```bash
# Clone the repository (if not already done)
git clone <repository-url>
cd PFA_Project-main

# Start all services with one command
docker-compose -f docker-compose.simple.yml up --build
```

### Option 2: Full Setup (with Kafka, Redis, Prometheus, Grafana)
```bash
docker-compose up --build
```

Wait 2-3 minutes for all services to start. You'll see logs from all containers.

## 🌐 Access the Application

Once all services are running:

| Service | URL | Description |
|---------|-----|-------------|
| **Frontend** | http://localhost | Main application |
| **API Swagger** | http://localhost:5203/swagger | API Documentation |
| **ML Service** | http://localhost:5000/health | ML Health Check |

### Login Credentials

| Role | Email | Password |
|------|-------|----------|
| **Admin** | `admin@fraudguard.com` | `Admin@123` |
| **User** | `demo@test.com` | `demo123` |

## 📦 What's Included

The Docker setup includes:

1. **Frontend (Angular)** - Port 80
   - Modern fraud detection dashboard
   - User and Admin interfaces
   
2. **Backend API (.NET Core)** - Port 5203
   - RESTful API with JWT authentication
   - Database management
   - Swagger documentation

3. **ML Service (Python/Flask)** - Port 5000
   - Fraud prediction using XGBoost
   - Real-time transaction analysis

4. **Database (SQL Server)** - Port 1433
   - Pre-seeded with sample data
   - Automatic database initialization

## 🛑 Stop the Application

```bash
# Stop all containers
docker-compose -f docker-compose.simple.yml down

# Stop and remove all data (volumes)
docker-compose -f docker-compose.simple.yml down -v
```

## 🔧 Troubleshooting

### Database not starting?
```bash
# Check database logs
docker logs fraudguard-db

# Restart just the database
docker-compose -f docker-compose.simple.yml restart database
```

### API not connecting to database?
The API waits for the database to be healthy. Wait 60 seconds after starting.

### Port already in use?
```bash
# Find what's using the port (Windows PowerShell)
netstat -ano | findstr :80
netstat -ano | findstr :5203

# Kill the process (replace PID with actual process ID)
taskkill /PID <PID> /F
```

### Clean restart
```bash
# Remove all containers and volumes
docker-compose -f docker-compose.simple.yml down -v

# Remove unused images
docker system prune -a

# Start fresh
docker-compose -f docker-compose.simple.yml up --build
```

## 📊 View Logs

```bash
# All services
docker-compose -f docker-compose.simple.yml logs -f

# Specific service
docker logs fraudguard-api -f
docker logs fraudguard-ml -f
docker logs fraudguard-ui -f
docker logs fraudguard-db -f
```

## 💻 Development Mode (Without Docker)

If you prefer to run locally without Docker:

### Backend API
```bash
cd FraudDetectionAPI
dotnet run
```

### ML Service
```bash
cd FraudDetectionML
pip install -r requirements.txt
python src/app.py
```

### Frontend
```bash
cd FraudDetectionUI
npm install
npm start
```

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Docker Network                          │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────┐    ┌─────────┐    ┌─────────┐    ┌──────────┐ │
│  │   UI    │───▶│   API   │───▶│   ML    │    │ Database │ │
│  │ :80     │    │ :5203   │    │ :5000   │    │ :1433    │ │
│  │ Angular │    │ .NET    │    │ Python  │    │ SQL Srv  │ │
│  └─────────┘    └────┬────┘    └─────────┘    └────▲─────┘ │
│                      │                              │       │
│                      └──────────────────────────────┘       │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## 📝 Environment Variables

These can be modified in `docker-compose.simple.yml`:

| Variable | Default | Description |
|----------|---------|-------------|
| `SA_PASSWORD` | `FraudGuard@2024!` | SQL Server password |
| `Jwt__Key` | (set in compose) | JWT signing key |
| `ASPNETCORE_ENVIRONMENT` | `Production` | API environment |
| `FLASK_ENV` | `production` | ML service environment |

---

**Need help?** Check the logs first: `docker-compose logs -f`
