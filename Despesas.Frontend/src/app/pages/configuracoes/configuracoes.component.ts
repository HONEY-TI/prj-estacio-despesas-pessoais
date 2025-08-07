import { Component } from "@angular/core";
import { MenuService } from "../../shared/services";
@Component({
  selector: 'app-configuracoes',
  templateUrl: './configuracoes.component.html',
  styleUrls: ['./configuracoes.component.scss'],
  standalone: false
})

export class ConfiguracoesComponent  {

  constructor(private menuService: MenuService){
    this.menuService.setMenuSelecionado(7);
  }
}
