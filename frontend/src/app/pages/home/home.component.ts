import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginService } from '../../services/login.service';
import { IUserDTO } from '../../models/user';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  title = 'PruebaDotnet';
  username: string | null = '';
  lastSession: string = '';
  role: string = '';

  userData?: IUserDTO;

  constructor(private loginService: LoginService, private userService: UserService) {}

  ngOnInit(): void {
    this.username = this.loginService.getUsername();
    this.lastSession = this.loginService.getLastSession()!;
    this.role = this.loginService.getRole()!;

    this.userService.getUserData(this.loginService.getId()!).subscribe({
      next: (value) => {
        this.userData = value;
      },
      error: (err) => {
        console.log(err)
      },
    })
  }
}
