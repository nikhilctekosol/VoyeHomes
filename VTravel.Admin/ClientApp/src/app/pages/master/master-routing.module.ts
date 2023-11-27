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
import { UsersComponent } from './users/users.component';
import { BannersComponent } from './banners/banners.component';
import { RateplanComponent } from './rateplan/rateplan.component';
import { OwnersComponent } from './owners/owners.component';
import { TdsComponent } from './tds/tds.component';
import { AltcontactComponent } from './altcontact/altcontact.component';

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
    {
      path: 'users',
      component: UsersComponent,
    },
    {
      path: 'banners',
      component: BannersComponent,
    },
    {
      path: 'rateplan',
      component: RateplanComponent,
    },
    {
      path: 'owners',
      component: OwnersComponent,
    },
    {
      path: 'tds',
      component: TdsComponent,
    },
    {
      path: 'altcontact',
      component: AltcontactComponent,
    }
   // {
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
  BookingChannelsComponent,
  UsersComponent,
  BannersComponent,
  RateplanComponent,
  OwnersComponent,
  TdsComponent,
  AltcontactComponent
];
