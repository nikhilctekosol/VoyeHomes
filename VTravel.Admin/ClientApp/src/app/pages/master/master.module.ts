import { NgModule } from '@angular/core';
import { Ng2SmartTableModule } from 'ng2-smart-table';

import { ThemeModule } from '../../@theme/theme.module';
import { MasterRoutingModule, routedComponents } from './master-routing.module';
import {
  NbCardModule, NbSpinnerModule, NbDialogModule, NbAlertModule,
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
    MasterRoutingModule,
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
    NbToastrModule
  ],
  declarations: [
    ...routedComponents,
  ],
})
export class MasterModule { }
