import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { OperationsComponent } from './operations.component';
import { ReservationComponent } from './reservation/reservation.component';
import { InventoryComponent } from './inventory/inventory.component';
import { EnquiriesComponent } from './enquiries/enquiries.component';
import { EnquiryComponent } from './enquiry/enquiry.component';


const routes: Routes = [{
  path: '',
  component: OperationsComponent,
  children: [
    {
      path: 'enquiries',
      component: EnquiriesComponent,
    },
    {
      path: 'enquiry/:id',
      component: EnquiryComponent,
    }, {
      path: 'reservation',
      component: ReservationComponent,
    },
    {
      path: 'inventory',
      component: InventoryComponent,
    }, 
    ],
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OperationsRoutingModule { }

export const routedComponents = [
  OperationsComponent,
  ReservationComponent,
  InventoryComponent,
  EnquiriesComponent,
  EnquiryComponent
];
