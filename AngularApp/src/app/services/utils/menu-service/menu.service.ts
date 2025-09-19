import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { CategoriaStrategy, ConfiguracoesStrategy, DespesaStrategy, HomeStrategy, InvalidStrategy, LancamentoStrategy, MenuStrategy, PerfilStrategy, ReceitaStrategy } from "./menu-strategy";
import { DashboardStrategy } from "./menu-strategy/dashboard/dashboard.strategy";

@Injectable({
  providedIn: 'root',
})

export class MenuService {
  menuSelecionado: number = 0;
  menu: string = '';
  private strategies: Map<number, MenuStrategy>;
  private invalidStrategy = new InvalidStrategy();

  constructor(private router: Router) {    
    this.strategies = new Map([
      [0, new HomeStrategy()],
      [1, new DashboardStrategy()],
      [2, new CategoriaStrategy()],
      [3, new DespesaStrategy()],
      [4, new ReceitaStrategy()],
      [5, new LancamentoStrategy()],
      [6, new PerfilStrategy()],
      [7, new ConfiguracoesStrategy()]
    ]);
  }

  setMenuSelecionado(menu: number): void {
    const strategy = this.strategies.get(menu) ?? this.invalidStrategy;

    if (strategy) {
      this.menu = strategy.setMenuName();
      this.menuSelecionado = menu;
      strategy.navigate(this.router);
    }
  }
}
