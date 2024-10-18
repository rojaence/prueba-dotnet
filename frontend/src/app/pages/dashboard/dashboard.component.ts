import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { IUserItemDTO } from '../../models/user';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { RoleService } from '../../services/role.service';
import { IRole } from '../../models/role';
import { MatTooltipModule } from '@angular/material/tooltip';
import { EditProfileComponent } from '../../components/components/users/edit-profile/edit-profile.component';
import { NewUserComponent } from '../../components/users/new-user/new-user.component';
import { UserRoles } from '../../constants';
import { LoginService } from '../../services/login.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatTableModule, CommonModule, MatButtonModule, MatIconModule, MatTooltipModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {

  roles: IRole[] = [];

  constructor(private userService: UserService,
    public dialog: MatDialog,
    private roleService: RoleService,
    private loginService: LoginService
  ) {}

  users: IUserItemDTO[] = [];
  displayedColumns: string[] = ['username', 'fullname', 'email', 'status', 'sessionActive', 'lastSession', 'role', 'actions'];

  ngOnInit(): void {
    this.userService.getUsers().subscribe();
    this.userService.users$.subscribe(users => this.users = users)
    this.roleService.getRoles().subscribe({
      next: (roles) => {
        this.roles = roles;
      },
      error: (err) => {
        console.log(err);
      },
    })
  }

  getUserName(idUser: number) {
    let user = this.users.find(u => u.idUser === idUser);
    return `${user?.firstName} ${user?.firstLastname} ${user?.secondLastname}`;
  }

  openNewUserDialog() {
    const dialogRef = this.dialog.open(NewUserComponent, {
      maxWidth: '900px',
      disableClose: true,
    });
    let dialogInstance = dialogRef.componentInstance;
    dialogInstance.roles = this.roles;
  }

  openUpdateUserDialog(idUser: number) {
    let user = this.users.find(u => u.idUser == idUser);
    const dialogRef = this.dialog.open(EditProfileComponent, {
      maxWidth: '900px',
      disableClose: true,
    });
    let dialogInstance = dialogRef.componentInstance;
    dialogInstance.roles = this.roles;
    dialogInstance.userData = user!;
  }

  getPermission(user: IUserItemDTO): boolean {
    let currentUser = this.loginService.currentUser!;
    if (currentUser.idUser === user.idUser) {
      return false;
    };
    if (currentUser.role === UserRoles.Admin && user.roleName !== UserRoles.Admin) {
      return true;
    };
    return false;
  }

  getStatusPermission(user: IUserItemDTO): boolean {
    let currentUser = this.loginService.currentUser!;
    return currentUser.idUser === user.idUser;
  }

  toggleUserStatus(user: IUserItemDTO) {
    this.loginService.toggleUserStatus(user.idUser, !user.status)
    .pipe(
      finalize(() => this.userService.getUsers().subscribe())
    )
    .subscribe({
      next: (res) => {
        alert('estado de usuario actualizado')
      }
    });
  }

  getStatusLabel(status: boolean) {
    return status ? "Deshabilitar usuario" :  "Habilitar usuario" ;
  }
}
