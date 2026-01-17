"""
FraudGuard ETL Pipeline
========================
Enterprise-grade ETL (Extract, Transform, Load) pipeline for:
- Extracting transaction data from multiple sources
- Transforming and enriching data for ML models
- Loading data into data warehouse/analytics platforms
- Generating reports for Power BI and other BI tools
"""

import pandas as pd
import numpy as np
import json
import os
from datetime import datetime, timedelta
from typing import Dict, List, Optional, Tuple
import logging

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

class FraudDetectionETL:
    """
    Enterprise ETL Pipeline for Fraud Detection System
    
    Features:
    - Multi-source data extraction
    - Data quality validation
    - Feature engineering for ML
    - Incremental loading support
    - Audit trail and lineage tracking
    """
    
    def __init__(self, config: Optional[Dict] = None):
        self.config = config or {}
        self.stats = {
            'records_extracted': 0,
            'records_transformed': 0,
            'records_loaded': 0,
            'records_rejected': 0,
            'start_time': None,
            'end_time': None
        }
        self.validation_errors = []
        
    # =========================================================================
    # EXTRACT PHASE
    # =========================================================================
    def extract_from_csv(self, file_path: str) -> pd.DataFrame:
        """Extract data from CSV file"""
        logger.info(f"📥 Extracting data from: {file_path}")
        try:
            df = pd.read_csv(file_path)
            self.stats['records_extracted'] = len(df)
            logger.info(f"   ✓ Extracted {len(df):,} records")
            return df
        except Exception as e:
            logger.error(f"   ✗ Extraction failed: {e}")
            raise
    
    def extract_from_database(self, connection_string: str, query: str) -> pd.DataFrame:
        """Extract data from SQL database"""
        logger.info("📥 Extracting data from database...")
        try:
            import pyodbc
            conn = pyodbc.connect(connection_string)
            df = pd.read_sql(query, conn)
            conn.close()
            self.stats['records_extracted'] = len(df)
            logger.info(f"   ✓ Extracted {len(df):,} records from database")
            return df
        except Exception as e:
            logger.error(f"   ✗ Database extraction failed: {e}")
            raise
    
    def extract_from_api(self, api_url: str, params: Optional[Dict] = None) -> pd.DataFrame:
        """Extract data from REST API"""
        logger.info(f"📥 Extracting data from API: {api_url}")
        try:
            import requests
            response = requests.get(api_url, params=params)
            response.raise_for_status()
            data = response.json()
            df = pd.DataFrame(data)
            self.stats['records_extracted'] = len(df)
            logger.info(f"   ✓ Extracted {len(df):,} records from API")
            return df
        except Exception as e:
            logger.error(f"   ✗ API extraction failed: {e}")
            raise
    
    def extract_from_kafka(self, topic: str, bootstrap_servers: str, 
                          max_messages: int = 10000) -> pd.DataFrame:
        """Extract data from Kafka topic"""
        logger.info(f"📥 Extracting data from Kafka topic: {topic}")
        try:
            from kafka import KafkaConsumer
            consumer = KafkaConsumer(
                topic,
                bootstrap_servers=bootstrap_servers,
                auto_offset_reset='earliest',
                value_deserializer=lambda m: json.loads(m.decode('utf-8')),
                consumer_timeout_ms=10000
            )
            
            messages = []
            for message in consumer:
                messages.append(message.value)
                if len(messages) >= max_messages:
                    break
            
            consumer.close()
            df = pd.DataFrame(messages)
            self.stats['records_extracted'] = len(df)
            logger.info(f"   ✓ Extracted {len(df):,} records from Kafka")
            return df
        except Exception as e:
            logger.error(f"   ✗ Kafka extraction failed: {e}")
            raise
    
    # =========================================================================
    # TRANSFORM PHASE
    # =========================================================================
    def validate_data(self, df: pd.DataFrame) -> Tuple[pd.DataFrame, pd.DataFrame]:
        """Validate data quality and separate valid/invalid records"""
        logger.info("🔍 Validating data quality...")
        
        valid_mask = pd.Series([True] * len(df))
        errors = []
        
        # Check for required columns
        required_columns = ['Amount', 'Time']
        for col in required_columns:
            if col not in df.columns:
                # Try lowercase
                if col.lower() in df.columns:
                    df = df.rename(columns={col.lower(): col})
                else:
                    logger.warning(f"   ⚠ Missing required column: {col}")
        
        # Validate Amount (must be positive)
        if 'Amount' in df.columns:
            invalid_amount = df['Amount'] < 0
            if invalid_amount.any():
                errors.append(f"Negative amounts: {invalid_amount.sum()}")
                valid_mask &= ~invalid_amount
        
        # Validate Time (must be non-negative)
        if 'Time' in df.columns:
            invalid_time = df['Time'] < 0
            if invalid_time.any():
                errors.append(f"Negative time values: {invalid_time.sum()}")
                valid_mask &= ~invalid_time
        
        # Check for duplicates
        duplicates = df.duplicated()
        if duplicates.any():
            errors.append(f"Duplicate records: {duplicates.sum()}")
            valid_mask &= ~duplicates
        
        # Check for null values in critical columns
        for col in df.columns:
            null_count = df[col].isnull().sum()
            if null_count > 0:
                errors.append(f"Null values in {col}: {null_count}")
        
        valid_df = df[valid_mask].copy()
        invalid_df = df[~valid_mask].copy()
        
        self.validation_errors = errors
        self.stats['records_rejected'] = len(invalid_df)
        
        logger.info(f"   ✓ Valid records: {len(valid_df):,}")
        logger.info(f"   ✗ Invalid records: {len(invalid_df):,}")
        
        return valid_df, invalid_df
    
    def engineer_features(self, df: pd.DataFrame) -> pd.DataFrame:
        """Create features for ML model"""
        logger.info("⚙️ Engineering features...")
        
        df = df.copy()
        
        # Standardize column names
        df.columns = df.columns.str.lower()
        
        # Time-based features
        if 'time' in df.columns:
            df['hour'] = (df['time'] / 3600) % 24
            df['day'] = df['time'] / 86400
            df['is_night'] = ((df['hour'] >= 22) | (df['hour'] <= 6)).astype(int)
            df['is_weekend'] = (df['day'] % 7 >= 5).astype(int)
        
        # Amount-based features
        if 'amount' in df.columns:
            df['amount_log'] = np.log1p(df['amount'])
            df['amount_zscore'] = (df['amount'] - df['amount'].mean()) / df['amount'].std()
            df['is_large_amount'] = (df['amount'] > df['amount'].quantile(0.95)).astype(int)
            
            # Amount-time interaction
            if 'hour' in df.columns:
                df['amount_hour_interaction'] = df['amount_log'] * df['hour']
        
        # Rolling statistics (if sorted by time)
        if 'amount' in df.columns and len(df) > 10:
            df['amount_rolling_mean'] = df['amount'].rolling(window=10, min_periods=1).mean()
            df['amount_rolling_std'] = df['amount'].rolling(window=10, min_periods=1).std().fillna(0)
            df['amount_deviation'] = df['amount'] - df['amount_rolling_mean']
        
        # Velocity features (transactions per time period)
        if 'time' in df.columns:
            df['hour_bin'] = (df['time'] // 3600).astype(int)
            hour_counts = df.groupby('hour_bin').size()
            df['transactions_this_hour'] = df['hour_bin'].map(hour_counts)
        
        self.stats['records_transformed'] = len(df)
        logger.info(f"   ✓ Engineered {len(df.columns)} features")
        
        return df
    
    def normalize_features(self, df: pd.DataFrame, 
                          columns: Optional[List[str]] = None) -> pd.DataFrame:
        """Normalize numerical features"""
        logger.info("📊 Normalizing features...")
        
        df = df.copy()
        
        if columns is None:
            columns = df.select_dtypes(include=[np.number]).columns.tolist()
        
        for col in columns:
            if col in df.columns:
                min_val = df[col].min()
                max_val = df[col].max()
                if max_val > min_val:
                    df[f'{col}_normalized'] = (df[col] - min_val) / (max_val - min_val)
        
        logger.info(f"   ✓ Normalized {len(columns)} columns")
        return df
    
    def aggregate_by_period(self, df: pd.DataFrame, 
                           period: str = 'hour') -> pd.DataFrame:
        """Aggregate data by time period for reporting"""
        logger.info(f"📈 Aggregating by {period}...")
        
        if 'hour_bin' not in df.columns and 'time' in df.columns:
            df['hour_bin'] = (df['time'] // 3600).astype(int)
        
        agg_funcs = {
            'amount': ['sum', 'mean', 'std', 'count'],
        }
        
        if 'class' in df.columns:
            agg_funcs['class'] = ['sum', 'mean']
        
        aggregated = df.groupby('hour_bin').agg(agg_funcs)
        aggregated.columns = ['_'.join(col).strip() for col in aggregated.columns.values]
        aggregated = aggregated.reset_index()
        
        logger.info(f"   ✓ Created {len(aggregated)} aggregated records")
        return aggregated
    
    # =========================================================================
    # LOAD PHASE
    # =========================================================================
    def load_to_csv(self, df: pd.DataFrame, output_path: str) -> None:
        """Load data to CSV file"""
        logger.info(f"📤 Loading data to: {output_path}")
        try:
            os.makedirs(os.path.dirname(output_path), exist_ok=True)
            df.to_csv(output_path, index=False)
            self.stats['records_loaded'] = len(df)
            logger.info(f"   ✓ Loaded {len(df):,} records to CSV")
        except Exception as e:
            logger.error(f"   ✗ CSV loading failed: {e}")
            raise
    
    def load_to_parquet(self, df: pd.DataFrame, output_path: str) -> None:
        """Load data to Parquet file (optimized for analytics)"""
        logger.info(f"📤 Loading data to Parquet: {output_path}")
        try:
            os.makedirs(os.path.dirname(output_path), exist_ok=True)
            df.to_parquet(output_path, index=False)
            self.stats['records_loaded'] = len(df)
            logger.info(f"   ✓ Loaded {len(df):,} records to Parquet")
        except Exception as e:
            logger.error(f"   ✗ Parquet loading failed: {e}")
            raise
    
    def load_to_database(self, df: pd.DataFrame, connection_string: str, 
                        table_name: str, if_exists: str = 'append') -> None:
        """Load data to SQL database"""
        logger.info(f"📤 Loading data to database table: {table_name}")
        try:
            from sqlalchemy import create_engine
            engine = create_engine(connection_string)
            df.to_sql(table_name, engine, if_exists=if_exists, index=False)
            self.stats['records_loaded'] = len(df)
            logger.info(f"   ✓ Loaded {len(df):,} records to {table_name}")
        except Exception as e:
            logger.error(f"   ✗ Database loading failed: {e}")
            raise
    
    def load_to_kafka(self, df: pd.DataFrame, topic: str, 
                     bootstrap_servers: str) -> None:
        """Load data to Kafka topic"""
        logger.info(f"📤 Loading data to Kafka topic: {topic}")
        try:
            from kafka import KafkaProducer
            producer = KafkaProducer(
                bootstrap_servers=bootstrap_servers,
                value_serializer=lambda v: json.dumps(v).encode('utf-8')
            )
            
            for _, row in df.iterrows():
                producer.send(topic, row.to_dict())
            
            producer.flush()
            producer.close()
            self.stats['records_loaded'] = len(df)
            logger.info(f"   ✓ Loaded {len(df):,} records to Kafka")
        except Exception as e:
            logger.error(f"   ✗ Kafka loading failed: {e}")
            raise
    
    def generate_powerbi_export(self, df: pd.DataFrame, 
                               output_dir: str) -> Dict[str, str]:
        """Generate exports for Power BI integration"""
        logger.info("📊 Generating Power BI exports...")
        
        os.makedirs(output_dir, exist_ok=True)
        exports = {}
        
        # Main transaction data
        main_path = os.path.join(output_dir, 'transactions.csv')
        df.to_csv(main_path, index=False)
        exports['transactions'] = main_path
        
        # Aggregated hourly data
        if 'hour_bin' in df.columns or 'time' in df.columns:
            hourly = self.aggregate_by_period(df, 'hour')
            hourly_path = os.path.join(output_dir, 'hourly_summary.csv')
            hourly.to_csv(hourly_path, index=False)
            exports['hourly_summary'] = hourly_path
        
        # Fraud summary
        if 'class' in df.columns:
            fraud_summary = pd.DataFrame({
                'metric': ['total_transactions', 'fraud_count', 'fraud_rate', 
                          'total_amount', 'fraud_amount'],
                'value': [
                    len(df),
                    df['class'].sum(),
                    df['class'].mean() * 100,
                    df['amount'].sum() if 'amount' in df.columns else 0,
                    df[df['class'] == 1]['amount'].sum() if 'amount' in df.columns else 0
                ]
            })
            summary_path = os.path.join(output_dir, 'fraud_summary.csv')
            fraud_summary.to_csv(summary_path, index=False)
            exports['fraud_summary'] = summary_path
        
        logger.info(f"   ✓ Generated {len(exports)} export files")
        return exports
    
    # =========================================================================
    # PIPELINE ORCHESTRATION
    # =========================================================================
    def run_pipeline(self, source_path: str, output_dir: str) -> Dict:
        """Run complete ETL pipeline"""
        logger.info("=" * 60)
        logger.info("  FraudGuard ETL Pipeline")
        logger.info("=" * 60)
        
        self.stats['start_time'] = datetime.now()
        
        try:
            # Extract
            df = self.extract_from_csv(source_path)
            
            # Validate
            valid_df, invalid_df = self.validate_data(df)
            
            # Save rejected records for audit
            if len(invalid_df) > 0:
                rejected_path = os.path.join(output_dir, 'rejected_records.csv')
                invalid_df.to_csv(rejected_path, index=False)
            
            # Transform
            transformed_df = self.engineer_features(valid_df)
            
            # Load
            self.load_to_csv(transformed_df, 
                           os.path.join(output_dir, 'transformed_data.csv'))
            
            # Generate Power BI exports
            self.generate_powerbi_export(transformed_df, 
                                        os.path.join(output_dir, 'powerbi'))
            
            self.stats['end_time'] = datetime.now()
            
            # Generate pipeline report
            report = self.generate_report()
            report_path = os.path.join(output_dir, 'pipeline_report.json')
            with open(report_path, 'w') as f:
                json.dump(report, f, indent=2, default=str)
            
            logger.info("=" * 60)
            logger.info("  Pipeline completed successfully!")
            logger.info("=" * 60)
            
            return report
            
        except Exception as e:
            self.stats['end_time'] = datetime.now()
            logger.error(f"Pipeline failed: {e}")
            raise
    
    def generate_report(self) -> Dict:
        """Generate pipeline execution report"""
        duration = (self.stats['end_time'] - self.stats['start_time']).total_seconds() \
                   if self.stats['end_time'] and self.stats['start_time'] else 0
        
        return {
            'pipeline_name': 'FraudGuard ETL Pipeline',
            'execution_stats': {
                'start_time': self.stats['start_time'],
                'end_time': self.stats['end_time'],
                'duration_seconds': round(duration, 2),
                'records_extracted': self.stats['records_extracted'],
                'records_transformed': self.stats['records_transformed'],
                'records_loaded': self.stats['records_loaded'],
                'records_rejected': self.stats['records_rejected']
            },
            'data_quality': {
                'validation_errors': self.validation_errors,
                'rejection_rate': round(
                    self.stats['records_rejected'] / self.stats['records_extracted'] * 100, 2
                ) if self.stats['records_extracted'] > 0 else 0
            },
            'status': 'SUCCESS'
        }


# =============================================================================
# MAIN EXECUTION
# =============================================================================
if __name__ == '__main__':
    import argparse
    
    parser = argparse.ArgumentParser(description='FraudGuard ETL Pipeline')
    parser.add_argument('--source', type=str, required=True, help='Source data file')
    parser.add_argument('--output', type=str, default='./output', help='Output directory')
    
    args = parser.parse_args()
    
    etl = FraudDetectionETL()
    report = etl.run_pipeline(args.source, args.output)
    
    print("\n📋 Pipeline Report:")
    print(json.dumps(report, indent=2, default=str))
