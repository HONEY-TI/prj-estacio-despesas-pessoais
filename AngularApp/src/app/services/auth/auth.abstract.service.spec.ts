import { TestBed, fakeAsync, flush } from '@angular/core/testing';
import { Router } from '@angular/router';
import { TokenStorageService } from '../token/token.storage.service';
import { IAuth } from '../../models';
import { AuthServiceBase } from './auth.abstract.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AcessoService } from '..';

describe('AuthServiceBase Unit Test', () => {
  let service: AuthServiceBase;
  let tokenStorage: jasmine.SpyObj<TokenStorageService>;
  let router: jasmine.SpyObj<Router>;
  let acessoService: jasmine.SpyObj<AcessoService>;

  class AuthServiceTest extends AuthServiceBase {
    constructor() {
      super(
        null as any,
        tokenStorage,
        router,
        acessoService
      );
    }
  }
  beforeEach(() => {
    // Arrange: Criar mocks para dependências
    tokenStorage = jasmine.createSpyObj('TokenStorageService', [
      'getAccessToken', 'getRefreshToken', 'saveAccessToken', 'saveRefreshToken', 'clear'
    ]);
    router = jasmine.createSpyObj('Router', ['navigate']);
    acessoService = jasmine.createSpyObj('AcessoService', ['refreshToken']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        { provide: AuthServiceBase, useClass: AuthServiceTest },
        { provide: TokenStorageService, useValue: tokenStorage },
        { provide: Router, useValue: router }
      ]
    });

    // Act: Injetar serviço
    service = TestBed.inject(AuthServiceBase);
  });

  it('should create the service', () => {
    // Assert
    expect(service).toBeTruthy();
  });

  it('should return true if user is authenticated', () => {
    // Arrange
    tokenStorage.getAccessToken.and.returnValue('token');

    // Act
    const result = service.isAuthenticated();

    // Assert
    expect(result).toBeTrue();
  });

  it('should return false if not authenticated', () => {
    // Arrange
    tokenStorage.getAccessToken.and.returnValue(null);
    service['accessTokenSubject'].next(undefined);

    // Act
    const result = service.isAuthenticated();

    // Assert
    expect(result).toBeFalse();
  });

  it('should logout correctly', () => {
    // Arrange
    service['accessTokenSubject'].next('token');
    service['isAuthenticated$'].next(true);

    // Act
    service.logout();

    // Assert
    expect(service['accessTokenSubject'].getValue()).toBeUndefined();
    service.isAuthenticated$.subscribe(auth => expect(auth).toBeFalse());
    expect(tokenStorage.clear).toHaveBeenCalled();
  });

  it('should login and save tokens', () => {
    // Arrange
    const auth: IAuth = { accessToken: 'a', refreshToken: 'b', authenticated: true, created: '', expiration: '' };

    // Act
    service.login(auth);

    // Assert
    expect(tokenStorage.saveAccessToken).toHaveBeenCalledWith('a');
    expect(tokenStorage.saveRefreshToken).toHaveBeenCalledWith('b');
    expect(service['accessTokenSubject'].getValue()).toBe('a');
    service.isAuthenticated$.subscribe(authenticated => expect(authenticated).toBeTrue());
  });

  it('should logout if no refresh token during autoLogin', fakeAsync(async () => {
    // Arrange
    tokenStorage.getRefreshToken.and.returnValue(null);
    const logoutSpy = spyOn(service, 'logout').and.callThrough();

    // Act
    await service.autoLogin();
    flush();

    // Assert
    expect(logoutSpy).toHaveBeenCalled();
  }));  
});
