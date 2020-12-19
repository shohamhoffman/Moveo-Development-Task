import { Component, OnInit } from '@angular/core';
import { AccountService } from './account/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'Moveo Home Task';

  constructor(private accountService: AccountService) {}

  ngOnInit(): void {
    // Loads the user when entering the site.
    this.loadCurrentUser();
  }

  loadCurrentUser() {
    const token = localStorage.getItem('token');
    this.accountService.loadCurrentUser(token).subscribe(
      () => {},
      (error) => {
        console.log(error);
      }
    );
  }
}
