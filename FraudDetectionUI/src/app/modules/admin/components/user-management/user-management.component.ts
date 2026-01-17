import { Component, OnInit } from '@angular/core';
import { AdminService, User, CreateUserRequest } from '../../../../services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements OnInit {
  users: User[] = [];
  loading = false;
  error = '';
  success = '';
  
  // Pagination
  currentPage = 1;
  pageSize = 20;
  totalCount = 0;
  totalPages = 0;

  // Filters
  roleFilter = '';
  searchQuery = '';

  // Modal states
  showCreateModal = false;
  showEditModal = false;
  showDeleteModal = false;
  showDetailModal = false;
  showAccountModal = false;
  showActionsDropdown: number | null = null;
  
  // Form data
  newUser: CreateUserRequest = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    role: 'User',
    createDefaultAccount: true,
    initialBalance: 1000
  };

  editingUser: User | null = null;
  editForm = {
    firstName: '',
    lastName: '',
    email: '',
    role: '',
    password: ''
  };

  userToDelete: User | null = null;
  userDetail: any = null;
  
  // Account management
  selectedUserForAccount: User | null = null;
  newAccountBalance = 1000;

  constructor(private adminService: AdminService) {}

  ngOnInit() {
    this.loadUsers();
    // Close dropdown when clicking outside
    document.addEventListener('click', () => {
      this.showActionsDropdown = null;
    });
  }

  loadUsers() {
    this.loading = true;
    this.error = '';
    console.log('Loading users...');
    this.adminService.getUsers(this.currentPage, this.pageSize, this.roleFilter || undefined, this.searchQuery || undefined)
      .subscribe({
        next: (response) => {
          console.log('Users loaded:', response);
          this.users = response.data;
          this.totalCount = response.pagination.totalCount;
          this.totalPages = response.pagination.totalPages;
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading users:', err);
          this.error = 'Échec du chargement des utilisateurs: ' + (err.error?.message || err.message || 'Erreur inconnue');
          this.loading = false;
          console.error(err);
        }
      });
  }

  onSearch() {
    this.currentPage = 1;
    this.loadUsers();
  }

  onFilterChange() {
    this.currentPage = 1;
    this.loadUsers();
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadUsers();
    }
  }
  
  toggleActionsDropdown(event: Event, userId: number) {
    event.stopPropagation();
    this.showActionsDropdown = this.showActionsDropdown === userId ? null : userId;
  }

  // Create user
  openCreateModal() {
    this.newUser = {
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      role: 'User',
      createDefaultAccount: true,
      initialBalance: 1000
    };
    this.showCreateModal = true;
  }

  createUser() {
    this.loading = true;
    this.adminService.createUser(this.newUser).subscribe({
      next: (response) => {
        this.success = `✅ Utilisateur "${this.newUser.firstName} ${this.newUser.lastName}" créé avec succès!`;
        this.showCreateModal = false;
        this.loadUsers();
        setTimeout(() => this.success = '', 5000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Échec de la création de l\'utilisateur';
        this.loading = false;
      }
    });
  }

  // Edit user
  openEditModal(user: User) {
    this.editingUser = user;
    this.editForm = {
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      role: user.role,
      password: ''
    };
    this.showEditModal = true;
    this.showActionsDropdown = null;
  }

  updateUser() {
    if (!this.editingUser) return;
    
    const updateData: any = {};
    if (this.editForm.firstName) updateData.firstName = this.editForm.firstName;
    if (this.editForm.lastName) updateData.lastName = this.editForm.lastName;
    if (this.editForm.email) updateData.email = this.editForm.email;
    if (this.editForm.role) updateData.role = this.editForm.role;
    if (this.editForm.password) updateData.password = this.editForm.password;

    this.loading = true;
    this.adminService.updateUser(this.editingUser.id, updateData).subscribe({
      next: () => {
        this.success = `✅ Utilisateur "${this.editingUser?.firstName} ${this.editingUser?.lastName}" mis à jour avec succès!`;
        this.showEditModal = false;
        this.editingUser = null;
        this.loadUsers();
        setTimeout(() => this.success = '', 5000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Échec de la mise à jour';
        this.loading = false;
      }
    });
  }

  // Change user role
  promoteToAdmin(user: User) {
    if (user.role === 'Admin') return;
    this.loading = true;
    this.adminService.updateUser(user.id, { role: 'Admin' }).subscribe({
      next: () => {
        this.success = `✅ ${user.firstName} ${user.lastName} est maintenant Administrateur!`;
        this.loadUsers();
        setTimeout(() => this.success = '', 5000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Échec de la promotion';
        this.loading = false;
      }
    });
    this.showActionsDropdown = null;
  }

  demoteToUser(user: User) {
    if (user.role === 'User') return;
    this.loading = true;
    this.adminService.updateUser(user.id, { role: 'User' }).subscribe({
      next: () => {
        this.success = `✅ ${user.firstName} ${user.lastName} est maintenant Utilisateur standard!`;
        this.loadUsers();
        setTimeout(() => this.success = '', 5000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Échec du changement de rôle';
        this.loading = false;
      }
    });
    this.showActionsDropdown = null;
  }

  // Delete user
  openDeleteModal(user: User) {
    this.userToDelete = user;
    this.showDeleteModal = true;
    this.showActionsDropdown = null;
  }

  confirmDelete() {
    if (!this.userToDelete) return;
    
    this.loading = true;
    this.adminService.deleteUser(this.userToDelete.id).subscribe({
      next: () => {
        this.success = `✅ Utilisateur "${this.userToDelete?.firstName} ${this.userToDelete?.lastName}" supprimé avec succès!`;
        this.showDeleteModal = false;
        this.userToDelete = null;
        this.loadUsers();
        setTimeout(() => this.success = '', 5000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Échec de la suppression';
        this.loading = false;
      }
    });
  }

  // View user details
  viewUserDetail(user: User) {
    this.showActionsDropdown = null;
    this.adminService.getUser(user.id).subscribe({
      next: (detail) => {
        this.userDetail = detail;
        this.showDetailModal = true;
      },
      error: (err) => {
        this.error = 'Échec du chargement des détails';
      }
    });
  }

  // Account management
  openAccountModal(user: User) {
    this.selectedUserForAccount = user;
    this.newAccountBalance = 1000;
    this.showAccountModal = true;
    this.showActionsDropdown = null;
  }

  closeModals() {
    this.showCreateModal = false;
    this.showEditModal = false;
    this.showDeleteModal = false;
    this.showDetailModal = false;
    this.showAccountModal = false;
    this.editingUser = null;
    this.userToDelete = null;
    this.userDetail = null;
    this.selectedUserForAccount = null;
  }

  getRoleBadgeClass(role: string): string {
    return role === 'Admin' ? 'badge-admin' : 'badge-user';
  }

  // Get action buttons based on user role
  getAvailableActions(user: User): string[] {
    const actions = ['view', 'edit'];
    if (user.role === 'User') {
      actions.push('promote');
    } else if (user.role === 'Admin') {
      actions.push('demote');
    }
    actions.push('account', 'delete');
    return actions;
  }
}
