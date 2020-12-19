import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { of, ReplaySubject } from 'rxjs';
import { IUser } from '../models/user';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { v4 as uuid } from 'uuid';
import { environment } from 'src/environments/environment';
import { IPatient } from '../models/patient';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  loadCurrentUser(token: string) {
    if (token === null) {
      this.currentUserSource.next(null);
      return of(null);
    }

    return this.http.get(this.baseUrl + 'account').pipe(
      map((user: IUser) => {
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('email', user.email);
          this.currentUserSource.next(user);
        }
      })
    );
  }

  register(values: any) {
    return this.http.post(this.baseUrl + 'account/register', values).pipe(
      map((user: IUser) => {
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('email', user.email);
          this.currentUserSource.next(user);
        }
      })
    );
  }

  login(values: any) {
    return this.http.post(this.baseUrl + 'account/login', values).pipe(
      map((user: IUser) => {
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('email', user.email);
          this.currentUserSource.next(user);
        }
      })
    );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('email');
    this.currentUserSource.next(null);
    this.router.navigateByUrl('/account/login');
  }

  checkEmailExists(email: string) {
    return this.http.get(this.baseUrl + 'account/emailexists?email=' + email);
  }

  // Creates the user in the sql patient / doctor tables (moveo.db), not in the user table (identity.db).
  createUserType(values: any) {
    if (values.userType === 'patient') {
      return this.createPatient(values.displayName, values.email);
    } else if (values.userType === 'doctor') {
      return this.createDoctor(values.displayName, values.doctorField);
    }
  }

  private createPatient(patientName: string, patientEmail: string) {
    const patient: IPatient = {
      id: uuid(),
      email: patientEmail,
      name: patientName,
    };
    return this.http.post(this.baseUrl + 'patients', patient);
  }

  private createDoctor(doctorName: string, doctorField: string) {
    const doctor = {
      id: uuid(),
      email: localStorage.getItem('email'),
      name: doctorName,
      doctorField,
    };
    return this.http.post(this.baseUrl + 'doctors', doctor);
  }

  getUserRole() {
    return this.http.get(this.baseUrl + 'account/type', {
      responseType: 'text',
    });
  }
}
