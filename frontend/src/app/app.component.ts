import { Component, OnInit, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginService } from './services/login.service';
import { AppToolbarComponent } from './components/app-toolbar/app-toolbar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, AppToolbarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  loginPage = signal(false)

  ngOnInit(): void {
   /* this.loginService.isAuthenticated().subscribe({
    next: (res) => {
      if (res.isAuthenticated) this.navigateToHome();
      else this.navigateToLogin();
    },
    error: (err) => {
      console.warn(err)
    },
   }) */


    this.router.events.subscribe(() => {
      const currentRoute = this.router.url;
      if (currentRoute === '/login') {
        this.loginPage.set(true)
      } else {
        this.loginPage.set(false)
      }
    });
  }

  constructor(private router: Router, private loginService: LoginService) {}

  navigateToLogin() {
    this.router.navigate(['login']);
  }

  navigateToHome() {
    this.router.navigate(['']);
  }
}
