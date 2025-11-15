import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { BarChartComponent } from "../../components";
import { DashboardService } from "../../services";
import { SharedModule } from "../../app.shared.module";
import { DashboardComponent } from "./dashboard.component";
import { DashboardRoutingModule } from "./dashboard.routing.module";

@NgModule({
  declarations: [DashboardComponent ],
  imports: [CommonModule, DashboardRoutingModule, SharedModule, BarChartComponent], 
  providers: [DashboardService ]
})

export class DashboardModule {}
