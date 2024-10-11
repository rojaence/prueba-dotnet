import { Component, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import { LoginService } from '../../services/login.service';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  protected readonly username = signal('');
  protected readonly password = signal('');
  protected readonly logging = signal(false);

  constructor(private loginService: LoginService, private router: Router) {}

  ngOnInit(): void {

  }

  protected onUserNameInput(event: Event) {
    this.username.set((event.target as HTMLInputElement).value);
  }

  protected onPasswordInput(event: Event) {
    this.password.set((event.target as HTMLInputElement).value);
  }

  protected onSubmit(event: SubmitEvent) {
    event.preventDefault();
    let user = {
      password: this.password(),
      username: this.username()
    }
    this.logging.set(true);
    this.loginService.login(user)
    .pipe(
      finalize(() => this.logging.set(false))
    )
    .subscribe({
      next: (success) => {
        if (success) {
          alert('inicio exitoso')
          console.log
          this.router.navigate(['']);
        }
      },
      error: (err) => {
        alert(err)
      },
    })
  }
}
