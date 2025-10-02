import { NgModule } from "@angular/core";
import { MatNativeDateModule } from "@angular/material/core";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatIconModule } from "@angular/material/icon";
import { DespesasFormComponent } from "./despesas-form/despesas.form.component";
import { DespesasComponent } from "./despesas.component";
import { DespesasRoutingModule } from "./despesas.routing.module";
import { SharedModule } from "../../app.shared.module";

@NgModule({
  declarations: [DespesasComponent, DespesasFormComponent ],
  imports: [SharedModule, DespesasRoutingModule, MatIconModule, MatNativeDateModule, MatDatepickerModule],
  providers: [DespesasFormComponent]
})

export class DespesasModule {}
