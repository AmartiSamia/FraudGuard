import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = `${environment.apiUrl}/Dashboard`;

  constructor(private http: HttpClient) {}

  getStatistics(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/statistics`);
  }

  getFraudByPeriod(days: number = 30): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/fraud-by-period`, { params: { days } });
  }

  getFraudByCountry(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/fraud-by-country`);
  }

  getFraudByDevice(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/fraud-by-device`);
  }

  getUserStatistics(accountId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/user-statistics/${accountId}`);
  }

  getRecentSuspiciousTransactions(limit: number = 20): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/recent-suspicious`, { params: { limit } });
  }

  getPendingAlerts(limit: number = 50): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/pending-alerts`, { params: { limit } });
  }

  getHighRiskAccounts(limit: number = 20): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/high-risk-accounts`, { params: { limit } });
  }
}
