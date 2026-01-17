import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, switchMap, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface LoginResponse {
  token: string;
  user: {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
    role: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;
  private tokenKey = 'fraud_detection_token';
  private userKey = 'fraud_detection_user';

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  private userRoleSubject = new BehaviorSubject<string | null>(this.getUserRole());
  public userRole$ = this.userRoleSubject.asObservable();

  private userNameSubject = new BehaviorSubject<string | null>(this.getUserName());
  public userName$ = this.userNameSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http.post(`${this.apiUrl}/User/login`, { email, password }, { responseType: 'text' })
      .pipe(
        map(token => {
          // Decode JWT to get user info
          const payload = this.decodeToken(token);
          console.log('JWT Payload:', payload);
          console.log('All keys in payload:', Object.keys(payload));
          
          // Try different claim names for role
          const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] 
                    || payload['role'] 
                    || payload['Role']
                    || 'User';
          
          console.log('Detected role:', role);
          
          const user = {
            id: parseInt(payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || payload['sub'] || '0'),
            email: email,
            firstName: email.split('@')[0],
            lastName: '',
            role: role
          };

          console.log('User object:', user);

          localStorage.setItem(this.tokenKey, token);
          localStorage.setItem(this.userKey, JSON.stringify(user));
          this.isAuthenticatedSubject.next(true);
          this.userRoleSubject.next(user.role);
          this.userNameSubject.next(user.firstName);

          return { token, user };
        })
      );
  }

  private decodeToken(token: string): any {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      return JSON.parse(window.atob(base64));
    } catch {
      return {};
    }
  }

  register(userData: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/User/register`, userData);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.isAuthenticatedSubject.next(false);
    this.userRoleSubject.next(null);
    this.userNameSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    return this.hasToken();
  }

  getUserRole(): string | null {
    const user = localStorage.getItem(this.userKey);
    return user ? JSON.parse(user).role : null;
  }

  getUserName(): string | null {
    const user = localStorage.getItem(this.userKey);
    if (user) {
      const userData = JSON.parse(user);
      return `${userData.firstName} ${userData.lastName}`;
    }
    return null;
  }

  getCurrentUser(): any {
    const user = localStorage.getItem(this.userKey);
    return user ? JSON.parse(user) : null;
  }
}
