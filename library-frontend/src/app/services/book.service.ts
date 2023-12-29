
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, EMPTY } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private apiUrl = 'https://localhost:44322';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getAllBooks(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/api/Book/AllBooksAddedByOtherUsers`);
  }

  borrowBook(bookId: number): Observable<any> {
    return this.authService.getUserTokensValue().pipe(
      switchMap((tokens: number) => {
        if (tokens >= 1) {
          return this.http.post(`${this.apiUrl}/api/Book/Borrow/${bookId}`, null, { responseType: 'text' })
            .pipe(
              tap(responseText => {
                // Log the response text for debugging
                console.log(responseText);

                // Update tokens in AuthService after borrowing
                this.authService.updateTokensAfterBorrow();
              })
            );
        } else {
          console.error('Insufficient tokens for borrowing');
          return EMPTY;
        }
      })
    );
  }

  postBook(book: any): Observable<any> {
    book.IsBookAvailable = true;

    return this.http.post<any>(`${this.apiUrl}/api/Book`, book);
  }

  searchBooks(query: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/api/Book/Search?query=${query}`);
  }
}
