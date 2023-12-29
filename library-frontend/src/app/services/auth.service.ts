import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, EMPTY } from 'rxjs';
import { switchMap, tap, map } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  nameid: string; //
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:44322';
  private tokenSubject: BehaviorSubject<string | null>;

  constructor(private http: HttpClient) {
    this.tokenSubject = new BehaviorSubject<string | null>(localStorage.getItem('token'));
  }

  get token(): string | null {
    return this.tokenSubject.value;
  }

  getUserId(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/api/User/UserId`);
  }

  login(username: string, password: string): Observable<any> {
    const credentials = { username, password };
    return this.http.post<any>(`${this.apiUrl}/api/User/Login`, credentials)
      .pipe(
        map(response => {
          if (response && response.token) {
            localStorage.setItem('token', response.token);
            this.tokenSubject.next(response.token);
          }
        })
      );
  }

  register(user: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/api/User/Register`, user);
  }

  logout(): void {
    localStorage.removeItem('token');
    this.tokenSubject.next(null);
  }

  isLoggedIn(): boolean {
    return this.token !== null;
  }

  getUsernameById(userId: number): Observable<string> {
    return this.http.get<any>(`${this.apiUrl}/api/User/${userId}`)
      .pipe(
        map(response => response.username)
      );
  }

  getUserTokensValue(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/api/User/Tokens`);
  }

  getDecodedToken(): any {
    const token = this.token;
    return token ? jwtDecode(token) : null;
  }

  updateTokensAfterBorrow(): void {
    const currentTokens = this.tokenSubject.value;

    if (currentTokens) {
      // Reduce tokens by 1 after borrowing
      const updatedTokens = String(Number(currentTokens) - 1);
      localStorage.setItem('token', updatedTokens);
      this.tokenSubject.next(updatedTokens);
    }
  }

  updateTokensAfterLend(): void {
    const currentTokens = this.tokenSubject.value;

    if (currentTokens) {
      // Increase tokens by 1 after lending
      const updatedTokens = String(Number(currentTokens) + 1);
      localStorage.setItem('token', updatedTokens);
      this.tokenSubject.next(updatedTokens);
    }
  }
}
