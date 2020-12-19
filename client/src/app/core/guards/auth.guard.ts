import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
} from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from 'src/app/account/account.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private accountService: AccountService, private router: Router) {}

  // Checks if the user is authenticated and if his role exists in the allowed users of the route.
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map((auth) => {
        if (auth) {
          this.getUserRole(next);
          return true;
        }
        this.router.navigate(['account/login']);
        return false;
      })
    );
  }

  getUserRole(next: ActivatedRouteSnapshot) {
    this.accountService.getUserRole().subscribe(
      (res) => {
        if (next.data.role && next.data.role.indexOf(res) === -1) {
          this.router.navigate(['account/login']);
          return false;
        }
      },
      (error) => {
        console.log(error);
      }
    );
  }
}
