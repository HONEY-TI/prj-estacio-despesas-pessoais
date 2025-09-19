import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { CategoriaStrategy, ConfiguracoesStrategy, DespesaStrategy, HomeStrategy, LancamentoStrategy, MenuStrategy, PerfilStrategy, ReceitaStrategy } from "./menu-strategy";

@Injectable({
  providedIn: 'root',
})

export default class MenuService {
  menuSelecionado: number = 0;
  menu: string = '';
  private strategies: Map<number, MenuStrategy>;

  constructor(private router: Router) {    
    this.strategies = new Map([
      [0, new HomeStrategy()],
      [1, new HomeStrategy()],
      [2, new CategoriaStrategy()],
      [3, new DespesaStrategy()],
      [4, new ReceitaStrategy()],
      [5, new LancamentoStrategy()],
      [6, new PerfilStrategy()],
      [7, new ConfiguracoesStrategy()],
    ]);
  }

  selectMenu(menu: number, router: Router): void {
    const strategy = this.strategies.get(menu);

    if (strategy) {
      this.menu = strategy.setMenuName();
      this.menuSelecionado = menu;  
    } else {
      this.router.navigate(['/dashboard']);
    }
  }

  setMenuSelecionado(menu: number): void {
    const strategy = this.strategies.get(menu);

    if (strategy) {
      this.menu = strategy.setMenuName();
      this.menuSelecionado = menu;
    }
  }
}
