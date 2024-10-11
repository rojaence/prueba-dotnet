import { CanActivateFn, Router } from '@angular/router';
import { LoginService } from '../services/login.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(LoginService);
  const router = inject(Router)

  const expectedRole = route.data["expectedRole"];
  /* if (!authService.getRole() || authService.getRole() !== expectedRole) {
    router.navigate(['/unauthorized']);
    return false;
  } */
  return true;
};
