import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { PartnerComponent } from './partner.component';
import { UsersComponent } from './users/users.component';

const routes: Routes = [{
  path: '',
  component: PartnerComponent,
  children: [
    {
      path: 'users',
      component: UsersComponent,
    }
   //{
  //    path: 'view-request/:id',
  //    component: ViewRequestComponent,
  //  }
    ],
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PartnerRoutingModule { }

export const routedComponents = [
  PartnerComponent,
  UsersComponent
];
