import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { IAuth } from '../models';

@Injectable({
  providedIn: 'root',
})

export class AuthProvider implements CanActivate {
  private accessTokenSubject = new BehaviorSubject<string | undefined>(undefined);

  accessToken$ = this.accessTokenSubject.asObservable();

  constructor() {
    try {
      const accessToken = sessionStorage.getItem('@access-token');
      if (accessToken) {
        this.setAccessToken(accessToken);
      } else {
        this.clearsessionStorage();
      }
    } catch {
      this.clearsessionStorage();
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
    const accessToken = this.accessTokenSubject.getValue() ?? sessionStorage.getItem('@access-token');
    if (accessToken === null || accessToken === undefined) {
      this.clearsessionStorage();
      return false;
    }
    return true;
  }

  createAccessToken(auth: IAuth): void {
    this.setAccessToken(auth.accessToken);
  }
}
