import { Component, OnInit } from '@angular/core';
import { TransactionService, Transaction, Account, CreateTransactionRequest, PaginatedResponse } from '../../../../services/transaction.service';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.scss']
})
export class TransactionsComponent implements OnInit {
  transactions: Transaction[] = [];
  accounts: Account[] = [];
  loading = true;
  error = '';
  filterType = 'all';
  
  // Pagination
  currentPage = 1;
  pageSize = 20;
  totalCount = 0;
  totalPages = 0;
  
  // Global counts (not affected by filters)
  allTransactionsCount = 0;
  suspiciousCount = 0;
  
  // New transaction form
  showNewTransactionModal = false;
  newTransaction: CreateTransactionRequest = {
    accountId: 0,
    amount: 0,
    type: 'Virement',
    country: 'Morocco',
    device: 'Web'
  };
  recipientRIB = '';
  transactionDescription = '';
  submitting = false;
  submitError = '';
  submitSuccess = '';

  // Transaction detail modal
  showDetailModal = false;
  selectedTransaction: Transaction | null = null;

  // Comprehensive banking transaction types
  transactionTypes = [
    { value: 'Virement', label: 'Virement Bancaire', icon: '↔️', description: 'Transfert vers un autre compte' },
    { value: 'Retrait', label: 'Retrait', icon: '💵', description: 'Retrait d\'espèces' },
    { value: 'Depot', label: 'Dépôt', icon: '📥', description: 'Dépôt d\'espèces ou chèque' },
    { value: 'Prelevement', label: 'Prélèvement', icon: '📤', description: 'Prélèvement automatique' },
    { value: 'VirementInternational', label: 'Virement International', icon: '🌍', description: 'Transfert vers l\'étranger' },
    { value: 'Paiement', label: 'Paiement Carte', icon: '💳', description: 'Achat par carte bancaire' },
    { value: 'PaiementEnLigne', label: 'Paiement en Ligne', icon: '🛒', description: 'Achat sur internet' },
    { value: 'VirementInstantane', label: 'Virement Instantané', icon: '⚡', description: 'Transfert immédiat' },
    { value: 'Emprunt', label: 'Emprunt', icon: '🏦', description: 'Demande de crédit' },
    { value: 'RemboursementCredit', label: 'Remboursement Crédit', icon: '📋', description: 'Échéance de prêt' },
    { value: 'Facture', label: 'Paiement Facture', icon: '📄', description: 'Règlement de facture' },
    { value: 'Salaire', label: 'Virement Salaire', icon: '💰', description: 'Réception de salaire' }
  ];

  countries = [
    { code: 'Morocco', name: 'Maroc 🇲🇦' },
    { code: 'France', name: 'France 🇫🇷' },
    { code: 'USA', name: 'États-Unis 🇺🇸' },
    { code: 'UK', name: 'Royaume-Uni 🇬🇧' },
    { code: 'Germany', name: 'Allemagne 🇩🇪' },
    { code: 'Spain', name: 'Espagne 🇪🇸' },
    { code: 'Italy', name: 'Italie 🇮🇹' },
    { code: 'Belgium', name: 'Belgique 🇧🇪' },
    { code: 'Netherlands', name: 'Pays-Bas 🇳🇱' },
    { code: 'Canada', name: 'Canada 🇨🇦' },
    { code: 'UAE', name: 'Émirats Arabes Unis 🇦🇪' },
    { code: 'SaudiArabia', name: 'Arabie Saoudite 🇸🇦' },
    { code: 'China', name: 'Chine 🇨🇳' },
    { code: 'Japan', name: 'Japon 🇯🇵' }
  ];

  devices = [
    { value: 'Web', label: 'Application Web', icon: '🖥️' },
    { value: 'Mobile', label: 'Application Mobile', icon: '📱' },
    { value: 'ATM', label: 'Distributeur ATM', icon: '🏧' },
    { value: 'Agency', label: 'Agence Bancaire', icon: '🏦' },
    { value: 'POS', label: 'Terminal de Paiement', icon: '💳' }
  ];

  constructor(private transactionService: TransactionService) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    this.error = '';
    
