import { Injectable } from '@angular/core';
import { IAppointment } from '../models/appointment';
import { IDoctor } from '../models/doctor';
import { v4 as uuidv4 } from 'uuid';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { IAppointmentWithPatientName } from '../models/appointmentWithPatientName';

@Injectable({
  providedIn: 'root',
})
export class HomeService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getDoctors(sort: boolean) {
    return this.http.get<IDoctor[]>(
      this.baseUrl + 'doctors?filter=' + String(sort)
    );
  }

  createAppointment(newAppointmentStartTime: Date, doctorId: string) {
    newAppointmentStartTime = new Date(
      newAppointmentStartTime.getFullYear(),
      newAppointmentStartTime.getMonth(),
      newAppointmentStartTime.getDate(),
      newAppointmentStartTime.getHours(),
      newAppointmentStartTime.getMinutes(),
      0
    );
    newAppointmentStartTime = new Date(
      newAppointmentStartTime.getTime() + 120 * 60000
    );
    const newAppointmentEndTime = new Date(
      newAppointmentStartTime.getTime() + 3 * 60000
    );
    const appointment = {
      id: uuidv4(),
      patientId: localStorage.getItem('email'),
      doctorId,
      startTime: newAppointmentStartTime.toISOString(),
      endTime: newAppointmentEndTime.toISOString(),
    };

    return this.http.post<IAppointment>(
      this.baseUrl + 'appointments',
      appointment
    );
  }

  checkIfAppointmentArrived() {
    const patientEmail = {
      email: localStorage.getItem('email'),
    };

    return this.http.get<IAppointment>(this.baseUrl + 'appointments', {
      headers: { skip: 'true' },
      params: patientEmail,
    });
  }

  getDoctorAppointments(doctorId: string) {
    const id = {
      id: doctorId,
    };

    return this.http.get<IAppointment[]>(
      this.baseUrl + 'doctors/appointments',
      { params: id }
    );
  }

  getPatientAppointments(doctorId: string) {
    const data = {
      patientEmail: localStorage.getItem('email'),
      doctorId,
    };

    return this.http.get<IAppointment[]>(
      this.baseUrl + 'patients/appointments',
      { params: data }
    );
  }

  isDoctorAvailable(id: string) {
    const data = {
      id,
    };

    return this.http.get(this.baseUrl + 'doctors/available', { params: data });
  }

  isDoctorAvailableAt(newAppointmentStartTime: Date, doctorId: string) {
    newAppointmentStartTime = new Date(
      newAppointmentStartTime.getFullYear(),
      newAppointmentStartTime.getMonth(),
      newAppointmentStartTime.getDate(),
      newAppointmentStartTime.getHours(),
      newAppointmentStartTime.getMinutes(),
      0
    );
    newAppointmentStartTime = new Date(newAppointmentStartTime.getTime());
    const newAppointmentEndTime = new Date(
      newAppointmentStartTime.getTime() + 3 * 60000
    );

    const data = {
      id: doctorId,
      appointmentStartTime: newAppointmentStartTime.toISOString(),
      appointmentEndTime: newAppointmentEndTime.toISOString(),
    };

    return this.http.get(this.baseUrl + 'doctors/availableat', {
      params: data,
    });
  }

  getDoctorAppointmentsByDoctorEmail(doctorEmail: string) {
    return this.http.get<IAppointmentWithPatientName[]>(
      this.baseUrl + 'doctors/appointments/email?email=' + doctorEmail,
      { headers: { skip: 'true' } }
    );
  }

  deleteAppointment(appointmentId: string) {
    return this.http.delete(
      this.baseUrl + 'appointments?appointmentid=' + appointmentId
    );
  }
}
