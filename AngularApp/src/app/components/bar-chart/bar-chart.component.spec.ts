import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { BarChartComponent } from '..';

describe('BarChartComponent', () => {
  let component: BarChartComponent;
  let fixture: ComponentFixture<BarChartComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [BarChartComponent],
      schemas: [NO_ERRORS_SCHEMA],

    });
    fixture = TestBed.createComponent(BarChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have the correct chart type', () => {
    expect(component.barChartType).toBe('bar');
  });

  
  it('should call chartClicked with correct params', () => {
    const active = [{ index: 0, datasetIndex: 0, x: 0, y: 0 }];
    const spy = spyOn(component, 'chartClicked').and.callThrough();

    component.chartClicked({ active });

    expect(spy).toHaveBeenCalledWith({ active });
  });

  it('should call chartHovered with correct params', () => {
    const active = [{ index: 1, datasetIndex: 0, x: 10, y: 20 }];
    const spy = spyOn(component, 'chartHovered').and.callThrough();

    component.chartHovered({ active });

    expect(spy).toHaveBeenCalledWith({ active });
  });
});