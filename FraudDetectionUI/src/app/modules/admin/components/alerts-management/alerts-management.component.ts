import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../../../services/dashboard.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-alerts-management',
  templateUrl: './alerts-management.component.html',
  styleUrls: ['./alerts-management.component.scss']
})
export class AlertsManagementComponent implements OnInit {
  alerts: any[] = [];
  loading = true;
  error = '';
  selectedAlert: any = null;
  showDetailModal = false;

  private apiUrl = `${environment.apiUrl}/FraudAlert`;

  constructor(
    private dashboardService: DashboardService,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.loadAlerts();
  }

  loadAlerts() {
    this.loading = true;
    this.dashboardService.getPendingAlerts(200).subscribe({
      next: (data) => {
        this.alerts = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Échec du chargement des alertes';
        this.loading = false;
        console.error(err);
      }
    });
  }

  getRiskLevel(score: number): string {
    if (score >= 0.8) return 'HIGH';
    if (score >= 0.5) return 'MEDIUM';
    return 'LOW';
  }

  getAlertsByRisk(level: string): any[] {
    return this.alerts.filter(alert => this.getRiskLevel(alert.riskScore) === level);
  }

  reviewAlert(alert: any) {
    this.selectedAlert = alert;
    this.showDetailModal = true;
  }

  closeModal() {
    this.showDetailModal = false;
    this.selectedAlert = null;
  }

  resolveAlert(alert: any) {
    if (confirm(`Voulez-vous vraiment marquer l'alerte #${alert.id} comme résolue ?`)) {
      this.http.put(`${this.apiUrl}/${alert.id}/status`, { status: 'Resolved' }).subscribe({
        next: () => {
          // Remove from list or update status
          this.alerts = this.alerts.filter(a => a.id !== alert.id);
          this.closeModal();
        },
        error: (err) => {
          console.error('Error resolving alert:', err);
          alert('Erreur lors de la résolution de l\'alerte');
        }
      });
    }
  }

  dismissAlert(alertItem: any) {
    if (confirm(`Voulez-vous vraiment rejeter l'alerte #${alertItem.id} ?`)) {
      this.http.put(`${this.apiUrl}/${alertItem.id}/status`, { status: 'Dismissed' }).subscribe({
        next: () => {
          this.alerts = this.alerts.filter(a => a.id !== alertItem.id);
          this.closeModal();
        },
        error: (err) => {
          console.error('Error dismissing alert:', err);
        }
      });
    }
  }

  refresh() {
    this.loadAlerts();
  }
}
