import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PatientComponent } from './patient/patient.component';
import { DoctorComponent } from './doctor/doctor.component';
import { HomeRoutingModule } from './home-routing.module';
import { DateTimePickerModule } from '@syncfusion/ej2-angular-calendars';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { AppointmentsDialogComponent } from './patient/appointments-dialog/appointments-dialog.component';
import { ToastrModule } from 'ngx-toastr';
import { HomeComponent } from './home.component';
import { Ng2FlatpickrModule } from 'ng2-flatpickr';
import { AppointmentCreationDialogComponent } from './patient/appointment-creation-dialog/appointment-creation-dialog.component';

@NgModule({
  declarations: [
    PatientComponent,
    DoctorComponent,
    AppointmentsDialogComponent,
    HomeComponent,
    AppointmentCreationDialogComponent,
  ],
  entryComponents: [AppointmentsDialogComponent],
  imports: [
    CommonModule,
    HomeRoutingModule,
    DateTimePickerModule,
    MatDialogModule,
    MatButtonModule,
    Ng2FlatpickrModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
      preventDuplicates: true,
    }),
  ],
})
export class HomeModule {}
