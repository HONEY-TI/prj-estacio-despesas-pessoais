import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { LocationStrategy, PathLocationStrategy } from '@angular/common';
import { AppRoutingModule } from './app.routing.module';
import { PageNotFoundComponent } from './pages/page-not-found/page-not-found.component';
import { PrivacyComponent } from './pages/privacy/privacy.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { AcessoComponent } from './pages/acesso/acesso.component';
import { AuthGuard } from './provider/auth.guard';
import { SharedModule } from './app.shared.module';

describe('AppRoutingModule (Lazy Load)', () => {
  let router: Router;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SharedModule, AppRoutingModule],
      providers: [AuthGuard],
    }).compileComponents();

    router = TestBed.inject(Router);
  });

  it('should configure LocationStrategy as PathLocationStrategy', () => {
    const locationStrategy = TestBed.inject(LocationStrategy);
    expect(locationStrategy instanceof PathLocationStrategy).toBeTrue();
  });

  it('should have correct lazy-loaded routes', () => {
    const lazyPaths = ['','categoria','despesa','receita','lancamento','perfil','configuracoes'];
    lazyPaths.forEach(path => {
      const route = router.config.find(r => r.path === path);
      expect(route).toBeTruthy();
      expect(route?.loadChildren).toBeTruthy();
      expect(typeof route?.loadChildren).toBe('function');
    });
  });

  it('should have correct direct component routes', () => {
    const directRoutes = [
      { path: 'register', component: AcessoComponent },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'privacy', component: PrivacyComponent },
      { path: '**', component: PageNotFoundComponent },
    ];

    directRoutes.forEach(r => {
      const route = router.config.find(rt => rt.path === r.path);
      expect(route).toBeTruthy();
      expect(route?.component).toBe(r.component);
    });
  });
});
