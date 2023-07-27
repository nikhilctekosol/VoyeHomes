import { ExtraOptions, RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { AuthGuard } from './auth-guard.service';
import {
} from '@nebular/auth';

const routes: Routes = [
  { path: 'pages', canActivate: [AuthGuard], loadChildren: 'app/pages/pages.module#PagesModule' },
  {
    path: 'auth', loadChildren: 'app/auth/auth.module#NgxAuthModule'

  },
  { path: '', redirectTo: 'pages', pathMatch: 'full' },
  { path: '**', redirectTo: 'pages' },
];

const config: ExtraOptions = {
  useHash: false,
};

@NgModule({
  imports: [RouterModule.forRoot(routes, config)],
  exports: [RouterModule],
})
export class AppRoutingModule {
}
