import { Component, inject, LOCALE_ID, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import {MatDatepickerModule} from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { LoginService } from '../../services/login.service';
import { IUserDTO } from '../../models/user';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import {provideNativeDateAdapter} from '@angular/material/core';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [MatFormFieldModule, MatInputModule, MatButtonModule, ReactiveFormsModule, CommonModule, MatDatepickerModule],
  providers: [provideNativeDateAdapter()],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  userData?: IUserDTO;

  constructor(private loginService: LoginService, private fb: FormBuilder, private userService: UserService) {}

  profileForm!: FormGroup;
  passwordForm!: FormGroup;

  locale = inject(LOCALE_ID)

  ngOnInit() {
    this.loginService.getUserData().subscribe({
      next: (value) => {
        this.userData = value;
        this.createForm();
      },
      error: (err) => {
        console.log(err)
      },
    })
    this.createForm();
    this.createPasswordForm();
  }

  createPasswordForm() {
    this.passwordForm = this.fb.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.pattern(/^(?=.*[A-Z])(?=.*\W)(?!.*\s).+$/), Validators.maxLength(100), Validators.minLength(8)]],
      repeatPassword: ['', [Validators.required]]
    })
  }

  createForm() {
    const date = this.userData?.birthDate ? new Date(this.userData.birthDate) : new Date();
    this.profileForm = this.fb.group({
      username: [this.userData?.username, [Validators.required, Validators.minLength(8), Validators.maxLength(20), Validators.pattern(/^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$/)]],
      firstName: [this.userData?.firstName, [Validators.required]],
      middleName: [this.userData?.middleName, [Validators.required]],
      firstLastname: [this.userData?.firstLastname, [Validators.required]],
      secondLastname: [this.userData?.secondLastname, [Validators.required]],
      email: [this.userData?.email],
      idCard: [this.userData?.idCard, [Validators.required, Validators.minLength(10), Validators.maxLength(10), Validators.pattern(/^(?!.*(\d)\1{3})\d{10}$/)]],
      birthDate: [date, [Validators.required]],
      role: [this.userData?.role],
    })

    this.f['email']?.disable();
    this.f['role']?.disable();
  }

  getErrorMessage(controlName: string): string | null {
    let control = this.f[controlName];
    if (control.errors) {
      if (control.hasError('required')) {
        return 'Este campo es obligatorio.';
      }
      if (control.hasError('minlength')) {
        const requiredLength = control.errors['minlength'].requiredLength;
        return `Debe tener al menos ${requiredLength} caracteres.`;
      }
      if (control.hasError('email')) {
        return 'Formato de correo no válido.';
      }
      if (control.hasError('maxlength')) {
        const requiredLength = control.errors['maxlength'].requiredLength;
        return `Debe tener máximo ${requiredLength} caracteres.`;
      }
      if (control.hasError('pattern')) {
        if (controlName == 'username') {
          return 'El nombre de usuario debe contener al menos una letra mayúscula y un número, y no puede contener signos.';
        }
        if (controlName == 'idCard') {
          return 'La identificación debe tener solo números y no puede tener 4 veces seguidas el mismo.';
        }
      }
    }
    return null;
  }

  onSubmit() {
    if (!this.profileForm.valid) return;
    this.userService.updateUserData(this.userData?.idUser!, this.profileForm.value).subscribe({
      next: (res) => {
        if (res.success) {
          alert('usuario actualizado correctamente');
        }
      },
      error: (err) => {
        alert('error al actualizar usuario');
      }
    })
  }

  get f() {
    return this.profileForm.controls;
  }

  get fpass() {
    return this.passwordForm.controls;
  }

  getPassErrorMessage(controlName: string): string | null {
    let control = this.fpass[controlName];
    if (control.hasError('required')) {
      return 'Este campo es obligatorio.';
    }
    if (control.hasError('minlength')) {
      if (control.errors) {
        const requiredLength = control.errors['minlength'].requiredLength;
        return `Debe tener al menos ${requiredLength} caracteres.`;
      }
      return 'Campo inválido';
    }
    if (control.hasError('email')) {
      return 'Formato de correo no válido.';
    }
    if (control.hasError('maxlength')) {
      if (control.errors) {
        const requiredLength = control.errors['maxlength'].requiredLength;
        return `Debe tener máximo ${requiredLength} caracteres.`;
      }
      return 'Campo inválido';
    }
    if (control.hasError('pattern')) {
      if (controlName == 'newPassword') {
        return 'La contraseña debe contener al menos una letra mayúscula, un signo y no debe contener espacios.';
      }
    }
    return null;
  }

  onPassSubmit() {
    if (!this.profileForm.valid) return;
    if (!this.userData) return;
    if (this.fpass['newPassword'].value != this.fpass['repeatPassword'].value) {
      alert('Repetir constraseña no coincide');
      return;
    }
    this.userService.updatePassword(this.userData.idUser, this.passwordForm.value).subscribe({
      next: (res) => {
        if (res.success) {
          alert('Contraseña actualizada correctamente');
          this.resetForm(this.passwordForm);
        }
      },
      error: (err) => {
        console.log(err.errors);
        alert('error al actualizar contraseña');
      }
    })
  }

  resetForm(form: FormGroup): void {
    form.reset();
  }
}
