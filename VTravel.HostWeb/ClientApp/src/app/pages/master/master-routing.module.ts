import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { MasterComponent } from './master.component';
import { TagsComponent } from './tags/tags.component';
import { CitiesComponent } from './cities/cities.component';
import { DestinationsComponent } from './destinations/destinations.component';
import { AmenitiesComponent } from './amenities/amenities.component';
import { PropertyTypesComponent } from './property-types/property-types.component';
import { RoomTypesComponent } from './room-types/room-types.component';
import { BookingChannelsComponent } from './booking-channels/booking-channels.component';

const routes: Routes = [{
  path: '',
  component: MasterComponent,
  children: [
    {
      path: 'tags',
      component: TagsComponent,
    }
    , {
      path: 'amenities',
      component: AmenitiesComponent,
    },
    {
      path: 'cities',
      component: CitiesComponent,
    },
    {
      path: 'destinations',
      component: DestinationsComponent,
    },
    {
      path: 'property-types',
      component: PropertyTypesComponent,
    },
    {
      path: 'room-types',
      component: RoomTypesComponent,
    },
    {
      path: 'booking-channels',
      component: BookingChannelsComponent,
    },
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
export class MasterRoutingModule { }

export const routedComponents = [
  MasterComponent,
  TagsComponent,
  AmenitiesComponent,
  CitiesComponent,
  DestinationsComponent,
  PropertyTypesComponent,
  RoomTypesComponent,
  BookingChannelsComponent
];
