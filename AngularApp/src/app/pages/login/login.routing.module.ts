import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login.component';
import { AuthProvider } from '../../shared/services';

const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'dashboard',  canActivate: [AuthProvider], loadChildren: () => import('../../pages/dashboard/dashboard.module').then(m => m.DashboardModule), },

];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class LoginRoutingModule { }


