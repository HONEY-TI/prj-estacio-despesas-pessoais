import { NgModule } from "@angular/core";
import { MatNativeDateModule } from "@angular/material/core";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { ReceitasFormComponent } from "./receitas-form/receitas.form.component";
import { ReceitasComponent } from "./receitas.component";
import { ReceitasRoutingModule } from "./receitas.routing.module";
import { MatIconModule } from "@angular/material/icon";
import { SharedModule } from "../../app.shared.module";

@NgModule({
  declarations: [ReceitasComponent, ReceitasFormComponent ],
  imports: [SharedModule, ReceitasRoutingModule, MatIconModule, MatNativeDateModule, MatDatepickerModule],
  providers: [ReceitasFormComponent]
})
export class ReceitasModule {}
