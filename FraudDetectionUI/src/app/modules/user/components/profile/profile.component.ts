import { Component, OnInit } from '@angular/core';
import { TransactionService, UserProfile } from '../../../../services/transaction.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  profile: UserProfile | null = null;
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

  constructor(private transactionService: TransactionService) {}

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.loading = true;
    this.transactionService.getProfile().subscribe({
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

    this.transactionService.updateProfile(this.editForm).subscribe({
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
    this.transactionService.changePassword(
      this.passwordForm.currentPassword,
      this.passwordForm.newPassword
    ).subscribe({
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
