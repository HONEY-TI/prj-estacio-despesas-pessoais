import { TestBed } from '@angular/core/testing';
import { TokenStorageService } from '..';

describe('TokenStorageService', () => {
  let service: TokenStorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TokenStorageService);
    sessionStorage.clear();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should save and get access token', () => {
    service.saveToken('myAccessToken');
    expect(sessionStorage.getItem('@access-token')).toBe('myAccessToken');
    expect(service.getAccessToken()).toBe('myAccessToken');
  });

  it('should update access token', () => {
    service.saveToken('oldToken');
    service.updateAccessToken('newToken');
    expect(sessionStorage.getItem('@access-token')).toBe('newToken');
    expect(service.getAccessToken()).toBe('newToken');
  });

  it('should save and get refresh token', () => {
    service.saveRefreshToken('myRefreshToken');
    expect(localStorage.getItem('@refresh-token')).toBe('myRefreshToken');
    expect(service.getRefreshToken()).toBe('myRefreshToken');
  });

  it('should revoke refresh token', () => {
    service.saveRefreshToken('tokenToRevoke');
    service.revokeRefreshToken();
    expect(localStorage.getItem('@refresh-token')).toBeNull();
    expect(service.getRefreshToken()).toBeNull();
  });

  it('should clear session storage', () => {
    sessionStorage.setItem('@access-token', 'test');
    service.clearSessionStorage();
    expect(sessionStorage.getItem('@access-token')).toBeNull();
  });

  it('should sign out and clear both storages', () => {
    sessionStorage.setItem('@access-token', 'testSession');
    localStorage.setItem('@refresh-token', 'testLocal');

    service.signOut();

    expect(sessionStorage.getItem('@access-token')).toBeNull();
    expect(localStorage.getItem('@refresh-token')).toBeNull();
  });
});
