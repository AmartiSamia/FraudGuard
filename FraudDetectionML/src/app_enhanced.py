"""
FraudGuard ML Service - Enhanced Edition
=========================================
Enterprise-grade fraud detection service with:
- Real-time prediction API
- Kafka event streaming
- Redis caching
- Prometheus metrics
- Batch processing capabilities
"""

from flask import Flask, request, jsonify
from flask_cors import CORS
import joblib
import numpy as np
import os
import json
import threading
import time
from datetime import datetime
from functools import wraps

# Initialize Flask app
app = Flask(__name__)
CORS(app)

# ============================================================================
# CONFIGURATION
# ============================================================================
config = {
    'KAFKA_ENABLED': os.getenv('KAFKA_ENABLED', 'false').lower() == 'true',
    'KAFKA_BOOTSTRAP_SERVERS': os.getenv('KAFKA_BOOTSTRAP_SERVERS', 'localhost:29092'),
    'REDIS_ENABLED': os.getenv('REDIS_ENABLED', 'false').lower() == 'true',
    'REDIS_HOST': os.getenv('REDIS_HOST', 'localhost'),
    'REDIS_PORT': int(os.getenv('REDIS_PORT', 6379)),
    'MODEL_VERSION': '2.0.0',
    'SERVICE_NAME': 'fraudguard-ml'
}

# ============================================================================
# METRICS (Prometheus-compatible)
# ============================================================================
class Metrics:
    def __init__(self):
        self.predictions_total = 0
        self.fraud_detected = 0
        self.prediction_latency_sum = 0
        self.prediction_latency_count = 0
        self.errors_total = 0
        self.kafka_messages_processed = 0
        self.cache_hits = 0
        self.cache_misses = 0
        self.start_time = time.time()
    
    def record_prediction(self, is_fraud, latency):
        self.predictions_total += 1
        if is_fraud:
            self.fraud_detected += 1
        self.prediction_latency_sum += latency
        self.prediction_latency_count += 1
    
    def record_error(self):
        self.errors_total += 1
    
    def get_metrics(self):
        uptime = time.time() - self.start_time
        avg_latency = (self.prediction_latency_sum / self.prediction_latency_count 
                       if self.prediction_latency_count > 0 else 0)
        fraud_rate = (self.fraud_detected / self.predictions_total 
                      if self.predictions_total > 0 else 0)
        
        return {
            'predictions_total': self.predictions_total,
            'fraud_detected_total': self.fraud_detected,
            'fraud_rate': round(fraud_rate, 4),
            'avg_prediction_latency_ms': round(avg_latency * 1000, 2),
            'errors_total': self.errors_total,
            'kafka_messages_processed': self.kafka_messages_processed,
            'cache_hits': self.cache_hits,
            'cache_misses': self.cache_misses,
            'uptime_seconds': round(uptime, 2)
        }

metrics = Metrics()

# ============================================================================
# MODEL LOADING
# ============================================================================
model_dir = os.path.join(os.path.dirname(__file__), '..', 'models')
model_path = os.path.join(model_dir, 'fraud_model.pkl')
scaler_path = os.path.join(model_dir, 'scaler.pkl')

try:
    model = joblib.load(model_path)
    scaler = joblib.load(scaler_path)
    model_loaded = True
    print("✅ Model loaded successfully")
except Exception as e:
    print(f"⚠️ Model loading failed: {e}")
    model = None
    scaler = None
    model_loaded = False

# ============================================================================
# OPTIONAL: KAFKA INTEGRATION
# ============================================================================
kafka_producer = None
kafka_consumer = None

def init_kafka():
    global kafka_producer, kafka_consumer
    if not config['KAFKA_ENABLED']:
        print("ℹ️ Kafka disabled - running in standalone mode")
        return
    
    try:
        from kafka import KafkaProducer, KafkaConsumer
        
        kafka_producer = KafkaProducer(
            bootstrap_servers=config['KAFKA_BOOTSTRAP_SERVERS'],
            value_serializer=lambda v: json.dumps(v).encode('utf-8'),
            acks='all'
        )
        
        print(f"✅ Kafka producer connected to {config['KAFKA_BOOTSTRAP_SERVERS']}")
        
        # Start consumer in background thread
        consumer_thread = threading.Thread(target=kafka_consumer_loop, daemon=True)
        consumer_thread.start()
        
    except Exception as e:
        print(f"⚠️ Kafka initialization failed: {e}")

