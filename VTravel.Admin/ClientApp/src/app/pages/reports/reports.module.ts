import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ThemeModule } from '../../@theme/theme.module';
import { ReportsRoutingModule, routedComponents } from './reports-routing.module';
import {
  NbCardModule, NbSpinnerModule, NbDialogModule, NbAlertModule,
  NbInputModule, NbSelectModule, NbTabsetModule, NbIconModule,
  NbCheckboxModule, NbToastrModule, NbDatepickerModule, NbListModule, NbActionsModule, NbAutocompleteModule
} from '@nebular/theme';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


@NgModule({
  declarations: [...routedComponents],
  imports: [
    ThemeModule,
    CommonModule,
    ReportsRoutingModule,
    FormsModule,
    NbCardModule,
    NbSpinnerModule,
    NbDialogModule,
    NbAlertModule,
    NbInputModule,
    NbSelectModule,
    NbTabsetModule,
    NbIconModule,
    NbCheckboxModule,
    NbToastrModule,
    NbDatepickerModule,
    NbListModule,
    NbActionsModule,
    NbAutocompleteModule
  ]
})
export class ReportsModule { }
