import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { HomeComponent } from './pages/home/home.component';
import { ProfileComponent } from './pages/profile/profile.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { UnauthorizedComponent } from './pages/unauthorized/unauthorized.component';
import { authGuard } from './guards/auth.guard';
import { UserRoles } from './constants';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    children: [],
    data: {
      expectedRoles: [UserRoles.Admin, UserRoles.User]
    }
  },
  {
    path: 'profile',
    component: ProfileComponent,
    data: {
      expectedRoles: [UserRoles.Admin, UserRoles.User]
    }
  },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard],
    data: {
      expectedRoles: [UserRoles.Admin]
    }
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'unauthorized',
    component: UnauthorizedComponent,
  },
  {
    path: '**',
    component: NotFoundComponent
  }
];
