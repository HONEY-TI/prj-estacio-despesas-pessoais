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
    service.saveAccessToken('access123');
    const token = service.getAccessToken();
    expect(token).toBe('access123');
    expect(sessionStorage.getItem('@access-token')).toBe('access123');
  });

  it('should overwrite access token', () => {
    service.saveAccessToken('oldToken');
    service.saveAccessToken('newToken');
    const token = service.getAccessToken();
    expect(token).toBe('newToken');
    expect(sessionStorage.getItem('@access-token')).toBe('newToken');
  });

  it('should save and get refresh token', () => {
    service.saveRefreshToken('refresh123');
    const token = service.getRefreshToken();
    expect(token).toBe('refresh123');
    expect(localStorage.getItem('@refresh-token')).toBe('refresh123');
  });

  it('should clear session and remove refresh token', () => {
    service.saveAccessToken('access123');
    service.saveRefreshToken('refresh123');
    service.clear();
    expect(service.getAccessToken()).toBeNull();
    expect(service.getRefreshToken()).toBeNull();
    expect(sessionStorage.getItem('@access-token')).toBeNull();
    expect(localStorage.getItem('@refresh-token')).toBeNull();
  });

  it('should sign out (clear session storage only)', () => {
    service.saveAccessToken('access123');
    service.saveRefreshToken('refresh123');
    service.signOut();
    expect(service.getAccessToken()).toBeNull();
    expect(service.getRefreshToken()).toBe('refresh123'); // localStorage não é limpo pelo signOut
  });
});
