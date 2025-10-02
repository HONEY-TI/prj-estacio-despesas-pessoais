import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed, fakeAsync, flush } from "@angular/core/testing";
import { ReactiveFormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { of, throwError } from "rxjs";
import { AcessoComponent } from "./acesso.component";
import { AlertComponent, AlertType } from "../../components";
import { IAcesso } from "../../models";
import { AcessoService } from "../../services";

describe('AcessoComponent', () => {
  let component: AcessoComponent;
  let fixture: ComponentFixture<AcessoComponent>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(() => {
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        ReactiveFormsModule,
        AcessoComponent
      ],
      providers: [
        AcessoService,
        AlertComponent,
        NgbActiveModal,
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
      email: 'user@example.com',
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
  }));

  it('should open modal when API returns an error', fakeAsync(() => {
    // Arrange
    const controleAcesso: IAcesso = {
      nome: 'Teste Usuário',
      sobreNome: 'Usuário',
      telefone: '(21) 9999-9999',
      email: 'user@example.com',
      senha: '!12345',
      confirmaSenha: '!12345'
    };

    const errorResponse = { message: "Teste Erro Message From API" };
    spyOn(component.modalALert, 'open').and.callThrough();
    spyOn(component.acessoService, 'createUsuario').and.returnValue(throwError(() => errorResponse));
    spyOn(component, 'onSaveClick').and.callThrough();

    // Act
    component.createAccountFrom.patchValue(controleAcesso);
    component.onSaveClick();
    flush();

    // Assert
    expect(component.modalALert.open).toHaveBeenCalled();
  }));

  it('should toggle senha visibility', () => {
    // Arrange
    component.showSenha = false;
    component.eyeIconClass = 'bi-eye';

    // Act
    component.onToogleSenha();

    // Assert
    expect(component.showSenha).toBeTrue();
    expect(component.eyeIconClass).toBe('bi-eye-slash');

    // Act
    component.onToogleSenha();

    // Assert
    expect(component.showSenha).toBeFalse();
    expect(component.eyeIconClass).toBe('bi-eye');
  });

  it('should toggle confirma senha visibility', () => {
    // Arrange
    component.showConfirmaSenha = false;
    component.eyeIconClassConfirmaSenha = 'bi-eye';

    // Act
    component.onToogleConfirmaSenha();

    // Assert
    expect(component.showConfirmaSenha).toBeTrue();
    expect(component.eyeIconClassConfirmaSenha).toBe('bi-eye-slash');

    // Act
    component.onToogleConfirmaSenha();

    // Assert
    expect(component.showConfirmaSenha).toBeFalse();
    expect(component.eyeIconClassConfirmaSenha).toBe('bi-eye');
  });

  it('should show validation errors from API', fakeAsync(() => {
    // Arrange
    const controleAcesso: IAcesso = {
      nome: 'Teste Usuário',
      sobreNome: 'Usuário',
      telefone: '(21) 9999-9999',
      email: 'user@example.com',
      senha: '!12345',
      confirmaSenha: '!12345'
    };
    const errorResponse = {
      status: 400,
      errors: { email: ['Email inválido'] }
    };

    spyOn(component.modalALert, 'open').and.callThrough();
    spyOn(component.acessoService, 'createUsuario').and.returnValue(throwError(() => errorResponse));
    spyOn(component, 'onSaveClick').and.callThrough();

    // Act
    component.createAccountFrom.patchValue(controleAcesso);
    component.onSaveClick();
    flush();

    // Assert
    expect(component.modalALert.open)
      .toHaveBeenCalledWith(AlertComponent, 'Email inválido', AlertType.Warning);
  }));
});
