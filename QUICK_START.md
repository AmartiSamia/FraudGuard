# Quick Start Guide

Get the Fraud Detection System up and running in 5 minutes!

## 🎯 What You'll Get

- ✅ ASP.NET Core API with fraud detection endpoints
- ✅ Angular web application with Admin & User interfaces
- ✅ Improved ML model for better fraud detection
- ✅ Real-time dashboards with analytics
- ✅ Complete authentication system

---

## ⚡ Quick Start (3 terminals)

### Terminal 1: Start Backend (ASP.NET Core)

```bash
cd FraudDetectionAPI
dotnet restore
dotnet ef database update  # Create database
dotnet run
# ✅ API running on https://localhost:7000
```

**Test it:**
```bash
curl -X POST https://localhost:7000/api/User/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password123"}'
```

### Terminal 2: Start ML Model (Flask)

```bash
cd FraudDetectionML
python -m venv venv
source venv/bin/activate  # Windows: venv\Scripts\activate
pip install -r requirements.txt
python src/train_improved.py  # Optional: retrain model
python src/app.py
# ✅ ML API running on http://localhost:5000
```

**Test it:**
```bash
curl -X GET http://localhost:5000/health
# Response: {"status": "healthy", "model_loaded": true}
```

### Terminal 3: Start Frontend (Angular)

```bash
cd FraudDetectionUI
npm install
npm start
# ✅ App running on http://localhost:4200
```

---

## 🔐 Default Credentials

**Admin Account:**
- Email: `admin@example.com`
- Password: `admin123`
- Role: Admin

**User Account:**
- Email: `user@example.com`
- Password: `user123`
- Role: User

*Note: Create these accounts in database or use register form*

---

## 🚀 Key Features by Role

### 👨‍💼 Admin

After login, navigate to **Admin Dashboard**:

1. **Dashboard** - Overall statistics
   - Total transactions & fraud count
   - Revenue metrics
   - Pending alerts count

2. **Suspicious Transactions** - View all flagged transactions
   - Search and filter by date
   - View fraud reasons

3. **Fraud Alerts** - Manage pending alerts
   - Review high-risk transactions
   - Accept or dismiss alerts

4. **Statistics** - Analytics & charts
   - 30-day fraud trends
   - Fraud by country/device
   - Risk distribution

### 👤 User

After login, navigate to **User Dashboard**:

1. **Dashboard** - Personal stats
   - Your total transactions
   - Number of suspicious transactions
   - Account summary

2. **Transactions** - Transaction history
   - All your transactions
   - Filter suspicious ones
   - View transaction details

---

## 📊 Understanding the Dashboards

### Admin Insights

```
Statistics Cards (7 cards):
├── Total Transactions: 10,234
├── Fraud Transactions: 127 (1.24%)
├── Total Amount: $2,456,789.50
├── Fraud Amount: $45,230.25
├── Total Users: 1,250
├── Total Accounts: 1,840
└── Pending Alerts: 15

Recent Suspicious Transactions (Table):
└── Shows ID, owner, amount, country, device, timestamp, reason

Pending Fraud Alerts (Cards):
└── Shows alert details with risk scores and action buttons

High Risk Accounts (Table):
└── Shows account ID, owner, transaction count, fraud count, risk score
```

### User Dashboard

```
Account Summary (4 cards):
├── Total Transactions: 245
├── Suspicious: 3 (1.22%)
├── Total Amount: $12,450
└── Suspicious Amount: $890.50

My Transactions (List):
└── Transaction items with type, amount, location, date, fraud status
```

---

## 🔑 API Endpoints Reference

### Authentication
```
POST /api/User/register          # Create account
POST /api/User/login             # Login & get JWT token
```

### Admin Dashboard APIs
```
GET /api/Dashboard/statistics            # Overall metrics
GET /api/Dashboard/fraud-by-period       # 30-day trend
GET /api/Dashboard/fraud-by-country      # By country
GET /api/Dashboard/fraud-by-device       # By device
GET /api/Dashboard/recent-suspicious     # Latest fraud
GET /api/Dashboard/pending-alerts        # Unresolved alerts
GET /api/Dashboard/high-risk-accounts    # High-risk accounts
```

### User Dashboard APIs
```
GET /api/Dashboard/user-statistics/{id}  # User stats
GET /api/Transaction/list                # User transactions
```

