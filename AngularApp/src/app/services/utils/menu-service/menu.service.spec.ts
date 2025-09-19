import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import MenuService from './menu.service';

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
    menuService.selectMenu(menu, router);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Home');
    expect(router.navigate).toHaveBeenCalledWith(['/home']);
  });

  it('should navigate to categoria for menu 2', () => {
    // Arrange
    const menu = 2;

    // Act
    menuService.selectMenu(menu, router);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Categorias');
    expect(router.navigate).toHaveBeenCalledWith(['/categorias']);
  });

  it('should navigate to receita for menu 4', () => {
    // Arrange
    const menu = 4;

    // Act
    menuService.selectMenu(menu, router);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Receitas');
    expect(router.navigate).toHaveBeenCalledWith(['/receitas']);
  });

  it('should navigate to lancamento for menu 5', () => {
    // Arrange
    const menu = 5;

    // Act
    menuService.selectMenu(menu, router);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Lançamentos');
    expect(router.navigate).toHaveBeenCalledWith(['/lancamentos']);
  });

  it('should navigate to perfil for menu 6', () => {
    // Arrange
    const menu = 6;

    // Act
    menuService.selectMenu(menu, router);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Perfil');
    expect(router.navigate).toHaveBeenCalledWith(['/perfil']);
  });

  it('should navigate to configuracoes for menu 7', () => {
    // Arrange
    const menu = 7;

    // Act
    menuService.selectMenu(menu, router);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(menuService.menu).toBe('Configurações');
    expect(router.navigate).toHaveBeenCalledWith(['/configuracoes']);
  });

  it('should navigate to dashboard when menu is invalid', () => {
    // Arrange
    const menu = 999;

    // Act
    menuService.selectMenu(menu, router);

    // Assert
    expect(menuService.menuSelecionado).toBe(menu);
    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });
});