def kafka_consumer_loop():
    """Background consumer for transaction events"""
    try:
        from kafka import KafkaConsumer
        
        consumer = KafkaConsumer(
            'transactions',
            bootstrap_servers=config['KAFKA_BOOTSTRAP_SERVERS'],
            value_deserializer=lambda m: json.loads(m.decode('utf-8')),
            group_id='fraudguard-ml-consumers',
            auto_offset_reset='latest'
        )
        
        print("✅ Kafka consumer started - listening for transactions")
        
        for message in consumer:
            try:
                transaction = message.value
                result = process_transaction(transaction)
                
                # Publish fraud alert if detected
                if result['is_fraud'] and kafka_producer:
                    kafka_producer.send('fraud-alerts', {
                        'transaction_id': transaction.get('id'),
                        'risk_score': result['probability'],
                        'risk_level': result['risk_level'],
                        'detected_at': datetime.utcnow().isoformat()
                    })
                    kafka_producer.flush()
                
                metrics.kafka_messages_processed += 1
                
            except Exception as e:
                print(f"Error processing Kafka message: {e}")
                metrics.record_error()
                
    except Exception as e:
        print(f"Kafka consumer error: {e}")

def publish_event(topic, event):
    """Publish event to Kafka topic"""
    if kafka_producer:
        try:
            kafka_producer.send(topic, event)
            kafka_producer.flush()
        except Exception as e:
            print(f"Failed to publish event: {e}")

# ============================================================================
# OPTIONAL: REDIS CACHING
# ============================================================================
redis_client = None

def init_redis():
    global redis_client
    if not config['REDIS_ENABLED']:
        print("ℹ️ Redis disabled - running without cache")
        return
    
    try:
        import redis
        redis_client = redis.Redis(
            host=config['REDIS_HOST'],
            port=config['REDIS_PORT'],
            decode_responses=True
        )
        redis_client.ping()
        print(f"✅ Redis connected to {config['REDIS_HOST']}:{config['REDIS_PORT']}")
    except Exception as e:
        print(f"⚠️ Redis initialization failed: {e}")

def get_cached_prediction(cache_key):
    """Get prediction from cache"""
    if redis_client:
        try:
            cached = redis_client.get(f"pred:{cache_key}")
            if cached:
                metrics.cache_hits += 1
                return json.loads(cached)
        except:
            pass
    metrics.cache_misses += 1
    return None

def set_cached_prediction(cache_key, result, ttl=3600):
    """Cache prediction result"""
    if redis_client:
        try:
            redis_client.setex(f"pred:{cache_key}", ttl, json.dumps(result))
        except:
            pass

# ============================================================================
# CORE PREDICTION LOGIC
# ============================================================================
def process_transaction(data):
    """Process a single transaction and return fraud prediction"""
    start_time = time.time()
    
    try:
        # Extract values
        amount = data.get('amount', 0)
        time_value = data.get('time', 0)

        # Normalize amount
        amount_scaled = scaler.transform([[amount]])[0][0]

        # Temporal features
        hour = (time_value / 3600) % 24
        day = (time_value / 86400)

        # Build feature vector (same order as training)
        features = []
        
        # V1 to V28 (PCA components)
        for i in range(1, 29):
            features.append(data.get(f'v{i}', 0))
        
        # Additional features
        features.append(amount)           # Raw amount
        features.append(amount_scaled)    # Scaled amount
        features.append(hour)             # Hour of day
        features.append(day)              # Day
        features.append(amount_scaled * hour)  # Interaction term

        # Validate feature count
        if len(features) != 33:
            raise ValueError(f"Bad feature count: expected 33, got {len(features)}")

        # Make prediction
        X = np.array([features])
        prediction = model.predict(X)[0]
        probability = model.predict_proba(X)[0][1]

        # Determine risk level
        if probability >= 0.8:
            risk_level = "HIGH"
        elif probability >= 0.5:
            risk_level = "MEDIUM"
        else:
            risk_level = "LOW"

        result = {
            "is_fraud": bool(prediction),
            "probability": float(probability),
            "risk_level": risk_level
        }
        
        # Record metrics
        latency = time.time() - start_time
        metrics.record_prediction(result['is_fraud'], latency)
        
        return result
        
    except Exception as e:
        metrics.record_error()
        raise e

# ============================================================================
# API ENDPOINTS
# ============================================================================
@app.route('/health', methods=['GET'])
def health():
    """Health check endpoint"""
    return jsonify({
        "status": "healthy",
        "model_loaded": model_loaded,
        "model_version": config['MODEL_VERSION'],
        "service": config['SERVICE_NAME'],
        "kafka_enabled": config['KAFKA_ENABLED'],
        "redis_enabled": config['REDIS_ENABLED'],
        "timestamp": datetime.utcnow().isoformat()
    }), 200

@app.route('/metrics', methods=['GET'])
def get_metrics():
    """Prometheus-compatible metrics endpoint"""
    return jsonify(metrics.get_metrics()), 200

