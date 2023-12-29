import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  username = '';
  password = '';
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
  }

  login(): void {
    this.authService.login(this.username, this.password)
      .subscribe(
        () => {
          this.router.navigate(['/dashboard']);
        },
        error => {
          this.errorMessage = 'Invalid username or password';
        }
      );
  }

  registerRedirect(): void {
    this.router.navigate(['/registration']);
  }
}
