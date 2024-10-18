import { CanActivateFn, Router } from '@angular/router';
import { LoginService } from '../services/login.service';
import { inject } from '@angular/core';
import { combineLatest, map } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const loginService = inject(LoginService);
  const router = inject(Router)

  // const expectedRole = route.data["expectedRole"];
  const expectedRoles: string[] = route.data["expectedRoles"];
  return combineLatest([loginService.isAuthenticated(), loginService.userData$]).pipe(
    map(([authStatus, user]) => {
      if (!authStatus.authenticated) {
        router.navigate(['/login']);
        return false;
      }

      if (user && expectedRoles.includes(user.role)) {
        return true;
      } else {
        router.navigate(['/unauthorized']);
        return false;
      }
    })
  );
};
