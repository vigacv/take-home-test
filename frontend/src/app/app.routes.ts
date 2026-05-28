import { Routes } from '@angular/router';
import { authGuard } from './auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'loans',
    loadComponent: () =>
      import('./loans/loans.component').then((m) => m.LoansComponent),
    canActivate: [authGuard],
  },
  { path: '', redirectTo: 'loans', pathMatch: 'full' },
  { path: '**', redirectTo: 'loans' },
];