@app.route('/predict', methods=['POST'])
def predict():
    """
    Predict if a transaction is fraudulent.
    
    Input JSON:
    {
        "amount": 1500.50,
        "time": 12345,
        "v1": ..., "v2": ..., ... "v28": ...
    }
    
    Output JSON:
    {
        "is_fraud": true/false,
        "probability": 0.85,
        "risk_level": "HIGH"/"MEDIUM"/"LOW"
    }
    """
    try:
        if not model_loaded:
            return jsonify({"error": "Model not loaded"}), 503
        
        data = request.get_json()
        
        # Generate cache key from transaction data
        cache_key = hash(json.dumps(data, sort_keys=True))
        
        # Check cache
        cached_result = get_cached_prediction(cache_key)
        if cached_result:
            cached_result['cached'] = True
            return jsonify(cached_result), 200
        
        # Process transaction
        result = process_transaction(data)
        
        # Cache result
        set_cached_prediction(cache_key, result)
        
        # Publish to Kafka if fraud detected
        if result['is_fraud'] and kafka_producer:
            publish_event('fraud-detected', {
                'amount': data.get('amount'),
                'risk_score': result['probability'],
                'risk_level': result['risk_level'],
                'detected_at': datetime.utcnow().isoformat()
            })
        
        result['cached'] = False
        return jsonify(result), 200
        
    except Exception as e:
        return jsonify({"error": str(e)}), 400

@app.route('/predict/batch', methods=['POST'])
def predict_batch():
    """
    Batch prediction for multiple transactions.
    
    Input JSON:
    {
        "transactions": [
            {"amount": 100, "time": 12345, "v1": ...},
            {"amount": 200, "time": 12346, "v1": ...},
            ...
        ]
    }
    """
    try:
        if not model_loaded:
            return jsonify({"error": "Model not loaded"}), 503
        
        data = request.get_json()
        transactions = data.get('transactions', [])
        
        if not transactions:
            return jsonify({"error": "No transactions provided"}), 400
        
        results = []
        fraud_count = 0
        
        for tx in transactions:
            try:
                result = process_transaction(tx)
                if result['is_fraud']:
                    fraud_count += 1
                results.append({
                    "index": len(results),
                    **result
                })
            except Exception as e:
                results.append({
                    "index": len(results),
                    "error": str(e)
                })
        
        return jsonify({
            "total": len(transactions),
            "processed": len(results),
            "fraud_detected": fraud_count,
            "fraud_rate": fraud_count / len(transactions) if transactions else 0,
            "results": results
        }), 200
        
    except Exception as e:
        return jsonify({"error": str(e)}), 400

@app.route('/model/info', methods=['GET'])
def model_info():
    """Get information about the loaded model"""
    if not model_loaded:
        return jsonify({"error": "Model not loaded"}), 503
    
    return jsonify({
        "model_type": type(model).__name__,
        "model_version": config['MODEL_VERSION'],
        "features_expected": 33,
        "feature_names": [f"v{i}" for i in range(1, 29)] + 
                         ["amount", "amount_scaled", "hour", "day", "amount_hour"],
        "classes": ["legitimate", "fraud"],
        "loaded_at": datetime.utcnow().isoformat()
    }), 200

@app.route('/analytics/summary', methods=['GET'])
def analytics_summary():
    """Get analytics summary"""
    m = metrics.get_metrics()
    return jsonify({
        "service_stats": m,
        "fraud_analysis": {
            "total_analyzed": m['predictions_total'],
            "fraud_detected": m['fraud_detected_total'],
            "fraud_rate_percent": round(m['fraud_rate'] * 100, 2),
            "average_response_time_ms": m['avg_prediction_latency_ms']
        },
        "cache_performance": {
            "hits": m['cache_hits'],
            "misses": m['cache_misses'],
            "hit_rate": round(m['cache_hits'] / (m['cache_hits'] + m['cache_misses']) * 100, 2)
                        if (m['cache_hits'] + m['cache_misses']) > 0 else 0
        }
    }), 200

# ============================================================================
# STARTUP
# ============================================================================
if __name__ == '__main__':
    print("=" * 60)
    print("  FraudGuard ML Service - Enterprise Edition")
    print("=" * 60)
    print(f"  Model Version: {config['MODEL_VERSION']}")
    print(f"  Kafka Enabled: {config['KAFKA_ENABLED']}")
    print(f"  Redis Enabled: {config['REDIS_ENABLED']}")
    print("=" * 60)
    
    # Initialize integrations
    init_kafka()
    init_redis()
    
    print("\n🚀 Starting Flask server on http://0.0.0.0:5000")
    app.run(host='0.0.0.0', port=5000, debug=False, threaded=True)
