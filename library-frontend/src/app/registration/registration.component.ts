import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {
  user = {
    name: '',
    username: '',
    password: '',
    tokensAvailable: 3 //default value for tokensAvailable
  };
  errorMessage = '';
  successMessage = '';

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
  }

  register(): void {
    this.authService.register(this.user)
      .subscribe(
        () => {
          this.successMessage = 'User registered successfully';
          this.router.navigate(['/login']);
        },
        error => {
          this.errorMessage = 'Registration failed';
        }
      );
  }
}
