import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { SuspiciousTransactionsComponent } from './components/suspicious-transactions/suspicious-transactions.component';
import { AlertsManagementComponent } from './components/alerts-management/alerts-management.component';
import { StatisticsComponent } from './components/statistics/statistics.component';
import { UserManagementComponent } from './components/user-management/user-management.component';
import { TransactionManagementComponent } from './components/transaction-management/transaction-management.component';
import { AdminProfileComponent } from './components/admin-profile/admin-profile.component';

const routes: Routes = [
  { path: 'dashboard', component: AdminDashboardComponent },
  { path: 'suspicious', component: SuspiciousTransactionsComponent },
  { path: 'alerts', component: AlertsManagementComponent },
  { path: 'statistics', component: StatisticsComponent },
  { path: 'users', component: UserManagementComponent },
  { path: 'transactions', component: TransactionManagementComponent },
  { path: 'profile', component: AdminProfileComponent },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
