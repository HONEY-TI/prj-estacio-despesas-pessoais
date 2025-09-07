import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PageNotFoundComponent } from './pages/page-not-found/page-not-found.component';
import { AuthProvider } from './services';
import { AcessoComponent } from './pages/acesso/acesso.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { LocationStrategy, PathLocationStrategy } from '@angular/common';
import { PrivacyComponent } from './pages/privacy/privacy.component';

const routes: Routes = [
  { path: '', loadChildren: () => import('./pages/login/login.module').then(m => m.LoginModule), pathMatch: 'full' },
  { path: "register", component: AcessoComponent},
  { path: 'dashboard',  canActivate: [AuthProvider],  component: DashboardComponent,},
  { path: 'categoria', canActivate: [AuthProvider], loadChildren: () => import('./pages/categorias/categorias.module').then(m => m.CategoriasModule), },
  { path: 'despesa', canActivate: [AuthProvider], loadChildren: () => import('./pages/despesas/despesas.module').then(m => m.DespesasModule), },
  { path: 'receita', canActivate: [AuthProvider], loadChildren: () => import('./pages/receitas/receitas.module').then(m => m.ReceitasModule), },
  { path: 'lancamento', canActivate: [AuthProvider], loadChildren: () => import('./pages/lancamentos/lancamentos.module').then(m => m.LancamentosModule),},
  { path: 'perfil', canActivate: [AuthProvider], loadChildren: () => import('./pages/perfil/perfil.module').then(m => m.PerfilModule), },
  { path: 'configuracoes', canActivate: [AuthProvider], loadChildren: () => import('./pages/configuracoes/configuracoes.module').then(m => m.ConfiguracoesModule),},
  { path: 'privacy', component: PrivacyComponent },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [{ provide: LocationStrategy, useClass: PathLocationStrategy } ]
})


export class AppRoutingModule { }
