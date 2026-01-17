# FraudDetectionML Dockerfile
# Python ML service for fraud prediction

FROM python:3.11-slim

WORKDIR /app

# Install system dependencies
RUN apt-get update && apt-get install -y \
    gcc \
    g++ \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copy requirements and install Python dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Install additional packages
RUN pip install --no-cache-dir kafka-python redis prometheus-client xgboost

# Create necessary directories
RUN mkdir -p ./models ./data

# Copy all source code and data
COPY src/ ./src/
COPY models/ ./models/
COPY data/ ./data/

# Environment variables
ENV FLASK_APP=src/app.py
ENV FLASK_ENV=production
ENV PYTHONUNBUFFERED=1

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Expose port
EXPOSE 5000

# Entry point
CMD ["python", "src/app.py"]
