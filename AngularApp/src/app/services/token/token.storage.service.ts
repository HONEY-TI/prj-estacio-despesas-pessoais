import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

const ACCESS_TOKEN_KEY = '@access-token';
const REFRESH_TOKEN_KEY = '@refresh-token';

@Injectable({
  providedIn: 'root'
})

export class TokenStorageService {

  constructor(private router: Router) { }

  public signOut(): void {
    sessionStorage.clear();
    this.router.navigate(["/"]);
  }

  public saveToken(accessToken: string): void {
    sessionStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
  }

  public updateAccessToken(accessToken: string): void {
    sessionStorage.removeItem(ACCESS_TOKEN_KEY);
    sessionStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
  }

  public getAccessToken(): string | null {
    return sessionStorage.getItem(ACCESS_TOKEN_KEY);
  }

  public saveRefreshToken(refreshToken: string): void {
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
  }

  public getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  public revokeRefreshToken(): void {
    localStorage.removeItem(REFRESH_TOKEN_KEY);
  }
}