    // Load accounts first
    this.transactionService.getMyAccounts().subscribe({
      next: (accounts) => {
        this.accounts = accounts;
        if (accounts.length > 0) {
          this.newTransaction.accountId = accounts[0].id;
        }
        this.loadTransactions();
      },
      error: (err) => {
        this.error = 'Failed to load accounts';
        this.loading = false;
        console.error(err);
      }
    });
  }

  loadTransactions() {
    const isFraud = this.filterType === 'suspicious' ? true : undefined;
    
    this.transactionService.getMyTransactions(this.currentPage, this.pageSize, isFraud).subscribe({
      next: (response: PaginatedResponse<Transaction>) => {
        this.transactions = response.data;
        this.totalCount = response.pagination.totalCount;
        this.totalPages = response.pagination.totalPages;
        // Use the counts from API
        if (response.counts) {
          this.allTransactionsCount = response.counts.all;
          this.suspiciousCount = response.counts.suspicious;
        }
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load transactions';
        this.loading = false;
        console.error(err);
      }
    });
  }

  onFilterChange(type: string) {
    this.filterType = type;
    this.currentPage = 1;
    this.loadTransactions();
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadTransactions();
    }
  }

  refresh() {
    this.currentPage = 1;
    this.loadData();
  }

  openNewTransactionModal() {
    this.showNewTransactionModal = true;
    this.submitError = '';
    this.submitSuccess = '';
    this.recipientRIB = '';
    this.transactionDescription = '';
  }

  closeNewTransactionModal() {
    this.showNewTransactionModal = false;
    this.submitError = '';
    this.submitSuccess = '';
    this.resetForm();
  }

  resetForm() {
    this.newTransaction = {
      accountId: this.accounts.length > 0 ? this.accounts[0].id : 0,
      amount: 0,
      type: 'Virement',
      country: 'Morocco',
      device: 'Web'
    };
    this.recipientRIB = '';
    this.transactionDescription = '';
  }

  requiresRecipient(): boolean {
    return ['Virement', 'VirementInternational', 'VirementInstantane'].includes(this.newTransaction.type);
  }

  getSelectedTypeInfo() {
    return this.transactionTypes.find(t => t.value === this.newTransaction.type);
  }

  submitTransaction() {
    if (this.newTransaction.amount <= 0) {
      this.submitError = 'Veuillez entrer un montant valide';
      return;
    }
    if (this.newTransaction.accountId === 0) {
      this.submitError = 'Veuillez sélectionner un compte';
      return;
    }
    if (this.requiresRecipient() && !this.recipientRIB) {
      this.submitError = 'Veuillez entrer le RIB du bénéficiaire';
      return;
    }

    // Check if account has sufficient balance for outgoing transactions
    const account = this.accounts.find(a => a.id === this.newTransaction.accountId);
    const outgoingTypes = ['Virement', 'Retrait', 'Prelevement', 'VirementInternational', 'Paiement', 'PaiementEnLigne', 'VirementInstantane', 'RemboursementCredit', 'Facture'];
    if (account && outgoingTypes.includes(this.newTransaction.type) && this.newTransaction.amount > account.balance) {
      this.submitError = `Solde insuffisant. Disponible: ${account.balance.toFixed(2)} MAD`;
      return;
    }

    this.submitting = true;
    this.submitError = '';
    this.submitSuccess = '';

    // Build request with optional fields
    const request: CreateTransactionRequest = {
      ...this.newTransaction
    };
    if (this.recipientRIB) {
      request.recipientRIB = this.recipientRIB;
    }
    if (this.transactionDescription) {
      request.description = this.transactionDescription;
    }

    this.transactionService.createTransaction(request).subscribe({
      next: (transaction) => {
        this.submitting = false;
        if (transaction.isFraud) {
          this.submitSuccess = `⚠️ Transaction créée mais signalée comme suspecte: ${transaction.fraudReason}`;
        } else {
          this.submitSuccess = '✅ Transaction effectuée avec succès!';
        }
        this.loadData(); // Reload to update balances
        setTimeout(() => this.closeNewTransactionModal(), 2500);
      },
      error: (err) => {
        this.submitting = false;
        this.submitError = err.error?.message || 'Échec de la transaction';
        console.error(err);
      }
    });
  }

  getAccountDisplay(accountId: number): string {
    const account = this.accounts.find(a => a.id === accountId);
    return account ? `${account.accountNumber} (${account.balance.toFixed(2)} MAD)` : 'Inconnu';
  }

  getTypeLabel(typeValue: string): string {
    const type = this.transactionTypes.find(t => t.value === typeValue);
    return type ? type.label : typeValue;
  }

  getTypeIcon(typeValue: string): string {
    const type = this.transactionTypes.find(t => t.value === typeValue);
    return type ? type.icon : '💳';
  }

  isOutgoingTransaction(type: string): boolean {
    return ['Virement', 'Retrait', 'Prelevement', 'VirementInternational', 'Paiement', 'PaiementEnLigne', 'VirementInstantane', 'RemboursementCredit', 'Facture'].includes(type);
  }

  isIncomingTransaction(type: string): boolean {
    return ['Depot', 'Salaire', 'Emprunt'].includes(type);
  }

  // Transaction detail modal
  openDetailModal(transaction: Transaction) {
    this.selectedTransaction = transaction;
    this.showDetailModal = true;
  }

  closeDetailModal() {
    this.showDetailModal = false;
    this.selectedTransaction = null;
  }

  getCountryName(code: string): string {
    const country = this.countries.find(c => c.code === code);
    return country ? country.name : code;
  }

  getDeviceLabel(value: string): string {
    const device = this.devices.find(d => d.value === value);
    return device ? `${device.icon} ${device.label}` : value;
  }

  getDeviceIcon(value: string): string {
    const device = this.devices.find(d => d.value === value);
    return device ? device.icon : '💻';
  }
}
