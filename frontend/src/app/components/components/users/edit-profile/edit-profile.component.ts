import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import {MatDatepickerModule} from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { provideNativeDateAdapter } from '@angular/material/core';
import { UserService } from '../../../../services/user.service';
import { IRole } from '../../../../models/role';
import { MatSelectModule } from '@angular/material/select';
import { IUserItemDTO } from '../../../../models/user';


@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [MatButtonModule, MatDialogModule, MatFormFieldModule, ReactiveFormsModule, MatSelectModule, CommonModule, MatInputModule, MatDatepickerModule],
  templateUrl: './edit-profile.component.html',
  styleUrl: './edit-profile.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [provideNativeDateAdapter()]
})
export class EditProfileComponent implements OnInit {
  @Output() onUpdate = new EventEmitter<boolean>();
  @Input() roles: IRole[] = [];
  @Input() userData!: IUserItemDTO;

  constructor(private newUserDialogRef: MatDialogRef<EditProfileComponent>, private fb: FormBuilder, private userService: UserService){}

  profileForm!: FormGroup;

  closeDialog() {
    this.newUserDialogRef.close();
  }

  ngOnInit() {
    this.createForm();
  }

  createForm() {
    let role = this.roles.find(r => r.roleName === this.userData.roleName) ?? this.roles[1];
    this.profileForm = this.fb.group({
      username: [this.userData?.username, [Validators.required, Validators.minLength(8), Validators.maxLength(20), Validators.pattern(/^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$/)]],
      firstName: [this.userData?.firstName, [Validators.required]],
      middleName: [this.userData?.middleName, [Validators.required]],
      firstLastname: [this.userData?.firstLastname, [Validators.required]],
      secondLastname: [this.userData?.secondLastname, [Validators.required]],
      idCard: [this.userData?.idCard, [Validators.required, Validators.minLength(10), Validators.maxLength(10), Validators.pattern(/^(?!.*(\d)\1{3})\d{10}$/)]],
      role: [role.idRole, Validators.required],
      birthDate: [new Date(this.userData.birthDate), [Validators.required]],
    })
  }

  getErrorMessage(controlName: string): string | null {
    let control = this.profileForm.get(controlName);
    console.log(control)
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
    }
    return null;
  }

  get f() {
    return this.profileForm.controls;
  }

  onSubmit() {
    this.profileForm.markAllAsTouched();
    this.profileForm.updateValueAndValidity();
    console.log('no paso la validación')
    if (!this.profileForm.valid) return;
    console.log('paso la validación')
    this.userService.updateUserData(this.userData?.idUser, this.profileForm.value).subscribe({
      next: (response) => {
        if (response.success) {
          alert('Usuario actualizado correctamente')
          this.onUpdate.emit(true);
        }
        this.closeDialog();
      },
      error: (err) => {
        console.log(err)
      },
    })
  }
}

