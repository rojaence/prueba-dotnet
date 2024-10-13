import { CanActivateFn, Router } from '@angular/router';
import { LoginService } from '../services/login.service';
import { inject } from '@angular/core';
import { combineLatest, map } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const loginService = inject(LoginService);
  const router = inject(Router)

  const expectedRole = route.data["expectedRole"];
  return combineLatest([loginService.userData$, loginService.isAuthenticated()]).pipe(
    map(([user, authStatus]) => {
      if (!authStatus) {
        router.navigate(['/login']);
        return false;
      }

      if (user && user.role === expectedRole) {
        return true;
      } else {
        router.navigate(['/unauthorized']);
        return false;
      }
    })
  );
};
