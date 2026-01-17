import pandas as pd
import numpy as np

# Set random seed for reproducibility
np.random.seed(42)

# Generate synthetic credit card fraud dataset
n_samples = 10000
fraud_ratio = 0.03  # 3% fraud rate

# Features - PCA transformed features V1-V28 + Amount + Time
features = ['V1', 'V2', 'V3', 'V4', 'V5', 'V6', 'V7', 'V8', 'V9', 'V10',
            'V11', 'V12', 'V13', 'V14', 'V15', 'V16', 'V17', 'V18', 'V19', 'V20',
            'V21', 'V22', 'V23', 'V24', 'V25', 'V26', 'V27', 'V28', 'Amount', 'Time', 'Class']

# Create dataset
data = {}

# Normal transactions
n_normal = int(n_samples * (1 - fraud_ratio))
for i in range(28):
    data[f'V{i+1}'] = np.random.normal(0, 1, n_normal).tolist()

data['Amount'] = np.random.exponential(50, n_normal).tolist()
data['Time'] = np.random.randint(0, 86400, n_normal).tolist()
data['Class'] = [0] * n_normal

# Fraudulent transactions
n_fraud = int(n_samples * fraud_ratio)
for i in range(28):
    data[f'V{i+1}'].extend(np.random.normal(2, 2, n_fraud).tolist())

# Fraudulent transactions tend to have higher amounts and different time patterns
data['Amount'].extend(np.random.exponential(200, n_fraud).tolist())
data['Time'].extend(np.random.randint(0, 86400, n_fraud).tolist())
data['Class'].extend([1] * n_fraud)

# Create DataFrame
df = pd.DataFrame(data)

# Shuffle
df = df.sample(frac=1).reset_index(drop=True)

# Save to CSV
df.to_csv('c:/Users/Victus/Desktop/PFA_Project-main/PFA_Project-main/FraudDetectionML/data/creditcard.csv', index=False)
print(f"✅ Dataset created with {len(df)} samples")
print(f"   - Normal transactions: {(df['Class'] == 0).sum()}")
print(f"   - Fraudulent transactions: {(df['Class'] == 1).sum()}")
print(f"   - Fraud ratio: {(df['Class'] == 1).sum() / len(df) * 100:.2f}%")
