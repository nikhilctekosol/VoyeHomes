import { NbMenuItem } from '@nebular/theme';

export const MENU_ITEMS: NbMenuItem[] = [
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

      {
        title: 'Reservation',
        link: '/pages/operations/reservation',
      },
      
    ],
  },
  
];
