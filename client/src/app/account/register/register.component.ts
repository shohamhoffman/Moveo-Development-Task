import { Component, OnInit } from '@angular/core';
import {
  AsyncValidatorFn,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { of, timer } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  errors: string[];
  type: string;

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      userType: [null, [Validators.required]],
      displayName: [null, [Validators.required]],
      doctorField: [null, [Validators.required]],
      email: [
        null,
        [
          Validators.required,
          Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$'),
        ],
        [this.validateEmailNotTaken()],
      ],
      password: [
        null,
        [
          Validators.required,
          Validators.pattern(
            '^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$'
          ),
        ],
      ],
    });
  }

  validateEmailNotTaken(): AsyncValidatorFn {
    return (control) => {
      // Adds a timer for 500ms before the validation starts.
      return timer(500).pipe(
        switchMap(() => {
          if (!control.value) {
            return of(null);
          }
          return this.accountService.checkEmailExists(control.value).pipe(
            map((res) => {
              return res ? { emailExists: true } : null;
            })
          );
        })
      );
    };
  }

  // Shows the registration form only after choosing the user type. Patient / Doctor.
  showForm(event?: any) {
    // If there is no type, assign patient / doctor to type.
    if (!this.type) {
      this.type = event.target.value;
    }

    if (this.type === 'patient') {
      this.registerForm.controls.doctorField.disable();
    } else {
      this.registerForm.controls.doctorField.enable();
    }
  }

  // Change the user type from doctor to patient, Or from patient to doctor. Based on the current type.
  changeUserType() {
    if (this.type === 'doctor') {
      this.type = 'patient';
      this.registerForm.reset();
      this.registerForm.controls.userType.setValue('patient');
    } else {
      this.type = 'doctor';
      this.registerForm.reset();
      this.registerForm.controls.userType.setValue('doctor');
    }
    this.showForm();
  }

  onSubmit() {
    this.accountService.register(this.registerForm.value).subscribe(
      () => {
        this.accountService.createUserType(this.registerForm.value).subscribe(
          () => {
            this.router.navigateByUrl('/');
          },
          (error) => {
            console.log(error);
          }
        );
      },
      (error) => {
        console.log(error);
        this.errors = error.errors;
      }
    );
  }
}
