from flask import Flask, request, jsonify
from flask_cors import CORS
import joblib
import numpy as np

app = Flask(__name__)
CORS(app)

# Load model and scaler with proper path handling
import os
model_dir = os.path.join(os.path.dirname(__file__), '..', 'models')
model_path = os.path.join(model_dir, 'fraud_model.pkl')
scaler_path = os.path.join(model_dir, 'scaler.pkl')

model = joblib.load(model_path)
scaler = joblib.load(scaler_path)

print("✅ Modèle chargé avec succès")


@app.route('/health', methods=['GET'])
def health():
    """Vérifier que l'API est en ligne"""
    return jsonify({"status": "healthy", "model_loaded": True}), 200


@app.route('/predict', methods=['POST'])
def predict():
    """
    Prédire si une transaction est frauduleuse.
    
    Input JSON:
    {
        "amount": 1500.50,
        "time": 12345,
        "v1": ...,
        ...
        "v28": ...
    }
    """
    try:
        data = request.get_json()

        # ----- Extraire les valeurs -----
        amount = data.get('amount', 0)
        time_value = data.get('time', 0)

        # Normaliser le montant
        amount_scaled = scaler.transform([[amount]])[0][0]

        # Features temporelles
        hour = (time_value / 3600) % 24
        day = (time_value / 86400)

        # ----- Construire le vecteur de features dans le même ordre que ton ETL -----
        features = []

        # 1. V1 .. V28 (28 features)
        for i in range(1, 29):
            features.append(data.get(f'v{i}', 0))

        # 2. Amount (brut)
        features.append(amount)

        # 3. Amount_scaled
        features.append(amount_scaled)

        # 4. Hour
        features.append(hour)

        # 5. Day
        features.append(day)

        # 6. Amount_Hour (feature d'interaction)
        features.append(amount_scaled * hour)

        # Vérification du nombre de features
        if len(features) != 33:
            return jsonify({
                "error": f"Bad feature count: expected 33, got {len(features)}",
                "details": features
            }), 400

        # ----- Conversion en array -----
        X = np.array([features])

        # ----- Prédictions -----
        prediction = model.predict(X)[0]
        probability = model.predict_proba(X)[0][1]

        # Niveau de risque
        if probability >= 0.8:
            risk_level = "HIGH"
        elif probability >= 0.5:
            risk_level = "MEDIUM"
        else:
            risk_level = "LOW"

        response = {
            "is_fraud": bool(prediction),
            "probability": float(probability),
            "risk_level": risk_level
        }

        return jsonify(response), 200

    except Exception as e:
        return jsonify({"error": str(e)}), 400


if __name__ == '__main__':
    print("🚀 API Flask lancée sur http://localhost:5000")
    app.run(host='0.0.0.0', port=5000, debug=True)
