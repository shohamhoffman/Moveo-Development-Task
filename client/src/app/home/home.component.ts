import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  userRole: string;

  constructor(private accountService: AccountService) {}

  // Gets the user type and shows the right component.
  // If the user type is patient -> the user will see the patient component,
  // If the user type is doctor -> the user will see the doctor component.
  ngOnInit(): void {
    this.accountService.getUserRole().subscribe(
      (res) => {
        this.userRole = res;
      },
      (error) => {
        console.log(error);
      }
    );
  }
}
