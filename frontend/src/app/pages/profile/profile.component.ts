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

    this.profileForm.valueChanges.subscribe(value => {
      console.log(value)
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
    })

    this.f['email']?.disable();
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
    console.log(this.profileForm.value)
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

  resetForm(): void {
    this.profileForm.reset();
  }
}
