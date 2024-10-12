import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, throwError } from 'rxjs';
import { IUpdateUserDTO, IUserDTO } from '../models/user';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) { }

  getUserData(idUser: string): Observable<IUserDTO> {
    return this.http.get<IUserDTO>(this.apiUrl + `/${idUser}`, { withCredentials: true }).pipe(
      map(response => {
        return response;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => new Error(error.message)))
    );
  }

  updateUserData(idUser: number, data: IUpdateUserDTO): Observable<{ success: boolean }> {
    return this.http.put<{ success: boolean }>(this.apiUrl + `/${idUser}`, { ...data }, {withCredentials: true})
    .pipe(
      map(response => {
        return response;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => {
        console.error('error al actualizar usuario', error)
        console.log(error.error.errors)
        return new Error(error.error)
      }))
    );
  }
}
