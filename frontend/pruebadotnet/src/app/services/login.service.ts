import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { IUserLoginDTO, ILoginSuccess } from '../../interfaces';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  private apiUrl = `${environment.apiUrl}/session`;
  private accessToken: string | null = '';
  private username = '';

  constructor(private http: HttpClient) { }

  checkUserLoggued(): boolean {
    this.accessToken = localStorage.getItem('token');
    if (!this.accessToken) return false;
    return true;
  }

  login(user: IUserLoginDTO): Observable<boolean> {
    return this.http.post<ILoginSuccess>(this.apiUrl + '/login', user)
    .pipe(
      map(response => {
        localStorage.setItem("token", response.token);
        if (!response.token) return false
        return true;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => new Error(error.message)))
    );
  }
}
