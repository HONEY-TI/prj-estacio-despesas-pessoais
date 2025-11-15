import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Dayjs } from 'dayjs';
import { AbstractService } from '../base/AbstractService';
import { Observable } from 'rxjs';
import { shareReplay } from 'rxjs/operators';
import { ISaldo } from '../../../models';

@Injectable({
  providedIn: 'root'
})

export class SaldoService extends AbstractService {
  private cacheAnual = new Map<string, Observable<ISaldo>>();
  private cacheMensal = new Map<string, Observable<ISaldo>>();

  constructor(public httpClient: HttpClient) {
    const ROUTE = 'Saldo';
    super(ROUTE);
  }

  public getSaldo(): Observable<ISaldo> {
    return this.httpClient.get<ISaldo>(`${this.routeUrl}`);
  }

  public getSaldoAnual(ano: Dayjs): Observable<ISaldo> {
    const key = ano.format('YYYY-MM');
    if (!this.cacheAnual.has(key)) {
      const req$ = this.httpClient
        .get<ISaldo>(`${this.routeUrl}/ByAno/${key}`)
        .pipe(shareReplay(1));
      this.cacheAnual.set(key, req$);
    }
    return this.cacheAnual.get(key)!;
  }

  public getSaldoByMesANo(mesAno: Dayjs): Observable<ISaldo> {
    const key = mesAno.format('YYYY-MM');
    if (!this.cacheMensal.has(key)) {
      const req$ = this.httpClient
        .get<ISaldo>(`${this.routeUrl}/ByMesAno/${key}`)
        .pipe(shareReplay(1));
      this.cacheMensal.set(key, req$);
    }
    return this.cacheMensal.get(key)!;
  }
}
