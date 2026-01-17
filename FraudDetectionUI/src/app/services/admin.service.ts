import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResponse, Transaction } from './transaction.service';
import { environment } from '../../environments/environment';

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  createdAt: string;
  accountCount?: number;
  transactionCount?: number;
}

export interface UserDetail {
  user: User & {
    accounts: {
      id: number;
      accountNumber: string;
      balance: number;
      createdAt: string;
    }[];
  };
  stats: {
    totalTransactions: number;
    fraudTransactions: number;
    totalAmount: number;
    totalBalance: number;
  };
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role?: string;
  createDefaultAccount?: boolean;
  initialBalance?: number;
}

export interface UpdateUserRequest {
  firstName?: string;
  lastName?: string;
  email?: string;
  password?: string;
  role?: string;
}

export interface AdminTransaction {
  id: number;
  accountId: number;
  accountNumber: string;
  userEmail: string;
  userName: string;
  amount: number;
  type: string;
  country: string;
  device: string;
  timestamp: string;
  isFraud: boolean;
  fraudReason?: string;
}

export interface TransactionDetail {
  transaction: AdminTransaction & {
    accountBalance: number;
    user: { id: number; email: string; name: string };
    fraudAlert: {
      id: number;
      riskScore: number;
      status: string;
      createdAt: string;
      updatedAt: string;
    } | null;
  };
  relatedTransactions: {
    id: number;
    amount: number;
    type: string;
    timestamp: string;
    isFraud: boolean;
  }[];
}

export interface Analytics {
  overview: {
    totalUsers: number;
    totalTransactions: number;
    totalFraud: number;
    fraudRate: number;
    totalAmount: number;
  };
  last30Days: {
    transactions: number;
    fraud: number;
    fraudRate: number;
  };
  last7Days: {
    transactions: number;
    fraud: number;
    fraudRate: number;
  };
  fraudByCountry: { country: string; count: number; amount: number }[];
  fraudByDevice: { device: string; count: number }[];
  dailyTrend: { date: string; total: number; fraud: number; amount: number }[];
  highRiskUsers: { id: number; email: string; name: string; fraudCount: number; totalTrans: number }[];
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = `${environment.apiUrl}/Admin`;

  constructor(private http: HttpClient) {}

  // ============================================
  // USER MANAGEMENT
  // ============================================

  getUsers(page: number = 1, pageSize: number = 20, role?: string, search?: string): Observable<PaginatedResponse<User>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    if (role) params = params.set('role', role);
    if (search) params = params.set('search', search);

    return this.http.get<PaginatedResponse<User>>(`${this.apiUrl}/users`, { params });
  }

  getUser(id: number): Observable<UserDetail> {
    return this.http.get<UserDetail>(`${this.apiUrl}/users/${id}`);
  }

  createUser(data: CreateUserRequest): Observable<{ message: string; userId: number }> {
    return this.http.post<{ message: string; userId: number }>(`${this.apiUrl}/users`, data);
  }

  updateUser(id: number, data: UpdateUserRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/users/${id}`, data);
  }

  deleteUser(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/users/${id}`);
  }

  // ============================================
  // TRANSACTION MANAGEMENT
  // ============================================

  getTransactions(
    page: number = 1,
    pageSize: number = 50,
    filters?: {
      isFraud?: boolean;
      country?: string;
      type?: string;
      minAmount?: number;
      maxAmount?: number;
      startDate?: string;
      endDate?: string;
    }
  ): Observable<PaginatedResponse<AdminTransaction>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    if (filters) {
      if (filters.isFraud !== undefined) params = params.set('isFraud', filters.isFraud.toString());
      if (filters.country) params = params.set('country', filters.country);
      if (filters.type) params = params.set('type', filters.type);
      if (filters.minAmount !== undefined) params = params.set('minAmount', filters.minAmount.toString());
      if (filters.maxAmount !== undefined) params = params.set('maxAmount', filters.maxAmount.toString());
      if (filters.startDate) params = params.set('startDate', filters.startDate);
      if (filters.endDate) params = params.set('endDate', filters.endDate);
    }

    return this.http.get<PaginatedResponse<AdminTransaction>>(`${this.apiUrl}/transactions`, { params });
  }

  getTransaction(id: number): Observable<TransactionDetail> {
    return this.http.get<TransactionDetail>(`${this.apiUrl}/transactions/${id}`);
  }

  updateTransaction(id: number, data: { isFraud?: boolean; fraudReason?: string }): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/transactions/${id}`, data);
  }

  deleteTransaction(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/transactions/${id}`);
  }

  // ============================================
  // ANALYTICS
  // ============================================

  getAnalytics(): Observable<Analytics> {
    return this.http.get<Analytics>(`${this.apiUrl}/analytics`);
  }

  exportData(type: 'transactions' | 'users' | 'fraud-summary', startDate?: string, endDate?: string): Observable<any[]> {
    let params = new HttpParams();
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);

    return this.http.get<any[]>(`${this.apiUrl}/export/${type}`, { params });
  }
}
