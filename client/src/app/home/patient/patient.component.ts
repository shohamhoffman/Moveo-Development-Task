import { Component, OnDestroy, OnInit } from '@angular/core';
import { IDoctor } from 'src/app/models/doctor';
import { HomeService } from '../home.service';
import { MatDialog } from '@angular/material/dialog';
import { AppointmentsDialogComponent } from './appointments-dialog/appointments-dialog.component';
import { IUser } from 'src/app/models/user';
import { AccountService } from 'src/app/account/account.service';
import { ToastrService } from 'ngx-toastr';
import { AppointmentCreationDialogComponent } from './appointment-creation-dialog/appointment-creation-dialog.component';
import { interval, Observable, Subscription } from 'rxjs';

@Component({
  selector: 'app-patient',
  templateUrl: './patient.component.html',
  styleUrls: ['./patient.component.scss'],
})
export class PatientComponent implements OnInit, OnDestroy {
  doctors: IDoctor[];
  sort = false;
  currentUser$: Observable<IUser>;
  subscriptions: Subscription[] = [];

  constructor(
    private homeService: HomeService,
    private accountService: AccountService,
    private dialog: MatDialog,
    private toastr: ToastrService
  ) {}

  // Gets the current patient that is logged in and the doctors.
  ngOnInit(): void {
    this.currentUser$ = this.accountService.currentUser$;
    this.getDoctors();
    // Checks if there is an appointment that starts now. Sends request every 3 sec.
    // If there is an appointment that starts now, notify the user and delete that appointment after 3 min.
    // The deletion timer is set in the api.
    this.subscriptions.push(
      interval(3000).subscribe(() => {
        this.homeService.checkIfAppointmentArrived().subscribe(
          (response) => {
            if (response != null) {
              this.toastr.success(
                'Hi ! You have an appointment that starts now.'
              );
              this.assignDoctorAvailability();
            }
          },
          (error) => {
            console.log(error);
          }
        );
      })
    );
  }

  getDoctors(sort = false) {
    this.doctors = [];
    this.homeService.getDoctors(sort).subscribe(
      (response) => {
        this.doctors = response;
        // Assign availability to each doctor.
        this.assignDoctorAvailability();
      },
      (error) => {
        console.log(error);
      }
    );
  }

  filterByAvailability(event: any) {
    // If the sort is false -> will become true,
    // If true -> will become false.
    this.sort = this.sort === false ? true : false;

    // Gets the doctors that matches the filter (available or not).
    this.getDoctors(this.sort);
  }

  // Checks if the doctor available at a certain time.
  isDoctorAvailableAt(newAppointmentTime: Date, doctor: IDoctor) {
    this.homeService
      .isDoctorAvailableAt(newAppointmentTime, doctor.id)
      .subscribe(
        (response) => {
          if (response === true) {
            // If the doctor available, create the appointment.
            this.homeService
              .createAppointment(newAppointmentTime, doctor.id)
              .subscribe(
                () => {
                  this.toastr.success(
                    'Appointment Created ! You will be notified  when the appointment starts.'
                  );
                  // Update the availability after creating an appointment.
                  this.assignDoctorAvailability();
                },
                (error) => {
                  console.log(error);
                }
              );
          } else {
            this.toastr.error(
              `We are sorry, but doctor ${doctor.name} is not available at this time. Try to select another time.`
            );
          }
        },
        (error) => {
          console.log(error);
        }
      );
  }

  // Opens the appointment creation dialog.
  openAppointmentCreationDialog(doctor: IDoctor) {
    const dialogRef = this.dialog.open(AppointmentCreationDialogComponent, {
      position: { top: '10%' },
      data: {
        doctor,
      },
    });

    dialogRef.afterClosed().subscribe(
      (res) => {
        if (res !== '') {
          this.isDoctorAvailableAt(res, doctor);
        }
      },
      (error) => {
        console.log(error);
      }
    );
  }

  // Opens the appointment dialog.
  openAppointmentsDialog(doctor: IDoctor, allAppointments = false) {
    const dialogRef = this.dialog.open(AppointmentsDialogComponent, {
      data: {
        doctor,
        allAppointments,
      },
    });

    dialogRef.afterClosed().subscribe(
      (res) => {
        // Update the availability after deleting an appointment.
        if (res) {
          this.assignDoctorAvailability();
        }
      },
      (error) => {
        console.log(error);
      }
    );
  }

  // Checks the availability of each doctor and assign it.
  private assignDoctorAvailability() {
    for (const doctor of this.doctors) {
      this.homeService.isDoctorAvailable(doctor.id).subscribe(
        (response) => {
          if (response === true) {
            doctor.available = true;
          } else {
            doctor.available = false;
          }
        },
        (error) => {
          console.log(error);
        }
      );
    }
  }

  logout() {
    this.accountService.logout();
  }

  ngOnDestroy() {
    this.subscriptions.forEach((subscription) => subscription.unsubscribe());
  }
}
