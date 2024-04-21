import { Component } from '@angular/core';
import { AuthService } from '../_shared/services/auth.service';
import { StorageService } from '../_shared/services/storage.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  form: any = {
    username: null,
    password: null
  };
  isLoggedIn = false;
  isLoginFailed = false;
  errorMessage = '';
  roles: string[] = [];

  constructor(
    private authService: AuthService, 
    private storageService: StorageService,
    private router: Router
  ) { }

  ngOnInit(): void {
    if (this.storageService.isLoggedIn()) {
      this.isLoggedIn = true;
      this.roles = this.storageService.getUser().name;
    }
  }

  onSubmit(): void {
    const { username, password } = this.form;

    const request: Record<string, unknown> = {
      email: username,
      password,
      isRemberMe: true
    };

    this.authService.login(request).subscribe({
      next: data => {
        this.storageService.saveUser(data);

        this.isLoginFailed = false;
        this.isLoggedIn = true;
        this.roles = this.storageService.getUser().name;
        this.navigateToUser();
      },
      error: err => {
        this.errorMessage = err.error.message;
        this.isLoginFailed = true;
      }
    });
  }

  navigateToUser(): void {
    void this.router.navigate(['users']);
  }
}
