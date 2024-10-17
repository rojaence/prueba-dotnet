import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, throwError } from 'rxjs';
import { ICreatedUserDTO, INewUserDTO, IUpdatePasswordDTO, IUpdateUserDTO, IUserDTO, IUserItemDTO } from '../models/user';
import { environment } from '../../environments/environment.development';
import { IActionSuccess } from '../interfaces';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = `${environment.apiUrl}/users`;
  private usersSubject = new BehaviorSubject<IUserItemDTO[]>([]);
  public users$ = this.usersSubject.asObservable();

  constructor(private http: HttpClient) { }

  getUsers(): Observable<IUserItemDTO[]> {
    return this.http.get<IUserItemDTO[]>(this.apiUrl, { withCredentials: true }).pipe(
      map(response => {
        this.usersSubject.next(response);
        return response;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => new Error(error.message)))
    );
  }

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
        this.getUsers().subscribe();
        return response;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => {
        console.error('error al actualizar usuario', error)
        return new Error(error.error)
      }))
    );
  }

  updatePassword(idUser: number, data: IUpdatePasswordDTO): Observable<IActionSuccess> {
    return this.http.post<IActionSuccess>(this.apiUrl + `/${idUser}/password`,  data, { withCredentials: true })
    .pipe(
      map(response => {
        return response;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => {
        console.error('error al actualizar la contraseña', error);
        return new Error(error.error)
      }))
    );
  }

  addNewUser(newUser: INewUserDTO) {
    return this.http.post<ICreatedUserDTO>(this.apiUrl, newUser, { withCredentials: true })
    .pipe(
      map(response => {
        this.getUsers().subscribe();
        return response;
      }),
      catchError((error: HttpErrorResponse) => throwError(() => {
        console.error('error al crear usuario', error);
        return new Error(error.error)
      }))
    );
  }
}
