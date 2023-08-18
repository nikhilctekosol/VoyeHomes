import { Component } from '@angular/core';
import { NbAuthJWTToken,NbAuthService } from '@nebular/auth';
/*import { MENU_ITEMS } from './pages-menu';*/

@Component({
  selector: 'ngx-pages',
  styleUrls: ['pages.component.scss'],
  template: `
    <ngx-one-column-layout>
      <nb-menu [items]="menu"></nb-menu>
      <router-outlet></router-outlet>
    </ngx-one-column-layout>
  `,
})
export class PagesComponent {

/* menu = MENU_ITEMS;*/
  menu = [];
  user : any;
  constructor(private authService: NbAuthService) {

    this.authService.onTokenChange()
      .subscribe((token: NbAuthJWTToken) => {

        if (token.isValid()) {
          this.user = token.getPayload();

          //console.log(JSON.stringify(this.user));


          this.menu = [
            {
              title: 'Dashboard',
              icon: 'shopping-cart-outline',
              link: '/pages/dashboard',
              home: true,
            },
            {
              title: 'MANAGE',
              group: true,
            },
            {
              title: 'Operations',
              icon: 'layout-outline',
              children: [

                //{
                //  title: 'Enquiries',
                //  link: '/pages/operations/enquiries',
                //},

                {
                  title: 'Reservation',
                  link: '/pages/operations/reservation',
                },
                {
                  title: 'Inventory',
                  "hidden": this.user.role != 'ADMIN' ? true : false,
                  link: '/pages/operations/inventory',
                },
              ],
            },
            {
              title: 'CMS',
              "hidden": this.user.role != 'ADMIN' ? true : false,
              group: true,
            },
            {
              title: 'Catalog',
              "hidden": this.user.role != 'ADMIN' ? true : false,
              icon: 'list-outline',
              children: [

                {
                  title: 'Properties',
                  link: '/pages/catalog/properties',
                },
                {
                  title: 'Pages',
                  link: '/pages/catalog/pages',
                },
              ],
            },
            {
              title: 'Master',
              "hidden": this.user.role != 'ADMIN' ? true : false,
              icon: 'layout-outline',
              children: [

                {
                  title: 'Tags',
                  link: '/pages/master/tags',
                },
                {
                  title: 'Amenities',
                  link: '/pages/master/amenities',
                },
                {
                  title: 'Cities',
                  link: '/pages/master/cities',
                },
                {
                  title: 'Destinations',
                  link: '/pages/master/destinations',
                },
                {
                  title: 'Property Types',
                  link: '/pages/master/property-types',
                },
                {
                  title: 'Room Types',
                  link: '/pages/master/room-types',
                },
                {
                  title: 'Booking Channels',
                  link: '/pages/master/booking-channels',
                },
                {
                  title: 'Users',
                  link: '/pages/master/users',
                },
              ],

            },
            {
              title: 'Partner',
              "hidden": this.user.role != 'ADMIN' ? true : false,
              //icon: 'people-outline',
              children: [
                {
                  title: 'Users',
                  link: '/pages/partner/users',
                },
              ],

            },
            {
              title: 'REPORTS',
              group: true ,
            },
            {
              title: 'Reports',
              //"hidden": this.user.role != 'ADMIN' ? true : false,
              //icon: 'people-outline',
              children: [
                {
                  title: 'Availability',
                  link: '/pages/reports/availability',
                },
              ],

            },

          ];
        }

      });

  }
}
