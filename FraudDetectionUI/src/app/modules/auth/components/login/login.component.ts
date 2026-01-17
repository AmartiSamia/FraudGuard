import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  loading = false;
  submitted = false;
  error = '';

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  get f() {
    return this.loginForm.controls;
  }

  onSubmit() {
    this.submitted = true;

    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;
    this.authService.login(this.f['email'].value, this.f['password'].value)
      .subscribe({
        next: (response) => {
          const role = response.user.role;
          console.log('Login response:', response);
          console.log('Role from response:', role);
          console.log('Role type:', typeof role);
          console.log('Is Admin?:', role === 'Admin');
          
          if (role === 'Admin') {
            console.log('Navigating to admin dashboard');
            this.router.navigate(['/admin/dashboard']);
          } else {
            console.log('Navigating to user dashboard');
            this.router.navigate(['/user/dashboard']);
          }
        },
        error: (error) => {
          this.error = error.error?.message || 'Login failed';
          this.loading = false;
        }
      });
  }
}
