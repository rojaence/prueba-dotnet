import { Component, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import { LoginService } from '../../services/login.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  protected readonly username = signal('');
  protected readonly password = signal('');

  constructor(private loginService: LoginService, private router: Router) {}

  protected onUserNameInput(event: Event) {
    this.username.set((event.target as HTMLInputElement).value);
  }

  protected onPasswordInput(event: Event) {
    this.password.set((event.target as HTMLInputElement).value);
  }

  protected onSubmit() {
    let user = {
      password: this.password(),
      username: this.username()
    }
    console.log(user)
    this.loginService.login(user).subscribe(success => {
      if (success) {
        // this.router.navigate(['']);
        alert('inicio exitoso')
      }
    })
  }
}
