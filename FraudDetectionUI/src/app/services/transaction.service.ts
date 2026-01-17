import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Transaction {
  id: number;
  accountId: number;
  accountNumber?: string;
  amount: number;
  type: string;
  country: string;
  device: string;
  recipientRIB?: string;
  description?: string;
  timestamp: string;
  isFraud: boolean;
  fraudReason?: string;
}

export interface Account {
  id: number;
  accountNumber: string;
  balance: number;
  createdAt: string;
  transactionCount?: number;
}

export interface UserStats {
  totalAccounts: number;
  totalBalance: number;
  totalTransactions: number;
  fraudTransactions: number;
  fraudPercentage: string;
  totalAmount: number;
  fraudAmount: number;
  recentTransactions: Transaction[];
}

export interface CreateTransactionRequest {
  accountId: number;
  amount: number;
  type: string;
  country: string;
  device: string;
  recipientRIB?: string;
  description?: string;
}

export interface PaginatedResponse<T> {
  data: T[];
  pagination: {
    currentPage: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
  };
  counts?: {
    all: number;
    suspicious: number;
  };
}

export interface UserProfile {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  createdAt: string;
  stats: {
    accountCount: number;
    totalBalance: number;
    totalTransactions: number;
    fraudTransactions: number;
    safeTransactions: number;
  };
  accounts: Account[];
}

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private apiUrl = `${environment.apiUrl}/Transaction`;
  private userApiUrl = `${environment.apiUrl}/User`;

  constructor(private http: HttpClient) {}

  // Get current user's transactions with pagination
  getMyTransactions(page: number = 1, pageSize: number = 20, isFraud?: boolean, type?: string): Observable<PaginatedResponse<Transaction>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    if (isFraud !== undefined) params = params.set('isFraud', isFraud.toString());
    if (type) params = params.set('type', type);

    return this.http.get<PaginatedResponse<Transaction>>(`${this.apiUrl}/my-transactions`, { params });
  }

  // Get current user's accounts
  getMyAccounts(): Observable<Account[]> {
    return this.http.get<Account[]>(`${this.apiUrl}/my-accounts`);
  }

  // Get current user's dashboard stats
  getMyStats(): Observable<UserStats> {
    return this.http.get<UserStats>(`${this.apiUrl}/my-stats`);
  }

  // Create a new transaction
  createTransaction(data: CreateTransactionRequest): Observable<Transaction> {
    return this.http.post<Transaction>(`${this.apiUrl}/create`, data);
  }

  // Get user profile
  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.userApiUrl}/profile`);
  }

  // Update user profile
  updateProfile(data: { firstName?: string; lastName?: string; email?: string }): Observable<any> {
    return this.http.put(`${this.userApiUrl}/profile`, data);
  }

  // Change password
  changePassword(currentPassword: string, newPassword: string): Observable<any> {
    return this.http.put(`${this.userApiUrl}/change-password`, { currentPassword, newPassword });
  }

  // Legacy methods for compatibility
  getTransactions(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(`${this.apiUrl}/my-transactions`);
  }

  getTransactionById(id: number): Observable<Transaction> {
    return this.http.get<Transaction>(`${this.apiUrl}/${id}`);
  }
}
