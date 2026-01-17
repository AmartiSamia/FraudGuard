import pandas as pd
import numpy as np
from sklearn.ensemble import RandomForestClassifier
from xgboost import XGBClassifier
from sklearn.metrics import classification_report, confusion_matrix, roc_auc_score
import joblib
from etl import FraudETL

class FraudModelTrainer:
    def __init__(self):
        self.model = None
        
    def train_xgboost(self, X_train, y_train):
        """Entraîner un modèle XGBoost"""
        print("🚀 Entraînement XGBoost...")
        
        self.model = XGBClassifier(
            n_estimators=100,
            max_depth=6,
            learning_rate=0.1,
            subsample=0.8,
            colsample_bytree=0.8,
            random_state=42,
            eval_metric='logloss'
        )
        
        self.model.fit(X_train, y_train)
        print("✅ Entraînement terminé")
        
    def train_random_forest(self, X_train, y_train):
        """Entraîner un Random Forest (alternative)"""
        print("🚀 Entraînement Random Forest...")
        
        self.model = RandomForestClassifier(
            n_estimators=100,
            max_depth=10,
            random_state=42,
            n_jobs=-1
        )
        
        self.model.fit(X_train, y_train)
        print("✅ Entraînement terminé")
    
    def evaluate(self, X_test, y_test):
        """Évaluer le modèle"""
        print("\n📊 Évaluation du modèle...")
        
        y_pred = self.model.predict(X_test)
        y_pred_proba = self.model.predict_proba(X_test)[:, 1]
        
        # Métriques
        print("\n" + "="*50)
        print(classification_report(y_test, y_pred, target_names=['Normal', 'Fraude']))
        print("="*50)
        
        # ROC AUC
        roc_auc = roc_auc_score(y_test, y_pred_proba)
        print(f"\n🎯 ROC AUC Score : {roc_auc:.4f}")
        
        # Matrice de confusion
        cm = confusion_matrix(y_test, y_pred)
        print(f"\n📈 Matrice de Confusion :")
        print(f"   TN: {cm[0,0]}  |  FP: {cm[0,1]}")
        print(f"   FN: {cm[1,0]}  |  TP: {cm[1,1]}")
        
        return roc_auc
    
    def save_model(self, path='../models/fraud_model.pkl'):
        """Sauvegarder le modèle"""
        joblib.dump(self.model, path)
        print(f"\n💾 Modèle sauvegardé : {path}")

if __name__ == "__main__":
    # 1. ETL
    etl = FraudETL('../data/creditcard.csv')
    X_train, X_test, y_train, y_test, features = etl.prepare_data()
    
    # 2. Entraînement
    trainer = FraudModelTrainer()
    trainer.train_xgboost(X_train, y_train)  # ou train_random_forest
    
    # 3. Évaluation
    roc_auc = trainer.evaluate(X_test, y_test)
    
    # 4. Sauvegarde
    if roc_auc > 0.95:  # Seulement si performant
        trainer.save_model()
    else:
        print("⚠️ Performance insuffisante, modèle non sauvegardé")