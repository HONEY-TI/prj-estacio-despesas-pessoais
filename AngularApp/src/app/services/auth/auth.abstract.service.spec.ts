import { TestBed, fakeAsync, flush } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { TokenStorageService } from '../token/token.storage.service';
import { IAuth } from '../../models';
import { AuthServiceBase } from './auth.abstract.service';

describe('AuthServiceBase Unit Test', () => {
  let service: AuthServiceBase;
  let tokenStorage: jasmine.SpyObj<TokenStorageService>;
  let router: jasmine.SpyObj<Router>;
  
  class AuthServiceTest extends AuthServiceBase { }

  beforeEach(() => {
    const tokenSpy = jasmine.createSpyObj('TokenStorageService', [
      'getAccessToken', 'getRefreshToken', 'saveToken', 'saveRefreshToken', 'clearSessionStorage'
    ]);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        { provide: TokenStorageService, useValue: tokenSpy },
        { provide: Router, useValue: routerSpy },
        { provide: AuthServiceBase, useClass: AuthServiceTest }
      ]
    });

    service = TestBed.inject(AuthServiceBase);
    tokenStorage = TestBed.inject(TokenStorageService) as jasmine.SpyObj<TokenStorageService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should create access token successfully', () => {
    const auth: IAuth = { accessToken: 'token', refreshToken: 'refresh', authenticated: true, created: '', expiration: '' };
    const result = service.createAccessToken(auth);

    expect(result).toBeTrue();
    expect(tokenStorage.saveToken).toHaveBeenCalledWith('token');
    expect(tokenStorage.saveRefreshToken).toHaveBeenCalledWith('refresh');
    service.isAuthenticated$.subscribe(authenticated => {
      expect(authenticated).toBeTrue();
    });
  });

  it('should fail creating access token if exception occurs', () => {
    tokenStorage.saveToken.and.throwError('Error');
    const auth: IAuth = { accessToken: 'token', refreshToken: 'refresh', authenticated: true, created: '', expiration: '' };
    const result = service.createAccessToken(auth);

    expect(result).toBeFalse();
  });

  it('should return true if user is authenticated', () => {
    tokenStorage.getAccessToken.and.returnValue('token');
    const result = service.isAuthenticated();

    expect(result).toBeTrue();
  });

  it('should return false and clear session if not authenticated', () => {
    tokenStorage.getAccessToken.and.returnValue(null);
    service['accessTokenSubject'].next(undefined);

    const result = service.isAuthenticated();
    expect(result).toBeFalse();
    expect(tokenStorage.clearSessionStorage).toHaveBeenCalled();
  });

  it('should logout correctly', () => {
    service['accessTokenSubject'].next('token');
    service['isAuthenticated$'].next(true);

    service['logout']();

    expect(service['accessTokenSubject'].getValue()).toBeUndefined();
    service.isAuthenticated$.subscribe(authenticated => {
      expect(authenticated).toBeFalse();
    });
    expect(tokenStorage.clearSessionStorage).toHaveBeenCalled();
  });

  it('should refresh token', () => {
    tokenStorage.getRefreshToken.and.returnValue('refreshToken');
    const result = service.refreshToken('refreshToken');
    expect(result).toBe('refreshToken');
    expect(tokenStorage.saveRefreshToken).toHaveBeenCalledWith('refreshToken');
  });

  it('should autoLogin and navigate to dashboard if refresh token exists', fakeAsync(async () => {
    tokenStorage.getRefreshToken.and.returnValue('refreshToken');
    spyOn(service, 'refreshToken').and.returnValue(of({
      accessToken: 'newAccessToken',
      refreshToken: 'newRefreshToken',
      authenticated: true,
      created: '',
      expiration: ''
    } as IAuth));

    await service.autoLogin();
    flush();

    expect(service['refreshToken']).toHaveBeenCalledWith('refreshToken');
    expect(tokenStorage.saveToken).toHaveBeenCalledWith('newAccessToken');
    expect(tokenStorage.saveRefreshToken).toHaveBeenCalledWith('newRefreshToken');
    expect(service['accessTokenSubject'].getValue()).toBe('newAccessToken');

    service.isAuthenticated$.subscribe(authenticated => {
      expect(authenticated).toBeTrue();
    });

  expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
}));

  it('should logout if no refresh token during autoLogin', fakeAsync(async () => {
    tokenStorage.getRefreshToken.and.returnValue(null);
    const logoutSpy = spyOn<any>(service, 'logout').and.callThrough();

    await service.autoLogin();
    flush();

    expect(logoutSpy).toHaveBeenCalled();
  }));

  it('should logout if autoLogin fails', fakeAsync(async () => {
    tokenStorage.getRefreshToken.and.returnValue('refreshToken');
    spyOn(service, 'refreshToken').and.returnValue(throwError(() => new Error('fail')));
    const logoutSpy = spyOn<any>(service, 'logout').and.callThrough();

    await service.autoLogin();
    flush();

    expect(logoutSpy).toHaveBeenCalled();
  }));
});
