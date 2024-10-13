import { Injectable, OnInit } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { IUserLoginDTO, ILoginSuccess, IAuthenticated } from '../interfaces';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, catchError, map, Observable, throwError } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Router } from '@angular/router';
import { IUserDTO } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class LoginService implements OnInit {

  private apiUrl = `${environment.apiUrl}/session`;

  private userDataSubject = new BehaviorSubject<IUserDTO | null>(null);
  userData$ = this.userDataSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit() {
    this.getUserData().subscribe();
  }

  isAuthenticated(): Observable<IAuthenticated> {
    return this.http.get<IAuthenticated>(this.apiUrl + '/check-auth', { withCredentials: true })
    .pipe(
      map(response => {
        return response;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => {
        console.log('error desde login service', error)
        return new Error(error.error)
      }))
    )
  }

  login(user: IUserLoginDTO): Observable<boolean> {
    return this.http.post<ILoginSuccess>(this.apiUrl + '/login', user)
    .pipe(
      map(response => {
        console.log('respuesta desde login', response)
        return response.success;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => {
        console.log('error desde login service', error)
        return new Error(error.error)
      }))
    );
  }

  logout(): Observable<boolean> {
    return this.http.post<ILoginSuccess>(this.apiUrl + '/logout', { }, {withCredentials: true})
    .pipe(
      map(response => {
        return response.success;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => {
        console.log('error desde login service', error)
        return new Error(error.error)
      }))
    );
  }

  getUserData(): Observable<IUserDTO> {
    return this.http.get<IUserDTO>(this.apiUrl + '/user-data', { withCredentials: true })
    .pipe(
      map(response => {
        this.userDataSubject.next(response);
        return response;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => {
        console.log('error desde login service', error)
        return new Error(error.error)
      }))
    );
  }

  get currentUser(): IUserDTO | null {
    return this.userDataSubject.getValue();
  }
}
