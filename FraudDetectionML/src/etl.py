import pandas as pd 
import numpy as np
from sklearn.preprocessing import StandardScaler
from sklearn.model_selection import train_test_split
from imblearn.over_sampling import SMOTE
import joblib

class FraudETL:
    def __init__(self, data_path):
        self.data_path = data_path
        self.scaler = StandardScaler()
        
    def load_data(self):
        """Charger le dataset"""
        df = pd.read_csv(self.data_path)
        return df
    
    def clean_data(self, df):
        """Nettoyer les données"""      
        # Supprimer les doublons
        initial_count = len(df)
        df = df.drop_duplicates()
        print(f"   - {initial_count - len(df)} doublons supprimés")
        
        # Gérer les valeurs manquantes
        if df.isnull().sum().sum() > 0:
            df = df.dropna()
            print(f"   - Valeurs manquantes supprimées")
        
        # Supprimer les outliers extrêmes pour Amount
        Q1 = df['Amount'].quantile(0.25)
        Q3 = df['Amount'].quantile(0.75)
        IQR = Q3 - Q1
        df = df[~((df['Amount'] < (Q1 - 3 * IQR)) | (df['Amount'] > (Q3 + 3 * IQR)))]
        
        return df
    
    def feature_engineering(self, df):
        """Créer de nouvelles features"""
        print("⚙️ Feature Engineering...")
        
        # Normaliser Amount
        df['Amount_scaled'] = self.scaler.fit_transform(df[['Amount']])
        
        # Créer des features temporelles
        df['Hour'] = (df['Time'] / 3600) % 24
        df['Day'] = (df['Time'] / 86400).astype(int)
        
        # Interactions
        df['Amount_Hour'] = df['Amount_scaled'] * df['Hour']
        
        print(f"✅ {len(df.columns)} features au total")
        return df
    
    def balance_dataset(self, X, y):
        """Équilibrer le dataset avec SMOTE"""
        print("⚖️ Équilibrage du dataset (SMOTE)...")
        
        fraud_count = y.sum()
        normal_count = len(y) - fraud_count
        print(f"   Avant : Normal={normal_count}, Fraude={fraud_count}")
        
        smote = SMOTE(random_state=42, sampling_strategy=0.5)
        X_balanced, y_balanced = smote.fit_resample(X, y)
        
        fraud_count_after = y_balanced.sum()
        normal_count_after = len(y_balanced) - fraud_count_after
        print(f"   Après : Normal={normal_count_after}, Fraude={fraud_count_after}")
        
        return X_balanced, y_balanced
    
    def prepare_data(self):
        """Pipeline ETL complet"""
        # 1. Charger
        df = self.load_data()
        
        # 2. Nettoyer
        df = self.clean_data(df)
        
        # 3. Feature Engineering
        df = self.feature_engineering(df)
        
        # 4. Séparer X et y
        feature_columns = [col for col in df.columns if col not in ['Class', 'Time']]
        X = df[feature_columns]
        y = df['Class']
        
        # 5. Split Train/Test
        X_train, X_test, y_train, y_test = train_test_split(
            X, y, test_size=0.2, random_state=42, stratify=y
        )
        
        # 6. Équilibrer (seulement le train set)
        X_train_balanced, y_train_balanced = self.balance_dataset(X_train, y_train)
        
        # 7. Sauvegarder le scaler
        joblib.dump(self.scaler, 'models/scaler.pkl')
        print("💾 Scaler sauvegardé")
        
        return X_train_balanced, X_test, y_train_balanced, y_test, feature_columns
    
    def prepare_data_with_validation(self):
        """Pipeline ETL complet avec validation set"""
        # 1. Charger
        df = self.load_data()
        
        # 2. Nettoyer
        df = self.clean_data(df)
        
        # 3. Feature Engineering
        df = self.feature_engineering(df)
        
        # 4. Séparer X et y
        feature_columns = [col for col in df.columns if col not in ['Class', 'Time']]
        X = df[feature_columns]
        y = df['Class']
        
        # 5. Split Train/Test (80/20)
        X_temp, X_test, y_temp, y_test = train_test_split(
            X, y, test_size=0.2, random_state=42, stratify=y
        )
        
        # 6. Split Train/Validation (80/20 of remaining = 64/16)
        X_train, X_val, y_train, y_val = train_test_split(
            X_temp, y_temp, test_size=0.2, random_state=42, stratify=y_temp
        )
        
        # 7. Équilibrer (seulement le train set)
        X_train_balanced, y_train_balanced = self.balance_dataset(X_train, y_train)
        
        # 8. Sauvegarder le scaler
        joblib.dump(self.scaler, 'models/scaler.pkl')
        print("💾 Scaler sauvegardé")
        
        return X_train_balanced, X_test, y_train_balanced, y_test, X_val, y_val, feature_columns

if __name__ == "__main__":
    etl = FraudETL('../data/creditcard.csv')
    X_train, X_test, y_train, y_test, features = etl.prepare_data()
    
    print(f"\n✅ ETL terminé !")
    print(f"   Train set : {len(X_train)} samples")
    print(f"   Test set : {len(X_test)} samples")