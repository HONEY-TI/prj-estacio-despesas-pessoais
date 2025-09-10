import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatSelectModule } from "@angular/material/select";
import { CurrencyMaskModule } from "ng2-currency-mask";
import { LayoutComponent, BarraFerramentaComponent, DataTableComponent } from "./components";
import { BarraFerramentaModule } from "./components/barra-ferramenta-component/barra-ferramenta.component.module";
import { DataTableModule } from "./components/data-table/data-table.component.module";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";

@NgModule({
  declarations: [LayoutComponent],
  imports: [CommonModule, ReactiveFormsModule, FormsModule, BarraFerramentaModule, CurrencyMaskModule, DataTableModule ],
  exports: [LayoutComponent, BarraFerramentaComponent, DataTableModule, CurrencyMaskModule, MatSelectModule, MatFormFieldModule, MatInputModule ],
  providers: [DataTableComponent]
})

export class SharedModule { }
