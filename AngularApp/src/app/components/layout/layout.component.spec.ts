import { ComponentFixture, TestBed, fakeAsync, flush } from '@angular/core/testing';
import { LayoutComponent } from './layout.component';
import { CommonModule } from '@angular/common';
import { MenuService, AuthService, ImagemPerfilService } from '../../services';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';

describe('LayoutComponent Unit Test', () => {
  let component: LayoutComponent;
  let fixture: ComponentFixture<LayoutComponent>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockImagemPerfilService: jasmine.SpyObj<ImagemPerfilService>;
  let menuService: MenuService;
  let router: Router;

  beforeEach(() => {
    mockAuthService = jasmine.createSpyObj('AuthService', ['logout']);
    mockImagemPerfilService = jasmine.createSpyObj('ImagemPerfilService', ['getImagemPerfilUsuario']);
    mockImagemPerfilService.getImagemPerfilUsuario.and.returnValue(of(new ArrayBuffer(8)));

    TestBed.configureTestingModule({
      declarations: [LayoutComponent],
      imports: [CommonModule, RouterTestingModule],
      providers: [
        MenuService,
        { provide: AuthService, useValue: mockAuthService },
        { provide: ImagemPerfilService, useValue: mockImagemPerfilService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LayoutComponent);
    component = fixture.componentInstance;
    menuService = TestBed.inject(MenuService);
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should call initialize on ngOnInit', () => {
    const spyInitialize = spyOn(component, 'initialize');
    component.ngOnInit();
    expect(spyInitialize).toHaveBeenCalled();
  });

  it('should set profile image from service', fakeAsync(() => {
    component.initialize();
    flush();
    expect(mockImagemPerfilService.getImagemPerfilUsuario).toHaveBeenCalled();
    expect(component.urlPerfilImage).toContain('blob:');
  }));

  it('should set default profile image on error', fakeAsync(() => {
    mockImagemPerfilService.getImagemPerfilUsuario.and.returnValue(throwError(() => 'Erro'));
    component.initialize();
    flush();
    expect(component.urlPerfilImage).toBe('../../../../assets/perfil_static.png');
  }));

  it('should call menuService.selectMenu with correct menu numbers', () => {
    spyOn(menuService, 'selectMenu');

    for (let menu = 1; menu <= 7; menu++) {
      component.selectMenu(menu);
      expect(menuService.selectMenu).toHaveBeenCalledWith(menu, router);
    }
  });

  it('should call authService.logout and navigate to / on logout', () => {
    spyOn(router, 'navigate');
    component.onLogoutClick();
    expect(mockAuthService.logout).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });
});
