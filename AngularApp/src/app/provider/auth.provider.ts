import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { IAuth } from '../models';
import { TokenStorageService } from '../services';

@Injectable({
  providedIn: 'root',
})

export class AuthProvider implements CanActivate {
  private accessTokenSubject = new BehaviorSubject<string | undefined>(undefined);

  accessToken$ = this.accessTokenSubject.asObservable();

  constructor(private tokenStorage: TokenStorageService ) {
    try {
      const accessToken = this.tokenStorage.getAccessToken();
      if (accessToken) {
        this.tokenStorage.saveToken(accessToken);
      } else {
        this.tokenStorage.signOut();
      }
    } catch {
      this.tokenStorage.signOut();
    }
  }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    return this.isAuthenticated();
  }

  public clearsessionStorage() {
    this.setAccessToken(undefined);
    sessionStorage.clear();
  }

  private setAccessToken(token: string | undefined) {
    this.accessTokenSubject.next(token);
  }

  isAuthenticated(): boolean {
    const accessToken = this.accessTokenSubject.getValue() ?? this.tokenStorage.getAccessToken();
    if (accessToken === null || accessToken === undefined) {
      this.tokenStorage.signOut();
      return false;
    }
    return true;
  }

  createAccessToken(auth: IAuth): void {
    this.tokenStorage.saveToken(auth.accessToken);
  }
}
