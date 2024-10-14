import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { IUserItemDTO } from '../../models/user';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { NewUserComponent } from '../../components/users/new-user/new-user.component';
import { RoleService } from '../../services/role.service';
import { IRole } from '../../models/role';


@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatTableModule, CommonModule, MatButtonModule, MatIconModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {

  roles: IRole[] = [];

  constructor(private userService: UserService,
    public dialog: MatDialog,
    private roleService: RoleService
  ) {}

  users: IUserItemDTO[] = [];
  displayedColumns: string[] = ['username', 'fullname', 'email', 'status', 'sessionActive', 'lastSession', 'role'];

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
}
