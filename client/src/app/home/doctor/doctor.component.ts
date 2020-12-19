import { Component, OnDestroy, OnInit } from '@angular/core';
import { interval, Observable, Subscription } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';
import { IAppointmentWithPatientName } from 'src/app/models/appointmentWithPatientName';
import { IUser } from 'src/app/models/user';
import { HomeService } from '../home.service';

@Component({
  selector: 'app-doctor',
  templateUrl: './doctor.component.html',
  styleUrls: ['./doctor.component.scss'],
})
export class DoctorComponent implements OnInit, OnDestroy {
  currentUser$: Observable<IUser>;
  appointments: IAppointmentWithPatientName[] = [];
  subscriptions: Subscription[] = [];

  constructor(
    private accountService: AccountService,
    private homeService: HomeService
  ) {}

  // Gets the current doctor that is logged in and his appointments.
  ngOnInit(): void {
    this.currentUser$ = this.accountService.currentUser$;
    this.getAppointments();
  }

  // Gets the appointments of the doctor that is logged in. Sends request every 3 sec.
  getAppointments() {
    this.subscriptions.push(
      interval(3000).subscribe(() => {
        this.homeService
          .getDoctorAppointmentsByDoctorEmail(localStorage.getItem('email'))
          .subscribe(
            (res) => {
              this.appointments = res;
            },
            (error) => {
              console.log(error);
            }
          );
      })
    );
  }

  logout() {
    this.accountService.logout();
  }

  ngOnDestroy() {
    this.subscriptions.forEach((subscription) => subscription.unsubscribe());
  }
}
