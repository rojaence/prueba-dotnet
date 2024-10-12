import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { HomeComponent } from './pages/home/home.component';
import { ProfileComponent } from './pages/profile/profile.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    children: []
  },
  {
    path: 'profile',
    component: ProfileComponent,
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
