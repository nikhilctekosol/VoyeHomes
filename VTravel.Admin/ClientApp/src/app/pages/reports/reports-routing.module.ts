import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ReportsComponent } from './reports.component';
import { AvailabilityComponent } from './availability/availability.component';


const routes: Routes = [{
  path: '',
  component: ReportsComponent,
  children: [
    {
      path: 'availability',
      component: AvailabilityComponent,
    }
  ],
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportsRoutingModule { }
export const routedComponents = [
  ReportsComponent,
  AvailabilityComponent
];
