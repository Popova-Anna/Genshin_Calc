import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountStoreService } from '../services/account-store.service';

/** Redirects to the home/import page when no account has been analyzed yet. */
export const hasDataGuard: CanActivateFn = () => {
  const store = inject(AccountStoreService);
  const router = inject(Router);

  return store.hasData() || router.createUrlTree(['/']);
};
