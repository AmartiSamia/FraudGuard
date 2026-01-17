import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../../../services/dashboard.service';

@Component({
  selector: 'app-suspicious-transactions',
  templateUrl: './suspicious-transactions.component.html',
  styleUrls: ['./suspicious-transactions.component.scss']
})
export class SuspiciousTransactionsComponent implements OnInit {
  suspiciousTransactions: any[] = [];
  loading = true;
  error = '';

  constructor(private dashboardService: DashboardService) {}

  ngOnInit() {
    this.loadSuspiciousTransactions();
  }

  loadSuspiciousTransactions() {
    this.loading = true;
    this.dashboardService.getRecentSuspiciousTransactions(100).subscribe({
      next: (data) => {
        this.suspiciousTransactions = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load suspicious transactions';
        this.loading = false;
        console.error(err);
      }
    });
  }

  getTotalAmount(): number {
    return this.suspiciousTransactions.reduce((sum, t) => sum + t.amount, 0);
  }

  getUniqueCountries(): number {
    const countries = new Set(this.suspiciousTransactions.map(t => t.country));
    return countries.size;
  }

  refresh() {
    this.loadSuspiciousTransactions();
  }
}
