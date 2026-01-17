import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../../../services/dashboard.service';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {
  statistics: any = null;
  recentSuspicious: any[] = [];
  pendingAlerts: any[] = [];
  highRiskAccounts: any[] = [];
  loading = true;
  error = '';
  lastUpdated = new Date();

  constructor(private dashboardService: DashboardService) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.loading = true;
    this.error = '';

    // Load all dashboard data
    this.dashboardService.getStatistics().subscribe({
      next: (data) => {
        this.statistics = data;
        this.lastUpdated = new Date();
      },
      error: (err) => {
        this.error = 'Failed to load statistics';
        console.error(err);
      }
    });

    this.dashboardService.getRecentSuspiciousTransactions(10).subscribe({
      next: (data) => {
        this.recentSuspicious = data;
      },
      error: (err) => {
        console.error(err);
      }
    });

    this.dashboardService.getPendingAlerts(10).subscribe({
      next: (data) => {
        this.pendingAlerts = data;
      },
      error: (err) => {
        console.error(err);
      }
    });

    this.dashboardService.getHighRiskAccounts(10).subscribe({
      next: (data) => {
        this.highRiskAccounts = data;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }

  refresh() {
    this.loadDashboardData();
  }

  formatAmount(amount: string | number): string {
    const num = typeof amount === 'string' ? parseFloat(amount) : amount;
    if (num >= 1000000) {
      return (num / 1000000).toFixed(2) + 'M';
    } else if (num >= 1000) {
      return (num / 1000).toFixed(1) + 'K';
    }
    return num.toFixed(2);
  }
}
