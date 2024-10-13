import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { IUserItemDTO } from '../../models/user';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { NewUserComponent } from '../../components/users/new-user/new-user.component';


@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatTableModule, CommonModule, MatButtonModule, MatIconModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {

  constructor(private userService: UserService,
    public dialog: MatDialog
  ) {}

  users: IUserItemDTO[] = [];
  displayedColumns: string[] = ['username', 'fullname', 'status', 'sessionActive', 'lastSession', 'role'];

  ngOnInit(): void {
    this.userService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
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
  }
}
