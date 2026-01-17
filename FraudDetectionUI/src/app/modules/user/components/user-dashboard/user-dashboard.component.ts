import { Component, OnInit } from '@angular/core';
import { TransactionService, UserStats } from '../../../../services/transaction.service';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-user-dashboard',
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.scss']
})
export class UserDashboardComponent implements OnInit {
  userStats: UserStats | null = null;
  loading = true;
  error = '';
  currentUser: any;

  constructor(
    private transactionService: TransactionService,
    private authService: AuthService
  ) {
    this.currentUser = this.authService.getCurrentUser();
  }

  ngOnInit() {
    this.loadUserDashboard();
  }

  loadUserDashboard() {
    this.loading = true;
    this.error = '';
    
    this.transactionService.getMyStats().subscribe({
      next: (data) => {
        this.userStats = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load dashboard data. Please try again.';
        this.loading = false;
        console.error(err);
      }
    });
  }

  refresh() {
    this.loadUserDashboard();
  }
}
