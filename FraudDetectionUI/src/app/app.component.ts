import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Fraud Detection System';
  isAuthenticated = false;
  userRole: string | null = null;
  userName: string | null = null;
  sidebarCollapsed = false;
  currentDate = new Date();
  currentRoute = '';

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.authService.isAuthenticated$.subscribe(isAuth => {
      this.isAuthenticated = isAuth;
    });
    this.authService.userRole$.subscribe(role => {
      this.userRole = role;
    });
    this.authService.userName$.subscribe(name => {
      this.userName = name;
    });

    // Track current route for page title
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.currentRoute = event.urlAfterRedirects;
    });
  }

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  getPageTitle(): string {
    const titles: { [key: string]: string } = {
      '/admin/dashboard': 'Admin Dashboard',
      '/admin/alerts': 'Fraud Alerts',
      '/admin/suspicious': 'Suspicious Transactions',
      '/admin/statistics': 'Analytics & Statistics',
      '/user/dashboard': 'My Dashboard',
      '/user/transactions': 'My Transactions'
    };
    return titles[this.currentRoute] || 'Dashboard';
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
