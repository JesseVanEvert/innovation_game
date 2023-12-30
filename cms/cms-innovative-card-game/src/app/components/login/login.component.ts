import { Component } from '@angular/core';
import { Token } from 'src/app/interfaces/token';
import { AuthenticationService } from '../../services/authentication.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  protected email: string;
  protected password: string;

  constructor(
    private authService: AuthenticationService,
    private router: Router
  ) {}

  onSubmit() {
    let token: Token;
    this.authService.login(this.email, this.password).subscribe({
      next: (t) => (token = t),
      error: (err) => console.log(err),
      complete: () => {
        localStorage.setItem('token', token.accessToken);
        localStorage.setItem(
          'expires_at',
          JSON.stringify(token.expiresIn.valueOf())
        );
        this.authService.isLoggedIn = true;
        this.routeUserToCMS();
      },
    });
  }

  routeUserToCMS() {
    this.router.navigateByUrl('/admin-cms');
  }
}
