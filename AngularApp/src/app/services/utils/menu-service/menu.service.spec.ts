import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { MenuService } from '../..';

describe('MenuService', () => {
  let menuService: MenuService;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    router = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        MenuService,
        { provide: Router, useValue: router }
      ]
    });

    menuService = TestBed.inject(MenuService);
  });

  it('should be created', () => {
    expect(menuService).toBeTruthy();
  });

  it('should navigate to home for menu 0', () => {
    // Arrange
    const menu = 0;

    // Act
    menuService.setMenuSelecionado(menu);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Home');
    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('should navigate to dashboard for menu 1', () => {
    // Arrange
    const menu = 1;

    // Act
    menuService.setMenuSelecionado(menu);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Dashboard');
    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('should navigate to categoria for menu 2', () => {
    // Arrange
    const menu = 2;

    // Act
    menuService.setMenuSelecionado(menu);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Categorias');
    expect(router.navigate).toHaveBeenCalledWith(['/categoria']);
  });

  it('should navigate to receita for menu 3', () => {
    // Arrange
    const menu = 3;

    // Act
    menuService.setMenuSelecionado(menu);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Despesas');
    expect(router.navigate).toHaveBeenCalledWith(['/despesa']);
  });

  it('should navigate to receita for menu 4', () => {
    // Arrange
    const menu = 4;

    // Act
    menuService.setMenuSelecionado(menu);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Receitas');
    expect(router.navigate).toHaveBeenCalledWith(['/receita']);
  });

  it('should navigate to lancamento for menu 5', () => {
    // Arrange
    const menu = 5;

    // Act
    menuService.setMenuSelecionado(menu);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Lançamentos');
    expect(router.navigate).toHaveBeenCalledWith(['/lancamento']);
  });

  it('should navigate to perfil for menu 6', () => {
    // Arrange
    const menu = 6;

    // Act
    menuService.setMenuSelecionado(menu);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Perfil');
    expect(router.navigate).toHaveBeenCalledWith(['/perfil']);
  });

  it('should navigate to configuracoes for menu 7', () => {
    // Arrange
    const menu = 7;

    // Act
    menuService.setMenuSelecionado(menu);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Configurações');
    expect(router.navigate).toHaveBeenCalledWith(['/configuracoes']);
  });

  it('should navigate to dashboard when menu is invalid', () => {
    // Arrange
    const menu = 999;

    // Act
    menuService.setMenuSelecionado(menu);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });
});
