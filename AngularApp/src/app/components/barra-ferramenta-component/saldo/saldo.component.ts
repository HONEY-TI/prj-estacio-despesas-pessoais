import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import dayjs, { Dayjs } from "dayjs";
import { ISaldo } from "../../../models";
import { FilterMesService } from "../../../services";
import { SaldoService } from "../../../services/api";

dayjs.locale('pt-br');

@Component({
  selector: 'app-saldo',
  standalone: true,
  templateUrl: './saldo.component.html',
  styleUrls: ['./saldo.component.scss'],
  imports: [CommonModule]
})

export class SaldoComponent implements OnInit {
  saldoAnual: number | string;
  saldoMensal: number | string;
  saldoAnualNegativo: string = 'text-danger';
  saldoMensalNegativo: string = 'text-danger';

  constructor(private saldoService: SaldoService, public filterMesService: FilterMesService) { }

  private initialize = (): void => {
    this.saldoService.getSaldoAnual(dayjs(dayjs().format('YYYY-01-01')))
      .subscribe({
        next: (response: ISaldo) => {
          if (response && response !== undefined && response !== null) {
            this.saldoAnualNegativo = this.isSaldoNegativo(response.saldo) ? 'text-danger' : '';
            this.saldoAnual = response.saldo.toLocaleString('pt-br', {
              style: 'currency',
              currency: 'BRL',
              minimumFractionDigits: 2,
              maximumFractionDigits: 2
            });
          }
        },
        error: () => {
          this.saldoAnual = 0;
        }
      });

    this.handleSaldoMesAno(this.filterMesService.dayJs);
  }
  
  private isSaldoNegativo = (saldo: number): boolean => {
    return saldo < 0;
  }
  
  public ngOnInit(): void {
    this.initialize();
  }

  public handleSaldoMesAno = (mes: Dayjs): void => {
    this.saldoService.getSaldoByMesANo(mes)
      .subscribe({
        next: (response: ISaldo) => {
          if (response && response !== undefined && response !== null) {
            this.saldoMensalNegativo = this.isSaldoNegativo(response.saldo) ? 'text-danger' : '';
            this.saldoMensal = response.saldo.toLocaleString('pt-br', {
              style: 'currency',
              currency: 'BRL',
              minimumFractionDigits: 2,
              maximumFractionDigits: 2
            });
          }
        },
        error: () => {
          this.saldoMensal = 0;
        }
      });
  }

  public handleSelectMes = (mes: string): void => {
    this.filterMesService.selectMonth = Number(mes);
    this.handleSaldoMesAno(dayjs(dayjs().format(`YYYY-${mes}-01`)));

  }
}