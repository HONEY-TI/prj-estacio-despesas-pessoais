import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })

export class TokenStorageService {
  private readonly ACCESS_TOKEN_KEY = '@access-token';
  private readonly REFRESH_TOKEN_KEY = '@refresh-token';

  signOut(): void {
    sessionStorage.clear();
  }

  saveAccessToken(token: string): void {
    sessionStorage.setItem(this.ACCESS_TOKEN_KEY, token);
  }

  getAccessToken(): string | null {
    return sessionStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  saveRefreshToken(token: string): void {
    localStorage.setItem(this.REFRESH_TOKEN_KEY, token);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  clear(): void {
    sessionStorage.clear();
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
  }
}
