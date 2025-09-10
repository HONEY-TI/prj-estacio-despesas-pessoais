import { TestBed } from '@angular/core/testing';
import { AuthProvider } from './auth.provider';
import { TokenStorageService } from '../services';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';

describe('AuthProvider Unit Test', () => {
  let authProvider: AuthProvider;
  let tokenStorageSpy: jasmine.SpyObj<TokenStorageService>;
  let router: Router;

  beforeEach(() => {
    tokenStorageSpy = jasmine.createSpyObj('TokenStorageService', [
      'getAccessToken', 'saveToken', 'signOut'
    ]);

    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        AuthProvider,
        { provide: TokenStorageService, useValue: tokenStorageSpy },
      ]
    });

    authProvider = TestBed.inject(AuthProvider);
    router = TestBed.inject(Router);
  });

  it('should be created', () => {
    expect(authProvider).toBeTruthy();
  });

  it('should allow activation when user is authenticated', () => {
    // Arrange
    tokenStorageSpy.getAccessToken.and.returnValue('validToken');

    // Act
    const canActivate = authProvider.canActivate({} as any, {} as any);

    // Assert
    expect(tokenStorageSpy.getAccessToken).toHaveBeenCalled();
    expect(canActivate).toBeTrue();
  });

  it('should redirect to login page when user is not authenticated', () => {
    // Arrange
    tokenStorageSpy.getAccessToken.and.returnValue(null);
    
    // Act
    const canActivate = authProvider.canActivate({} as any, {} as any);

    // Assert
    expect(tokenStorageSpy.getAccessToken).toHaveBeenCalled();
    expect(tokenStorageSpy.signOut).toHaveBeenCalled();
    expect(canActivate).toBeFalse();
  });
});
