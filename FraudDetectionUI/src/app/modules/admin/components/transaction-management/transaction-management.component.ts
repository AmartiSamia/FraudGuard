import { Component, OnInit } from '@angular/core';
import { AdminService, AdminTransaction, TransactionDetail } from '../../../../services/admin.service';

@Component({
  selector: 'app-transaction-management',
  templateUrl: './transaction-management.component.html',
  styleUrls: ['./transaction-management.component.scss']
})
export class TransactionManagementComponent implements OnInit {
  transactions: AdminTransaction[] = [];
  loading = false;
  error = '';
  success = '';

  // Pagination
  currentPage = 1;
  pageSize = 50;
  totalCount = 0;
  totalPages = 0;

  // Filters
  filters = {
    isFraud: undefined as boolean | undefined,
    country: '',
    type: '',
    minAmount: undefined as number | undefined,
    maxAmount: undefined as number | undefined,
    startDate: '',
    endDate: ''
  };

  // Available options
  countries: string[] = [];
  types = ['Purchase', 'Transfer', 'Withdrawal', 'Deposit'];

  // Modal states
  showDetailModal = false;
  showEditModal = false;
  showDeleteModal = false;

  // Current items
  transactionDetail: TransactionDetail | null = null;
  editingTransaction: AdminTransaction | null = null;
  transactionToDelete: AdminTransaction | null = null;

  // Edit form
  editForm = {
    isFraud: false,
    fraudReason: ''
  };

  constructor(private adminService: AdminService) {}

  ngOnInit() {
    this.loadTransactions();
    this.loadCountries();
  }

  loadTransactions() {
    this.loading = true;
    this.error = '';

    const filterParams = {
      isFraud: this.filters.isFraud,
      country: this.filters.country || undefined,
      type: this.filters.type || undefined,
      minAmount: this.filters.minAmount,
      maxAmount: this.filters.maxAmount,
      startDate: this.filters.startDate || undefined,
      endDate: this.filters.endDate || undefined
    };

    this.adminService.getTransactions(this.currentPage, this.pageSize, filterParams)
      .subscribe({
        next: (response) => {
          this.transactions = response.data;
          this.totalCount = response.pagination.totalCount;
          this.totalPages = response.pagination.totalPages;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load transactions';
          this.loading = false;
          console.error(err);
        }
      });
  }

  loadCountries() {
    // Get unique countries from transactions
    this.adminService.getTransactions(1, 1000).subscribe({
      next: (response) => {
        const countrySet = new Set(response.data.map(t => t.country));
        this.countries = Array.from(countrySet).sort();
      }
    });
  }

  applyFilters() {
    this.currentPage = 1;
    this.loadTransactions();
  }

  clearFilters() {
    this.filters = {
      isFraud: undefined,
      country: '',
      type: '',
      minAmount: undefined,
      maxAmount: undefined,
      startDate: '',
      endDate: ''
    };
    this.currentPage = 1;
    this.loadTransactions();
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadTransactions();
    }
  }

  // View transaction detail
  viewDetail(transaction: AdminTransaction) {
    this.adminService.getTransaction(transaction.id).subscribe({
      next: (detail) => {
        this.transactionDetail = detail;
        this.showDetailModal = true;
      },
      error: () => {
        this.error = 'Failed to load transaction details';
      }
    });
  }

  // Edit transaction
  openEditModal(transaction: AdminTransaction) {
    this.editingTransaction = transaction;
    this.editForm = {
      isFraud: transaction.isFraud,
      fraudReason: transaction.fraudReason || ''
    };
    this.showEditModal = true;
  }

  updateTransaction() {
    if (!this.editingTransaction) return;

    this.loading = true;
    this.adminService.updateTransaction(this.editingTransaction.id, this.editForm).subscribe({
      next: () => {
        this.success = 'Transaction updated successfully!';
        this.showEditModal = false;
        this.editingTransaction = null;
        this.loadTransactions();
        setTimeout(() => this.success = '', 3000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to update transaction';
        this.loading = false;
      }
    });
  }

  // Delete transaction
  openDeleteModal(transaction: AdminTransaction) {
    this.transactionToDelete = transaction;
    this.showDeleteModal = true;
  }

  confirmDelete() {
    if (!this.transactionToDelete) return;

    this.loading = true;
    this.adminService.deleteTransaction(this.transactionToDelete.id).subscribe({
      next: () => {
        this.success = 'Transaction deleted successfully!';
        this.showDeleteModal = false;
        this.transactionToDelete = null;
        this.loadTransactions();
        setTimeout(() => this.success = '', 3000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to delete transaction';
        this.loading = false;
      }
    });
  }

  closeModals() {
    this.showDetailModal = false;
    this.showEditModal = false;
    this.showDeleteModal = false;
    this.transactionDetail = null;
    this.editingTransaction = null;
    this.transactionToDelete = null;
  }

  getTypeIcon(type: string): string {
    const icons: { [key: string]: string } = {
      'Purchase': 'fa-shopping-cart',
      'Transfer': 'fa-exchange-alt',
      'Withdrawal': 'fa-minus-circle',
      'Deposit': 'fa-plus-circle'
    };
    return icons[type] || 'fa-receipt';
  }

  getTypeClass(type: string): string {
    return `type-${type.toLowerCase()}`;
  }

  getRiskLevel(reason: string | undefined): string {
    if (!reason) return 'low';
    const lowerReason = reason.toLowerCase();
    if (lowerReason.includes('high') || lowerReason.includes('suspicious')) return 'high';
    if (lowerReason.includes('medium') || lowerReason.includes('unusual')) return 'medium';
    return 'low';
  }
}
