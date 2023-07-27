import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgxAuthRoutingModule } from './auth-routing.module';
import { NgxAuthComponent } from './auth.component';
import { NbAuthModule } from '@nebular/auth';
import { NbAlertModule, NbButtonModule, NbCheckboxModule, NbInputModule,  NbCardModule, NbLayoutModule } from '@nebular/theme';
import { NgxLoginComponent } from './login/login.component'; // <---
import { NgxLogoutComponent } from './logout/logout.component'; // <---
import { NgxRequestPasswordComponent } from './request-password/request-password.component'; // <---
import { NgxResetPasswordComponent } from './reset-password/reset-password.component'; // <---


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    NbAlertModule,
    NbInputModule,
    NbButtonModule,
    NbCheckboxModule,
    NgxAuthRoutingModule,
    NbAuthModule,
    NbCardModule,
    NbLayoutModule,
  ],
  declarations: [
    NgxAuthComponent,
    NgxLoginComponent, // <---
    NgxLogoutComponent,
    NgxRequestPasswordComponent,
    NgxResetPasswordComponent,
  ],
})
export class NgxAuthModule {
}
