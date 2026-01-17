import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminRoutingModule } from './admin-routing.module';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { SuspiciousTransactionsComponent } from './components/suspicious-transactions/suspicious-transactions.component';
import { AlertsManagementComponent } from './components/alerts-management/alerts-management.component';
import { StatisticsComponent } from './components/statistics/statistics.component';
import { UserManagementComponent } from './components/user-management/user-management.component';
import { TransactionManagementComponent } from './components/transaction-management/transaction-management.component';
import { AdminProfileComponent } from './components/admin-profile/admin-profile.component';
import { NgChartsModule } from 'ng2-charts';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [
    AdminDashboardComponent,
    SuspiciousTransactionsComponent,
    AlertsManagementComponent,
    StatisticsComponent,
    UserManagementComponent,
    TransactionManagementComponent,
    AdminProfileComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    AdminRoutingModule,
    NgChartsModule,
    HttpClientModule
  ]
})
export class AdminModule { }
