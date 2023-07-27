import { NgModule } from '@angular/core';
import { Ng2SmartTableModule } from 'ng2-smart-table';

import { ThemeModule } from '../../@theme/theme.module';
import { OperationsRoutingModule, routedComponents } from './operations-routing.module';
import {
  NbCardModule, NbSpinnerModule, NbDialogModule, NbAlertModule,
  NbInputModule, NbSelectModule, NbTabsetModule, NbIconModule, NbCheckboxModule, NbToastrModule, NbDatepickerModule,
} from '@nebular/theme';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { CKEditorModule } from 'ng2-ckeditor';
import { GoogleMapsModule } from '@angular/google-maps';
import { FullCalendarModule } from '@fullcalendar/angular';

@NgModule({
  imports: [    
    DragDropModule,
    ThemeModule,
    OperationsRoutingModule,
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
    FullCalendarModule,
    NbDatepickerModule
  ],
  declarations: [
    ...routedComponents,
  ],
})
export class OperationsModule { }
