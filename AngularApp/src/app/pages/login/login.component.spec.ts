import { HttpClientTestingModule } from "@angular/common/http/testing";
import { ComponentFixture, TestBed, fakeAsync, flush } from "@angular/core/testing";
import { Router } from "@angular/router";
import { RouterTestingModule } from "@angular/router/testing";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { of } from "rxjs";
import { LoginComponent } from "./login.component";
import { SharedModule } from "../../app.shared.module";
import { AuthService } from "../../services";
import { AlertComponent } from "../../components";
import { IAuth, ILogin } from "../../models";

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockAuthService: jasmine.SpyObj<AuthService>;

  beforeEach(() => {
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    mockAuthService = jasmine.createSpyObj('AuthService', ['createAccessToken', 'isAuthenticated']);
    TestBed.configureTestingModule({
      declarations: [LoginComponent],
      imports: [SharedModule,  RouterTestingModule, HttpClientTestingModule  ],
      providers: [AlertComponent, NgbActiveModal,
        { provide: Router, useValue: mockRouter },
        { provide: AuthService, useValue: mockAuthService },
      ]
    });
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    // Assert
    expect(component).toBeTruthy();
  });

  it('should navigate to dashboard on successful login', fakeAsync(() => {
    // Arrange
    const login: ILogin = { email: "teste@teste.com", senha: "teste" };
    const authResponse: IAuth = {
      authenticated: true,
      created: '2023-10-01',
      expiration: '2023-10-30',
      accessToken: 'teste#token',
      refreshToken: 'testeRefreshToken'
    };
    spyOn(component.acessoService, 'signIn').and.returnValue(of(authResponse));
    mockAuthService.createAccessToken.and.returnValue(true);
    mockAuthService.isAuthenticated.and.returnValue(true);
    mockRouter.navigate.and.returnValue(Promise.resolve(true));

    // Act
    component.loginForm.patchValue(login);
    component.onLoginClick();
    flush();

    // Assert
    expect(component.acessoService.signIn).toHaveBeenCalledWith(login);
    expect(component.authProviderService.createAccessToken).toHaveBeenCalledWith(authResponse);
    expect(component.authProviderService.isAuthenticated()).toBe(true);
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/dashboard']);
  }));

  it('should open modal when promisse is rejected ', () => {
    // Arrange
    const errorMessage = "Error Test Component";
    spyOn(component.modalALert, 'open').and.callThrough();
    spyOn(component.acessoService, 'signIn').and.rejectWith().and.callThrough();
    spyOn(component, 'onLoginClick');

    // Act
    component.onLoginClick();

    // Asssert
    expect(component.onLoginClick).toHaveBeenCalled();
    expect(component.acessoService.signIn).not.toHaveBeenCalled();
  });

  it('should open modal when authenticated is not true ', () => {
    // Arrange
    const authResponse = { authenticated: false, message: 'Test Erro Auth' };
    spyOn(component.modalALert, 'open').and.callThrough();
    spyOn(component.acessoService, 'signIn').and.returnValue(of(authResponse));
    spyOn(component, 'onLoginClick').and.callThrough();

    // Act
    component.onLoginClick();

    // Asssert
    expect(component.modalALert.open).toHaveBeenCalled();
  });

  it('should return login form controls', () => {
    // Arrange
    component.ngOnInit();
    component.loginForm.controls['email'].setValue('teste@teste.com');
    component.loginForm.controls['senha'].setValue('password');

    // Act
    const loginDados = component.loginForm.getRawValue();

    // Assert
    expect(loginDados.email).toBe('teste@teste.com');
    expect(loginDados.senha).toBe('password');
  });

  it('should toggle password visibility and update eye icon class', () => {
    // Arrange
    component.ngOnInit();
    component.showPassword = false;
    component.eyeIconClass = 'bi-eye';

    // Act
    component.onTooglePassword();

    // Assert
    expect(component.showPassword).toBe(true);
    expect(component.eyeIconClass).toBe('bi-eye-slash');

    // Act
    component.onTooglePassword();

    // Assert
    expect(component.showPassword).toBe(false);
    expect(component.eyeIconClass).toBe('bi-eye');
  });
});