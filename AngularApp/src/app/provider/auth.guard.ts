import { Injectable } from "@angular/core";
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from "@angular/router";
import { AuthServiceBase } from "../services/auth/auth.abstract.service";

@Injectable({ providedIn: 'root' })

export class AuthGuard implements CanActivate {
  constructor(protected authService: AuthServiceBase, private router: Router) {}

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (this.authService.isAuthenticated()) {
      return true;
    }
    this.router.navigate(['/']);
    return false;
  }
}
