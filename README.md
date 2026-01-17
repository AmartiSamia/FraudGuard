# 🛡️ FraudGuard - Enterprise Fraud Detection Platform

<p align="center">
  <strong>Real-time fraud detection powered by Machine Learning</strong>
</p>

<p align="center">
  <a href="#features">Features</a> •
  <a href="#architecture">Architecture</a> •
  <a href="#quick-start">Quick Start</a> •
  <a href="#api-documentation">API Docs</a> •
  <a href="#deployment">Deployment</a>
</p>

---

## 📋 Overview

FraudGuard is an enterprise-grade fraud detection platform that uses machine learning to identify suspicious financial transactions in real-time. Built with a microservices architecture, it provides scalable, reliable, and accurate fraud detection capabilities.

### Key Capabilities

- 🔍 **Real-time Detection**: Sub-second fraud prediction using XGBoost ML models
- 📊 **Analytics Dashboard**: Comprehensive admin and user dashboards
- 🔔 **Alert Management**: Automated fraud alerts with risk scoring
- 📈 **Trend Analysis**: Historical pattern recognition and reporting
- 🔐 **Role-based Access**: Separate admin and user experiences
- 🐳 **Cloud-Ready**: Docker and Kubernetes deployment support

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                         FraudGuard Platform                          │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐           │
│  │   Angular    │    │  ASP.NET Core │    │   Python     │           │
│  │   Frontend   │◄──►│     API       │◄──►│  ML Service  │           │
│  │   (Port 4200)│    │  (Port 5203)  │    │  (Port 5000) │           │
│  └──────────────┘    └──────────────┘    └──────────────┘           │
│         │                   │                    │                    │
│         │                   ▼                    │                    │
│         │           ┌──────────────┐             │                    │
│         │           │   SQL Server  │             │                    │
│         │           │   Database    │             │                    │
│         │           └──────────────┘             │                    │
│         │                   │                    │                    │
│         └───────────────────┼────────────────────┘                    │
│                             ▼                                         │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐           │
│  │    Kafka     │    │    Redis     │    │  Prometheus  │           │
│  │   Streaming  │    │   Caching    │    │   Metrics    │           │
│  └──────────────┘    └──────────────┘    └──────────────┘           │
│                                                 │                     │
│                                                 ▼                     │
│                                          ┌──────────────┐            │
│                                          │   Grafana    │            │
│                                          │  Dashboards  │            │
│                                          └──────────────┘            │
└─────────────────────────────────────────────────────────────────────┘
```

### Technology Stack

| Layer | Technology | Purpose |
|-------|------------|---------|
| Frontend | Angular 17 | User interface |
| Backend API | ASP.NET Core 8 | REST API, Business Logic |
| ML Service | Python Flask | Fraud prediction |
| Database | SQL Server | Data persistence |
| Caching | Redis | Performance optimization |
| Messaging | Apache Kafka | Event streaming |
| Monitoring | Prometheus + Grafana | Metrics & dashboards |
| Containers | Docker + Kubernetes | Deployment |

---

## ✨ Features

### 👤 User Features
- View personal transaction history
- See account balances and activity
- Receive fraud alerts on suspicious activity
- Filter transactions by status

### 👨‍💼 Admin Features
- Comprehensive analytics dashboard
- View all users and accounts
- Manage fraud alerts
- Transaction trend analysis
- Export data for Power BI
- Country/device fraud analysis
- Hourly pattern detection

### 🤖 ML Capabilities
- XGBoost-based fraud detection
- Real-time prediction (<100ms latency)
- Risk scoring (0-100%)
- Batch prediction support
- Model versioning

---

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)
- [Python 3.11+](https://www.python.org/)
- [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- [Docker](https://www.docker.com/) (optional)

### Local Development Setup

#### 1. Clone the Repository

```bash
git clone https://github.com/your-org/fraudguard.git
cd fraudguard
```

#### 2. Start the Backend API

```bash
cd FraudDetectionAPI
dotnet restore
dotnet ef database update
dotnet run
```

The API will start on `http://localhost:5203`

#### 3. Start the ML Service

```bash
cd FraudDetectionML
pip install -r requirements.txt
python src/app.py
```

The ML service will start on `http://localhost:5000`

#### 4. Start the Frontend

```bash
cd FraudDetectionUI
npm install --legacy-peer-deps
ng serve
```

The UI will start on `http://localhost:4200`

### 🔐 Test Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@fraudguard.com | Admin@123 |
| User | demo@test.com | demo123 |

---

## 🐳 Docker Deployment

### Using Docker Compose

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### Service URLs (Docker)

| Service | URL |
|---------|-----|
| Frontend | http://localhost:80 |
| API | http://localhost:5203 |
| ML Service | http://localhost:5000 |
| Kafka UI | http://localhost:8080 |
| Grafana | http://localhost:3000 |
| Prometheus | http://localhost:9090 |

