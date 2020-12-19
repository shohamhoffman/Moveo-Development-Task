import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FlatpickrOptions } from 'ng2-flatpickr';
import { IDoctor } from 'src/app/models/doctor';

@Component({
  selector: 'app-appointment-creation-dialog',
  templateUrl: './appointment-creation-dialog.component.html',
  styleUrls: ['./appointment-creation-dialog.component.scss'],
})
export class AppointmentCreationDialogComponent implements OnInit {
  // Date time picker options.
  dateOptions: FlatpickrOptions = {
    time_24hr: true,
    altInput: true,
    allowInput: true,
    enableTime: true,
    minuteIncrement: 1,
    dateFormat: 'Y-M-D h:i',
    minDate: new Date(),
    maxDate: new Date(new Date().getFullYear() + 1, new Date().getMonth() + 1),
    disable: [
      function (date) {
        return date.getDay() === 5 || date.getDay() === 6;
      },
    ],
    // For the purpose of the Moveo task, I disabled minTime and maxTime.
    // In a real-world application the time was between the doctors work hours.
    //minTime: "08:00",
    //maxTime: "17:00",
    defaultDate: new Date(),
    onClose: (selectedDates) => this.onSelectedDate(selectedDates),
  };

  appointmentStart: Date;
  appointmentEnd: Date;
  isValidDate = false;
  doctor: IDoctor;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}

  ngOnInit(): void {
    this.doctor = this.data.doctor;
  }

  onSelectedDate(dates: any) {
    if (dates.length > 0) {
      this.appointmentStart = dates[0];
      // Set the appointment end time to 3 min after the appointment start time.
      this.appointmentEnd = new Date(
        this.appointmentStart.getFullYear(),
        this.appointmentStart.getMonth(),
        this.appointmentStart.getDate(),
        this.appointmentStart.getHours(),
        this.appointmentStart.getMinutes() + 3
      );

      // Check if the time the user picked is valid.
      if (dates[0] > Date.now()) {
        this.isValidDate = true;
      }
    }
  }
}
