import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';

interface AdminProfile {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  createdAt: string;
}

interface SystemStats {
  totalUsers: number;
  totalTransactions: number;
  totalAlerts: number;
  fraudDetectionRate: number;
}

@Component({
  selector: 'app-admin-profile',
  templateUrl: './admin-profile.component.html',
  styleUrls: ['./admin-profile.component.scss']
})
export class AdminProfileComponent implements OnInit {
  profile: AdminProfile | null = null;
  systemStats: SystemStats | null = null;
  loading = false;
  error = '';
  success = '';

  // Edit mode
  isEditing = false;
  editForm = {
    firstName: '',
    lastName: '',
    email: ''
  };

  // Change password
  showPasswordModal = false;
  passwordForm = {
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  };
  passwordError = '';

  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loadProfile();
    this.loadSystemStats();
  }

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  loadProfile() {
    this.loading = true;
    this.http.get<AdminProfile>(`${this.apiUrl}/User/profile`, { headers: this.getHeaders() })
      .subscribe({
        next: (profile) => {
          this.profile = profile;
          this.editForm = {
            firstName: profile.firstName,
            lastName: profile.lastName,
            email: profile.email
          };
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load profile';
          this.loading = false;
          console.error(err);
        }
      });
  }

  loadSystemStats() {
    this.http.get<any>(`${this.apiUrl}/Dashboard/statistics`, { headers: this.getHeaders() })
      .subscribe({
        next: (stats) => {
          this.systemStats = {
            totalUsers: stats.totalUsers,
            totalTransactions: stats.totalTransactions,
            totalAlerts: stats.pendingAlerts,
            fraudDetectionRate: parseFloat(stats.fraudPercentage) || 0
          };
        },
        error: (err) => console.error('Failed to load stats', err)
      });
  }

  startEditing() {
    this.isEditing = true;
    if (this.profile) {
      this.editForm = {
        firstName: this.profile.firstName,
        lastName: this.profile.lastName,
        email: this.profile.email
      };
    }
  }

  cancelEditing() {
    this.isEditing = false;
    if (this.profile) {
      this.editForm = {
        firstName: this.profile.firstName,
        lastName: this.profile.lastName,
        email: this.profile.email
      };
    }
  }

  saveProfile() {
    this.loading = true;
    this.error = '';

    this.http.put(`${this.apiUrl}/User/profile`, this.editForm, { headers: this.getHeaders() })
      .subscribe({
        next: () => {
          this.success = 'Profile updated successfully!';
          this.isEditing = false;
          this.loadProfile();
          setTimeout(() => this.success = '', 3000);
        },
        error: (err) => {
          this.error = err.error?.message || 'Failed to update profile';
          this.loading = false;
        }
      });
  }

  openPasswordModal() {
    this.passwordForm = {
      currentPassword: '',
      newPassword: '',
      confirmPassword: ''
    };
    this.passwordError = '';
    this.showPasswordModal = true;
  }

  closePasswordModal() {
    this.showPasswordModal = false;
    this.passwordError = '';
  }

  changePassword() {
    this.passwordError = '';

    if (this.passwordForm.newPassword !== this.passwordForm.confirmPassword) {
      this.passwordError = 'New passwords do not match';
      return;
    }

    if (this.passwordForm.newPassword.length < 6) {
      this.passwordError = 'Password must be at least 6 characters';
      return;
    }

    this.loading = true;
    this.http.put(`${this.apiUrl}/User/change-password`, {
      currentPassword: this.passwordForm.currentPassword,
      newPassword: this.passwordForm.newPassword
    }, { headers: this.getHeaders() })
      .subscribe({
        next: () => {
          this.success = 'Password changed successfully!';
          this.closePasswordModal();
          this.loading = false;
          setTimeout(() => this.success = '', 3000);
        },
        error: (err) => {
          this.passwordError = err.error?.message || 'Failed to change password';
          this.loading = false;
        }
      });
  }

  getInitials(): string {
    if (!this.profile) return '?';
    return `${this.profile.firstName.charAt(0)}${this.profile.lastName.charAt(0)}`;
  }

  getMemberSince(): string {
    if (!this.profile) return '';
    const date = new Date(this.profile.createdAt);
    return date.toLocaleDateString('en-US', { month: 'long', year: 'numeric' });
  }
}
