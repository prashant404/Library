import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  username: string = '';
  tokens: number = 0;

  constructor(private router: Router, public authService: AuthService) { }

  ngOnInit(): void {
    const userId = this.authService.getDecodedToken()?.nameid;

    if (userId) {
      // Fetch both username and tokens
      this.authService.getUsernameById(userId).subscribe(
        (username: string) => {
          this.username = username;
          console.log(username)
        },
        (error) => {
          console.error('Error fetching username:', error);
        }
      );

      this.authService.getUserTokensValue().subscribe(
        (tokens: number) => {
          this.tokens = tokens;
        },
        (error) => {
          console.error('Error fetching tokens:', error);
        }
      );
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  navigateToUserDetails(): void {
    console.log('Navigating to User Details');
    this.router.navigate(['/user-details']);
  }
}
