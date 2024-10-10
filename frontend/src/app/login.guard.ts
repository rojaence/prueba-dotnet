import { CanActivateFn, Router } from '@angular/router';
import { LoginService } from './services/login.service';
import { inject } from '@angular/core';

export const loginGuard: CanActivateFn = (route, state) => {
  const authService = inject(LoginService);
  const router = inject(Router)
  const token = authService.getToken();
  if (!token) {
    router.navigate(['/login'])
    return false;
  };
  return true;
};
