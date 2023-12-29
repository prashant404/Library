
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { UserService } from '../services/user.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.css']
})
export class UserDetailsComponent implements OnInit {
  borrowedBooks: any[] = [];
  lentBooks: any[] = [];
  userId: number | undefined;
  username: string | undefined;

  constructor(private authService: AuthService, private userService: UserService, private router: Router) { }


  ngOnInit(): void {
    // Fetch user ID
    this.authService.getUserId().subscribe({
      next: (userId: number) => {
        if (userId) {
          this.userId = userId;

          // Fetch borrowed books
          this.userService.getBooksBorrowedByUser(userId).subscribe(books => {
            this.borrowedBooks = books;
          });

          // Fetch lent books
          this.userService.getBooksLentByUser(userId).subscribe(books => {
            this.lentBooks = books;
          });

          // Fetch and display username
          this.authService.getUsernameById(userId).subscribe(username => {
            this.username = username;
          });
        }
      },
      error: (error: any) => {
        console.error('Error fetching user ID:', error);
      }
    });
  }
  goBackToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }
}
