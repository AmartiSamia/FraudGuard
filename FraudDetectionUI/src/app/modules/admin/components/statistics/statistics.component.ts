import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../../../services/dashboard.service';
import { ChartConfiguration } from 'chart.js';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.scss']
})
export class StatisticsComponent implements OnInit {
  fraudByCountryChart: ChartConfiguration | null = null;
  fraudByDeviceChart: ChartConfiguration | null = null;
  fraudTrendChart: ChartConfiguration | null = null;
  loading = true;
  error = '';

  constructor(private dashboardService: DashboardService) {}

  ngOnInit() {
    this.loadStatistics();
  }

  loadStatistics() {
    this.loading = true;
    this.error = '';

    // Load fraud by country
    this.dashboardService.getFraudByCountry().subscribe({
      next: (data) => {
        this.setupFraudByCountryChart(data);
      },
      error: (err) => {
        console.error('Error loading fraud by country:', err);
      }
    });

    // Load fraud by device
    this.dashboardService.getFraudByDevice().subscribe({
      next: (data) => {
        this.setupFraudByDeviceChart(data);
      },
      error: (err) => {
        console.error('Error loading fraud by device:', err);
      }
    });

    // Load fraud trend
    this.dashboardService.getFraudByPeriod(30).subscribe({
      next: (data) => {
        this.setupFraudTrendChart(data);
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading fraud trend:', err);
        this.loading = false;
      }
    });
  }

  setupFraudByCountryChart(data: any[]) {
    const labels = data.map(d => d.country);
    const fraudCounts = data.map(d => d.fraudCount);
    
    this.fraudByCountryChart = {
      type: 'bar',
      data: {
        labels: labels,
        datasets: [{
          label: 'Fraud Count by Country',
          data: fraudCounts,
          backgroundColor: '#dc3545'
        }]
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            display: true
          }
        }
      }
    };
  }

  setupFraudByDeviceChart(data: any[]) {
    const labels = data.map(d => d.device);
    const fraudCounts = data.map(d => d.fraudCount);
    
    this.fraudByDeviceChart = {
      type: 'doughnut',
      data: {
        labels: labels,
        datasets: [{
          data: fraudCounts,
          backgroundColor: ['#667eea', '#764ba2', '#f093fb', '#4facfe']
        }]
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            display: true
          }
        }
      }
    };
  }

  setupFraudTrendChart(data: any[]) {
    const labels = data.map(d => new Date(d.date).toLocaleDateString());
    const fraudCounts = data.map(d => d.fraudCount);
    
    this.fraudTrendChart = {
      type: 'line',
      data: {
        labels: labels,
        datasets: [{
          label: 'Daily Fraud Transactions',
          data: fraudCounts,
          borderColor: '#667eea',
          backgroundColor: 'rgba(102, 126, 234, 0.1)',
          tension: 0.4
        }]
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            display: true
          }
        }
      }
    };
  }

  refresh() {
    this.loadStatistics();
  }
}
