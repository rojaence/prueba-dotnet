import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { LoginService } from '../../services/login.service';

@Component({
  selector: 'app-toolbar',
  standalone: true,
  imports: [MatToolbarModule, CommonModule, MatButtonModule],
  templateUrl: './app-toolbar.component.html',
  styleUrl: './app-toolbar.component.scss'
})
export class AppToolbarComponent {
  title = 'Pruebadotnet';

  constructor(private loginService: LoginService) {}

  logout() {
    this.loginService.logout().subscribe();
  }
}
