import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { catchError, map, Observable, throwError } from 'rxjs';
import { IRole } from '../models/role';

@Injectable({
  providedIn: 'root'
})
export class RoleService {

  constructor(private http: HttpClient) { }

  private apiUrl = `${environment.apiUrl}/roles`;

  getRoles(): Observable<IRole[]> {
    return this.http.get<IRole[]>(this.apiUrl, { withCredentials: true }).pipe(
      map(response => {
        return response;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => new Error(error.message)))
    );
  }
}
