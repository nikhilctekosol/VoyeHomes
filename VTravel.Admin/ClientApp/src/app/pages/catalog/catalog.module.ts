import { NgModule } from '@angular/core';
import { Ng2SmartTableModule } from 'ng2-smart-table';

import { ThemeModule } from '../../@theme/theme.module';
import { CatalogRoutingModule, routedComponents } from './catalog-routing.module';
import {
  NbCardModule, NbSpinnerModule, NbDialogModule, NbAlertModule, NbDatepickerModule,
  NbInputModule, NbSelectModule, NbTabsetModule, NbIconModule, NbCheckboxModule, NbToastrModule
} from '@nebular/theme';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { CKEditorModule } from 'ng2-ckeditor';
import { GoogleMapsModule } from '@angular/google-maps';

@NgModule({
  imports: [    
    DragDropModule,
    ThemeModule,
    CatalogRoutingModule,
    Ng2SmartTableModule,
    NbCardModule,
    NbSpinnerModule,
    FormsModule,
    NbAlertModule,
    NbInputModule,
    NbSelectModule,
    NbDialogModule,
    NbTabsetModule,
    NbIconModule,
    CKEditorModule,
    NbCheckboxModule,
    GoogleMapsModule,
    NbToastrModule,
    NbDatepickerModule
  ],
  declarations: [
    ...routedComponents,
  ],
})
export class CatalogModule { }
