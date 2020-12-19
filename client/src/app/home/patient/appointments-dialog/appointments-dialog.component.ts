import { Component, Inject, Input, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IAppointment } from 'src/app/models/appointment';
import { IDoctor } from 'src/app/models/doctor';
import { HomeService } from '../../home.service';

@Component({
  selector: 'app-appointments-dialog',
  templateUrl: './appointments-dialog.component.html',
  styleUrls: ['./appointments-dialog.component.scss'],
})
export class AppointmentsDialogComponent implements OnInit {
  doctor: IDoctor;
  appointments: IAppointment[] = [];
  allAppointments = true;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private homeService: HomeService
  ) {}

  ngOnInit(): void {
    // Check if the dialog should
    // show all the appointments of the doctor (in case you clicked on the doctors waiting list)
    // Or show only the paitient appointments for that doctor (int case you clicked my appointments).
    this.doctor = this.data.doctor;
    if (this.data.allAppointments === true) {
      this.getDoctorAppointments(this.doctor.id);
    } else {
      this.allAppointments = false;
      this.getPatientAppointments(this.doctor.id);
    }
  }

  private getDoctorAppointments(doctorId: string) {
    this.homeService.getDoctorAppointments(doctorId).subscribe(
      (response) => {
        this.appointments = response;
      },
      (error) => {
        console.log(error);
      }
    );
  }

  private getPatientAppointments(doctorId: string) {
    this.homeService.getPatientAppointments(doctorId).subscribe(
      (response) => {
        this.appointments = response;
      },
      (error) => {
        console.log(error);
      }
    );
  }

  isAppointmentStarted(startTime) {
    return new Date(startTime) <= new Date();
  }

  deleteAppointment(appoinmentId: string) {
    this.homeService.deleteAppointment(appoinmentId).subscribe(
      () => {
        // Refresh the dialog view so that after the appointment deletion,
        // the dialog won't show that appointment anymore.
        if (this.data.allAppointments === true) {
          this.getDoctorAppointments(this.doctor.id);
        } else {
          this.allAppointments = false;
          this.getPatientAppointments(this.doctor.id);
        }
      },
      (error) => {
        console.log(error);
      }
    );
  }
}
