import { HttpClientTestingModule } from "@angular/common/http/testing";
import { ComponentFixture, TestBed, fakeAsync, flush } from "@angular/core/testing";
import { Router } from "@angular/router";
import { of, throwError } from "rxjs";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { Platform } from '@ionic/angular';

import { LoginComponent } from "./login.component";
import { AlertComponent, AlertType } from "../../components";
import { IAuth, ILogin } from "../../models";
import { AuthService, AcessoService, AuthGoogleService } from "../../services";

describe('LoginComponent Unit Tests', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockAcessoService: jasmine.SpyObj<AcessoService>;
  let mockGoogleService: jasmine.SpyObj<AuthGoogleService>;
  let mockAlert: jasmine.SpyObj<AlertComponent>;

  beforeEach(() => {
    // Arrange: criar mocks
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    mockAuthService = jasmine.createSpyObj('AuthService', ['login', 'isAuthenticated']);
    mockAcessoService = jasmine.createSpyObj('AcessoService', ['signIn']);
    mockGoogleService = jasmine.createSpyObj('AuthGoogleService', ['handleGoogleLogin']);
    mockAlert = jasmine.createSpyObj('AlertComponent', ['open']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, ReactiveFormsModule],
      declarations: [LoginComponent],
      providers: [
        FormBuilder,
        Platform,
        { provide: Router, useValue: mockRouter },
        { provide: AuthService, useValue: mockAuthService },
        { provide: AcessoService, useValue: mockAcessoService },
        { provide: AuthGoogleService, useValue: mockGoogleService },
        { provide: AlertComponent, useValue: mockAlert }
      ]
    });

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component', () => {
    // Assert
    expect(component).toBeTruthy();
  });

  it('should initialize login form with default values', () => {
    // Act
    component.ngOnInit();
    const formValues = component.loginForm.getRawValue();

    // Assert
    expect(formValues.email).toBe('user@example.com');
    expect(formValues.senha).toBe('12345T!');
  });

  it('should login successfully and navigate to dashboard', fakeAsync(() => {
    // Arrange
    const login: ILogin = { email: 'user@example.com', senha: '12345T!' };
    const authResponse: IAuth = { authenticated: true, accessToken: 'token', refreshToken: 'refresh', created: '', expiration: '' };
    mockAcessoService.signIn.and.returnValue(of(authResponse));

    // Act
    component.loginForm.patchValue(login);
    component.onLoginClick();
    flush();

    // Assert
    expect(mockAcessoService.signIn).toHaveBeenCalledWith(login);
    expect(mockAuthService.login).toHaveBeenCalledWith(authResponse);
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/dashboard']);
  }));

  it('should show modal when login response is not authenticated', fakeAsync(() => {
    // Arrange
    const login: ILogin = { email: 'user@example.com', senha: '12345T!' };
    const authResponse = 'Erro de autenticação';
    mockAcessoService.signIn.and.returnValue(of(authResponse));

    // Act
    component.loginForm.patchValue(login);
    component.onLoginClick();
    flush();

    // Assert
    expect(mockAlert.open).toHaveBeenCalledWith(AlertComponent, authResponse, AlertType.Warning);
  }));

  it('should show modal when login throws an error', fakeAsync(() => {
    // Arrange
    const login: ILogin = { email: 'user@example.com', senha: '12345T!' };
    const error = { error: 'Erro inesperado' };
    mockAcessoService.signIn.and.returnValue(throwError(() => error));

    // Act
    component.loginForm.patchValue(login);
    component.onLoginClick();
    flush();

    // Assert
    expect(mockAlert.open).toHaveBeenCalledWith(AlertComponent, 'Erro inesperado', AlertType.Warning);
  }));

  it('should toggle password visibility and eye icon', () => {
    // Arrange
    component.showPassword = false;
    component.eyeIconClass = 'bi-eye';

    // Act
    component.onTooglePassword();

    // Assert
    expect(component.showPassword).toBeTrue();
    expect(component.eyeIconClass).toBe('bi-eye-slash');

    // Act
    component.onTooglePassword();

    // Assert
    expect(component.showPassword).toBeFalse();
    expect(component.eyeIconClass).toBe('bi-eye');
  });

  it('should handle Google login success', fakeAsync(() => {
    // Arrange
    const authResponse: IAuth = { authenticated: true, accessToken: 'token', refreshToken: 'refresh', created: '', expiration: '' };
    mockGoogleService.handleGoogleLogin.and.returnValue(of(authResponse));

    // Act
    component.onGoogleLoginClick();
    flush();

    // Assert
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/dashboard']);
  }));

  it('should handle Google login error', fakeAsync(() => {
    // Arrange
    const errorMessage = 'Erro Google';
    mockGoogleService.handleGoogleLogin.and.returnValue(throwError(() => errorMessage));

    // Act
    component.onGoogleLoginClick();
    flush();

    // Assert
    expect(mockAlert.open).toHaveBeenCalledWith(AlertComponent, errorMessage, AlertType.Warning);
  }));
});
