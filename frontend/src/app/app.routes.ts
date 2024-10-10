import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { authGuard } from './guards/auth.guard';
import { HomeComponent } from './pages/home/home.component';
import { loginGuard } from './login.guard';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    canActivate: [loginGuard],
    children: []
  },
  /* {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard],
    data: {
      expectedRole: 'Admin'
    }
  } */
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: '**',
    component: NotFoundComponent
  }
];
