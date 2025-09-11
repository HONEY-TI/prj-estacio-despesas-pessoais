import { NgModule } from "@angular/core";
import { CategoriasFormComponent } from "./categorias-form/categorias.form.component";
import { CategoriasComponent } from "./categorias.component";
import { CategoriaRoutingModule } from "./categorias.routing.module";
import { CategoriaService } from "../../services";
import { SharedModule } from "../../app.shared.module";
import { MatIconModule } from "@angular/material/icon";

@NgModule({
  declarations: [CategoriasComponent, CategoriasFormComponent],
  imports: [SharedModule, CategoriaRoutingModule, MatIconModule ],
  providers: [CategoriaService]
})

export class CategoriasModule { }