---

## 🛠️ Troubleshooting

### Issue: CORS Error in Console

**Solution:**
Backend CORS is configured for `localhost:4200`. If running on different port:

Edit `Program.cs`:
```csharp
.WithOrigins("http://localhost:YOUR_PORT")
```

### Issue: "Model not loaded" Error

**Solution:**
ML model files missing. Run training:
```bash
cd FraudDetectionML/src
python train_improved.py
```

### Issue: Database Error

**Solution:**
Create/update database:
```bash
cd FraudDetectionAPI
dotnet ef database update
```

### Issue: Port Already in Use

**Solution:**
Change port in configuration:

**Backend:** `launchSettings.json`
**Frontend:** `angular.json` 
**ML:** `app.py` line with `port=5000`

---

## 📱 Browser Support

- ✅ Chrome 90+
- ✅ Firefox 88+
- ✅ Safari 14+
- ✅ Edge 90+

---

## 🔒 Security Notes

⚠️ **Development Only:**
- HTTPS metadata not required
- Default credentials provided
- CORS allows all methods

⚠️ **For Production:**
1. Generate strong JWT secret key
2. Use HTTPS everywhere
3. Change default credentials
4. Enable HTTPS metadata validation
5. Restrict CORS to specific domains
6. Implement rate limiting
7. Add API key authentication

---

## 📊 Checking Model Quality

View detailed ML assessment:

```bash
# Terminal 2 output will show:
🎯 ROC AUC Score: 0.9567  (Target: > 0.90) ✅
📊 F1 Score: 0.8234      (Target: > 0.75) ✅
Optimal Threshold: 0.4237

Confusion Matrix:
   TN: 56,862  |  FP: 98     (Specificity: 0.9983)
   FN: 18      |  TP: 84     (Sensitivity: 0.8235)
```

**Interpretation:**
- ROC-AUC > 0.95: Excellent model discrimination
- F1 > 0.82: Good balance between false positives & negatives
- High specificity: Few false fraud alerts
- Good sensitivity: Catches most real fraud

---

## 🎨 Customization

### Change Application Title
`FraudDetectionUI/src/app/app.component.ts`:
```typescript
title = 'Your Custom Title';
```

### Change Logo/Branding
`FraudDetectionUI/src/app/app.component.html`:
Update navbar section with your branding

### Change Colors
`FraudDetectionUI/src/app/app.component.scss`:
```scss
.navbar {
  background: linear-gradient(135deg, #YOUR_COLOR1, #YOUR_COLOR2);
}
```

---

## 📈 Performance Tips

### Backend
- Use SQL Server for production (faster than SQLite)
- Enable EF Core query optimization
- Add database indexing on frequently queried columns

### Frontend
- Lazy load modules (already done)
- Use production build: `npm run build`
- Enable gzip compression in web server

### ML Model
- Cache model in memory (Flask does this)
- Use batch predictions when possible
- Monitor model performance drift

---

## 🆘 Getting Help

1. **Check logs:** Look at terminal output for errors
2. **Verify services:** Ensure all 3 terminals running
3. **Clear cache:** Browser Dev Tools → Clear Storage
4. **Restart:** Kill all 3 processes and start fresh

---

## 📚 Documentation Files

- `README.md` - Complete project documentation
- `ML_MODEL_ASSESSMENT.md` - Detailed ML analysis
- Code comments in each component

---

## ✅ Checklist

- [ ] Node.js & npm installed (`node -v`, `npm -v`)
- [ ] .NET SDK installed (`dotnet --version`)
- [ ] Python 3.8+ installed (`python --version`)
- [ ] Database created (`dotnet ef database update`)
- [ ] ML model trained (`python train_improved.py`)
- [ ] All 3 terminals running
- [ ] Can access `http://localhost:4200`
- [ ] Can login with provided credentials
- [ ] Dashboard loads without errors
- [ ] Charts display correctly

---

## 🎉 You're Ready!

The system is now running. Explore:

1. **Login** with admin credentials
2. **Browse Admin Dashboard** to see system overview
3. **Check Statistics** to view fraud analytics
4. **Review Alerts** to see fraud detection in action
5. **Switch to User** role to see user interface

**Enjoy!** 🚀

---

For detailed information, see `README.md` and `ML_MODEL_ASSESSMENT.md`
