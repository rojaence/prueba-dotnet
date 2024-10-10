import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { IUserLoginDTO, ILoginSuccess } from '../interfaces';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  private apiUrl = `${environment.apiUrl}/session`;
  private accessToken: string | null = '';
  private username = '';
  private lastSession: string = "";

  constructor(private http: HttpClient, private router: Router) { }

  checkUserLoggued(): boolean {
    this.accessToken = this.getToken();
    if (!this.accessToken) return false;
    return true;
  }

  login(user: IUserLoginDTO): Observable<boolean> {
    return this.http.post<ILoginSuccess>(this.apiUrl + '/login', user)
    .pipe(
      map(response => {
        if (!this.getToken()) {
          localStorage.setItem("token", response.token);
          return true;
        }
        this.setLastSession();
        return true;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => {
        console.log('error desde login service', error)
        return new Error(error.error)
      }))
    );
  }

  logout(): Observable<boolean> {
    console.log(this.getUsername())
    return this.http.post<ILoginSuccess>(this.apiUrl + '/logout', { Username: this.getUsername() })
    .pipe(
      map(response => {
        localStorage.removeItem('token');
        this.lastSession = '';
        this.accessToken = null;
        this.username = '';
        this.router.navigate(['/login']);
        return true;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => {
        console.log('error desde login service', error)
        return new Error(error.error)
      }))
    );
  }

  setLastSession() {
    const token = this.getToken();
    console.log(token)
    if (!token) return null;
    const helper = new JwtHelperService();
    const decodedToken = helper.decodeToken(token);
    this.lastSession = decodedToken.date;
    return true;
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUsername(): string | null {
    const token = this.getToken();
    if (!token) return null;
    const helper = new JwtHelperService();
    const decodedToken = helper.decodeToken(token);
    return decodedToken.name;
  }

  getRole(): string | null {
    const token = this.getToken();
    if (!token) return null;

    const helper = new JwtHelperService();
    const decodedToken = helper.decodeToken(token);
    return decodedToken.role || null;
  }

  getLastSession(): string | null {
    const token = this.getToken();
    if (!token) return null;
    const helper = new JwtHelperService();
    const decodedToken = helper.decodeToken(token);
    return decodedToken.date;
  }

  getId(): string | null {
    const token = this.getToken();
    if (!token) return null;
    const helper = new JwtHelperService();
    const decodedToken = helper.decodeToken(token);
    return decodedToken.id;
  }

  isAdmin() {
    return this.getRole() === 'Admin';
  }

  isUser() {
    return this.getRole() === 'User';
  }
}
