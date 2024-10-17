import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import {MatDialog, MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import {MatDatepickerModule} from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { provideNativeDateAdapter } from '@angular/material/core';
import { UserService } from '../../../services/user.service';
import { ICreatedUserDTO, IUserDTO } from '../../../models/user';
import { IRole } from '../../../models/role';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-new-user',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, CommonModule, MatInputModule, MatFormFieldModule, MatDatepickerModule, ReactiveFormsModule, MatSelectModule],
  templateUrl: './new-user.component.html',
  styleUrl: './new-user.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [provideNativeDateAdapter()],
})
export class NewUserComponent implements OnInit {
  @Output() onCreate = new EventEmitter<ICreatedUserDTO>();
  @Input() roles: IRole[] = [];

  constructor(private newUserDialogRef: MatDialogRef<NewUserComponent>, private fb: FormBuilder, private userService: UserService){}

  profileForm!: FormGroup;

  closeDialog() {
    this.newUserDialogRef.close();
  }

  ngOnInit() {
    this.createForm();
  }
  createForm() {
    this.profileForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(20), Validators.pattern(/^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$/)]],
      firstName: ['', [Validators.required]],
      middleName: ['', [Validators.required]],
      firstLastname: ['', [Validators.required]],
      secondLastname: ['', [Validators.required]],
      idCard: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(10), Validators.pattern(/^(?!.*(\d)\1{3})\d{10}$/)]],
      role: [this.roles[1].idRole, Validators.required],
      birthDate: [new Date(), [Validators.required]],
      password: ['', [Validators.required, Validators.pattern(/^(?=.*[A-Z])(?=.*\W)(?!.*\s).+$/), Validators.maxLength(100), Validators.minLength(8)]],
    })
  }

  getErrorMessage(controlName: string): string | null {
    let control = this.profileForm.get(controlName);
    if (!control) return '';
    if (control.errors) {
      if (control.hasError('required')) {
        return 'Este campo es obligatorio.';
      }
      if (control.hasError('minlength')) {
        const requiredLength = control.errors['minlength'].requiredLength;
        return `Debe tener al menos ${requiredLength} caracteres.`;
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
      if (control.hasError('pattern')) {
        if (controlName == 'password') {
          console.log('pas')
          return 'La contraseña debe contener al menos una letra mayúscula, un signo y no debe contener espacios.';
        }
      }
    }
    return null;
  }

  get f() {
    return this.profileForm.controls;
  }

  onSubmit() {
    console.log(this.profileForm.value)
    this.profileForm.markAllAsTouched();
    this.profileForm.updateValueAndValidity();
    if (!this.profileForm.valid) return;
    this.userService.addNewUser(this.profileForm.value).subscribe({
      next: (user) => {
        this.onCreate.emit(user);
        this.closeDialog();
      },
      error: (err) => {

      },
    })
  }
}
