import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { LoginService } from '../../services/login.service';
import { Router, RouterModule } from '@angular/router';
import { IUserDTO } from '../../models/user';
import { UserRoles } from '../../constants';

@Component({
  selector: 'app-toolbar',
  standalone: true,
  imports: [MatToolbarModule, CommonModule, MatButtonModule, RouterModule],
  templateUrl: './app-toolbar.component.html',
  styleUrl: './app-toolbar.component.scss'
})
export class AppToolbarComponent implements OnInit {
  title = 'Pruebadotnet';
  userData: IUserDTO | null = null;

  UserRoles = UserRoles;

  constructor(private loginService: LoginService, private router: Router) {}

  logout() {
    this.loginService.logout().subscribe({
      next: (success) => {
        if (success) this.router.navigate(['/login']);
      },
    });
  }

  ngOnInit(): void {
    this.loginService.userData$.subscribe({
      next: (data) => {
        this.userData = data;
      },
      error: (err) => console.log("Error al obtener el estado de userData", err)
    })
  }


}
