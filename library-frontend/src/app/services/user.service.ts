
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'https://localhost:44322';

  constructor(private http: HttpClient) { }

  getBooksLentByUser(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/api/User/LentBooks/${userId}`);
  }

  getBooksBorrowedByUser(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/api/User/BorrowedBooks/${userId}`);

  }
}
