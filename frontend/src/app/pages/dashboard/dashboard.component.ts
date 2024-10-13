import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { IUserItemDTO } from '../../models/user';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatTableModule, CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  constructor(private userService: UserService) {}

  users: IUserItemDTO[] = [];
  displayedColumns: string[] = ['username', 'fullname', 'status', 'sessionActive', 'lastSession', 'role'];

  ngOnInit(): void {
    this.userService.getUsers().subscribe({
      next: (users) => {
        console.log(users)
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
}