---

## 📚 API Documentation

### Authentication

```bash
POST /api/User/login
Content-Type: application/json

{
  "email": "admin@fraudguard.com",
  "password": "Admin@123"
}
```

### Fraud Prediction

```bash
POST /api/Transaction/create
Authorization: Bearer {token}
Content-Type: application/json

{
  "accountId": 1,
  "amount": 1500.00,
  "type": "transfer",
  "country": "US",
  "device": "mobile"
}
```

### Analytics Endpoints (Admin Only)

| Endpoint | Description |
|----------|-------------|
| GET /api/Analytics/dashboard | Get dashboard statistics |
| GET /api/Analytics/trends | Get transaction trends |
| GET /api/Analytics/fraud-by-country | Fraud analysis by country |
| GET /api/Analytics/fraud-by-device | Fraud analysis by device |
| GET /api/Analytics/hourly-patterns | Hourly fraud patterns |
| GET /api/Analytics/users | Get all users |
| GET /api/Analytics/export/powerbi | Export for Power BI |

### Full API Documentation

Access Swagger UI at: `http://localhost:5203/swagger`

---

## 📊 Analytics & Reporting

### Power BI Integration

1. Export data via API: `GET /api/Analytics/export/powerbi`
2. Import JSON into Power BI
3. Create custom dashboards

### Grafana Dashboards

Pre-configured dashboards available:
- System Health
- Transaction Volume
- Fraud Detection Rate
- Response Time Metrics

Access Grafana at: `http://localhost:3000` (admin/FraudGuard@2024)

---

## 🔄 ETL Pipeline

Run the ETL pipeline for data processing:

```bash
cd FraudDetectionML
python src/etl_pipeline.py --source data/transactions.csv --output output/
```

---

## 📁 Project Structure

```
FraudGuard/
├── FraudDetectionAPI/          # ASP.NET Core Backend
│   ├── Controllers/            # API endpoints
│   │   ├── UserController.cs
│   │   ├── TransactionController.cs
│   │   ├── AnalyticsController.cs  # Admin analytics
│   │   └── HealthController.cs     # Health checks
│   ├── Services/              # Business logic
│   │   ├── KafkaService.cs    # Event streaming
│   │   └── CacheService.cs    # Redis caching
│   ├── Repositories/          # Data access
│   ├── Models/                # Entity models
│   ├── DTO/                   # Data transfer objects
│   └── Data/                  # Database context
│
├── FraudDetectionML/           # Python ML Service
│   ├── src/
│   │   ├── app.py            # Flask API
│   │   ├── app_enhanced.py   # Enhanced with Kafka/Redis
│   │   ├── train.py          # Model training
│   │   └── etl_pipeline.py   # ETL processing
│   └── models/               # Trained models
│
├── FraudDetectionUI/           # Angular Frontend
│   ├── src/app/
│   │   ├── modules/
│   │   │   ├── admin/        # Admin dashboard
│   │   │   ├── user/         # User dashboard
│   │   │   └── auth/         # Authentication
│   │   ├── services/         # API services
│   │   └── shared/           # Shared components
│   └── styles.scss           # Global styles
│
├── monitoring/                 # Prometheus & Grafana
│   ├── prometheus/
│   │   ├── prometheus.yml
│   │   └── alert_rules.yml
│   └── grafana/
│       └── provisioning/
│
├── docker-compose.yml          # Container orchestration
├── Dockerfile (per service)    # Container definitions
└── README.md                   # This file
```

---

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | API environment | Development |
| `ConnectionStrings__DefaultConnection` | Database connection | LocalDB |
| `Kafka__Enabled` | Enable Kafka | false |
| `Redis__Enabled` | Enable Redis | false |
| `MLService__Url` | ML service URL | http://localhost:5000 |

---

## 👥 Team

This project was developed by a team of 5 members:

| Role | Responsibilities |
|------|-----------------|
| Backend Developer | ASP.NET Core API, Database design |
| ML Engineer | Model training, Python service |
| Frontend Developer | Angular UI, Dashboard design |
| DevOps Engineer | Docker, Kubernetes, Monitoring |
| QA/Documentation | Testing, Documentation |

---

## 📈 Database Schema

```sql
Users (Id, Email, Password, FirstName, LastName, Role)
    │
    └── Accounts (Id, UserId, AccountNumber, Balance, CreatedAt)
            │
            └── Transactions (Id, AccountId, Amount, Type, Country, Device, Timestamp, IsFraud)
                    │
                    └── FraudAlerts (Id, TransactionId, RiskScore, Status, CreatedAt, UpdatedAt)
```

---

## 🔒 Security

- JWT-based authentication
- Role-based authorization (Admin/User)
- Password hashing with BCrypt
- CORS protection
- Input validation
- SQL injection prevention via EF Core

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

<p align="center">
  Made with ❤️ by the FraudGuard Team
</p>
