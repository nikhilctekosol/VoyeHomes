import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { CatalogComponent } from './catalog.component';
import { PropertiesComponent } from './properties/properties.component';
import { PropertyComponent } from './property/property.component';
import { PagesComponent } from './pages/pages.component';
import { PageComponent } from './page/page.component';

const routes: Routes = [{
  path: '',
  component: CatalogComponent,
  children: [
    {
      path: 'properties',
      component: PropertiesComponent,
    },
    {
      path: 'property/:id',
      component: PropertyComponent,
    },
    {
      path: 'pages',
      component: PagesComponent,
    }
    ,
    {
      path: 'page/:id',
      component: PageComponent,
    }
    ],
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CatalogRoutingModule { }

export const routedComponents = [
  CatalogComponent,
  PropertiesComponent,
  PropertyComponent,
  PagesComponent,
  PageComponent
];
