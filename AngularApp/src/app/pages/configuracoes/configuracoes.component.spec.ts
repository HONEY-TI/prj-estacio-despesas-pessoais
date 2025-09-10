import { ComponentFixture, TestBed } from "@angular/core/testing";
import { RouterTestingModule } from "@angular/router/testing";
import { ConfiguracoesComponent } from "./configuracoes.component";
import { SharedModule } from "../../app.shared.module";
import { MenuService } from "../../services";

describe('Unit Test ConfiguracoesComponent', () => {
  let component: ConfiguracoesComponent;
  let fixture: ComponentFixture<ConfiguracoesComponent>;
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ SharedModule, RouterTestingModule],
      providers: [MenuService]
    });
    fixture = TestBed.createComponent(ConfiguracoesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    // Assert
    expect(component).toBeTruthy();
  });
});