import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ReportsComponent } from './reports.component';
import { AvailabilityComponent } from './availability/availability.component';
import { SettlementComponent } from './settlement/settlement.component';


const routes: Routes = [{
  path: '',
  component: ReportsComponent,
  children: [
    {
      path: 'availability',
      component: AvailabilityComponent,
    },
    {
      path: 'settlement',
      component: SettlementComponent,
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
  AvailabilityComponent,
  SettlementComponent
];
