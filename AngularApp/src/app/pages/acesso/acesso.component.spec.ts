import { HttpClientTestingModule } from "@angular/common/http/testing";
import { ComponentFixture, TestBed, fakeAsync, flush } from "@angular/core/testing";
import { ReactiveFormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { RouterTestingModule } from "@angular/router/testing";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { of, throwError } from "rxjs";
import { AcessoComponent } from "./acesso.component";
import { AlertComponent, AlertType } from "../../components";
import { IAcesso } from "../../models";

describe('AcessoComponent', () => {
  let component: AcessoComponent;
  let fixture: ComponentFixture<AcessoComponent>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(() => {
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    TestBed.configureTestingModule({
      imports: [AcessoComponent, ReactiveFormsModule,  RouterTestingModule, HttpClientTestingModule ],
      providers: [AlertComponent, NgbActiveModal,
        { provide: Router, useValue: mockRouter }
      ]
    });
    fixture = TestBed.createComponent(AcessoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });


  it('should create', () => {
    // Assert
    expect(component).toBeTruthy();
  });

  it('onSaveClick should open a modal with success message', fakeAsync(() => {
    // Arrange
    const controleAcesso: IAcesso = {
      nome: 'Teste Usuário',
      sobreNome: 'Usuário',
      telefone: '(21) 9999-9999',
      email: 'teste@teste.com',
      senha: '!12345',
      confirmaSenha: '!12345'
    };
    const response = { message: true };

    spyOn(component.modalALert, 'open').and.callThrough();
    spyOn(component.acessoService, 'createUsuario').and.returnValue(of(response));
    spyOn(component, 'onSaveClick').and.callThrough();

    // Act
    component.createAccountFrom.patchValue(controleAcesso);
    component.onSaveClick();

    // Assert
    expect(component.acessoService.createUsuario).toHaveBeenCalledWith(controleAcesso);
    expect(component.onSaveClick).toHaveBeenCalled();
    expect(component.modalALert.open).toHaveBeenCalled();
    //expect(component.router.navigate).toHaveBeenCalledWith(['/dashboard']);
  }));

  it('should open modal when when error comes from api ', () => {
    // Arrange
    const controleAcesso: IAcesso = {
      nome: 'Teste Usuário',
      sobreNome: 'Usuário',
      telefone: '(21) 9999-9999',
      email: 'teste@teste.com',
      senha: '!12345',
      confirmaSenha: '!12345'
    };

    const response = {  message: "Teste Erro Message From API" };
    spyOn(component.modalALert, 'open').and.callThrough();
    spyOn(component.acessoService, 'createUsuario').and.returnValue(of(response));
    spyOn(component, 'onSaveClick').and.callThrough();

    // Act
    component.createAccountFrom.patchValue(controleAcesso);
    component.onSaveClick();

    // Asssert
    expect(component.modalALert.open).toHaveBeenCalled();
  });

  it('should open modal when when throws error ', () => {
    // Arrange
    const controleAcesso: IAcesso = {
      nome: 'Teste Usuário',
      sobreNome: 'Usuário',
      telefone: '(21) 9999-9999',
      email: 'teste@teste.com',
      senha: '!12345',
      confirmaSenha: '!12345'
    };

    const response = {  message: "Teste Throws Error" };
    spyOn(component.modalALert, 'open').and.callThrough();
    spyOn(component.acessoService, 'createUsuario').and.returnValue(of(response)).and.throwError;
    spyOn(component, 'onSaveClick').and.callThrough();

    // Act
    component.createAccountFrom.patchValue(controleAcesso);
    component.onSaveClick();

    // Asssert
    expect(component.modalALert.open).toHaveBeenCalled();
  });


  it('should toggle senha visibility and update eye icon class', () => {
    // Arrange
    component.ngOnInit();
    component.showSenha = false;
    component.eyeIconClass = 'bi-eye';

    // Act
    component.onToogleSenha();

    // Assert
    expect(component.showSenha).toBe(true);
    expect(component.eyeIconClass).toBe('bi-eye-slash');

    // Act
    component.onToogleSenha();

    // Assert
    expect(component.showSenha).toBe(false);
    expect(component.eyeIconClass).toBe('bi-eye');
  });

  it('should toggle confirma senha visibility and update eye icon class', () => {
    // Arrange
    component.ngOnInit();
    component.showConfirmaSenha = false;
    component.eyeIconClassConfirmaSenha = 'bi-eye';

    // Act
    component.onToogleConfirmaSenha();

    // Assert
    expect(component.showConfirmaSenha).toBe(true);
    expect(component.eyeIconClassConfirmaSenha).toBe('bi-eye-slash');

    // Act
    component.onToogleConfirmaSenha();

    // Assert
    expect(component.showConfirmaSenha).toBe(false);
    expect(component.eyeIconClassConfirmaSenha).toBe('bi-eye');
  });

  it('should open modal with validation error message when API returns 400 with validation errors', fakeAsync(() => {
    // Arrange
    const controleAcesso: IAcesso = {
      nome: 'Teste Usuário',
      sobreNome: 'Usuário',
      telefone: '(21) 9999-9999',
      email: 'teste@teste.com',
      senha: '!12345',
      confirmaSenha: '!12345'
    };
    const errorResponse = {
      status: 400,
      errors: {
        email: ['Email inválido']
      }
    };
    spyOn(component.modalALert, 'open').and.callThrough();
    spyOn(component.acessoService, 'createUsuario').and.returnValue(throwError(errorResponse));
    spyOn(component, 'onSaveClick').and.callThrough();

    // Act
    component.createAccountFrom.patchValue(controleAcesso);
    component.onSaveClick();
    flush();

    // Assert
    expect(component.modalALert.open).toHaveBeenCalledWith(AlertComponent, 'Email inválido', AlertType.Warning);
  }));
});