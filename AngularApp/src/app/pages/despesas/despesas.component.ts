import { Component, OnInit, ViewChild } from "@angular/core";
import { DespesasFormComponent } from "./despesas-form/despesas.form.component";
import { BarraFerramentaBase, DataTableComponent, AlertComponent, ModalFormComponent, ModalConfirmComponent, AlertType } from "../../components";
import { DespesaDataSet, DespesaColumns } from "../../models/datatable-config/despesas";
import { IDespesa, Action } from "../../models";
import { MenuService, DespesaService } from "../../services";
import dayjs from "dayjs";

@Component({
  selector: 'app-despesas',
  templateUrl: './despesas.component.html',
  styleUrls: ['./despesas.component.scss'],
  standalone: false
})

export class DespesasComponent implements BarraFerramentaBase, OnInit {
  @ViewChild(DataTableComponent) dataTable: DataTableComponent;
  despesasData: DespesaDataSet[] = [];
  columns = DespesaColumns;

  constructor(
    private menuService: MenuService,
    public modalAlert: AlertComponent,
    public modalForm: ModalFormComponent,
    public modalConfirm: ModalConfirmComponent,
    public despesaService: DespesaService,
    private despesasFormComponent: DespesasFormComponent) { }

  public ngOnInit(): void {
    this.menuService.setMenuSelecionado(3);
    this.initializeDataTable();
  }

  private parseToDespesasData(despesas: IDespesa[]): DespesaDataSet[] {
    return despesas.map((despesa: IDespesa) => ({
      id: despesa.id,
      data: dayjs(despesa.data).format('DD/MM/YYYY'),
      categoria: despesa.categoria.descricao,
      descricao: despesa.descricao,
      valor: `${despesa.valor.toLocaleString('pt-BR', {
        style: 'currency',
        currency: 'BRL',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      })}`,
      dataVencimento: (despesa.dataVencimento && dayjs(despesa.dataVencimento).isValid()) ? dayjs(despesa.dataVencimento).format('DD/MM/YYYY') : null
    }));
  }

  public initializeDataTable = (): void => {
    this.despesaService.getDespesas()
      .subscribe({
        next: (result: IDespesa[]) => {
          if (result) {
            this.despesasData = this.parseToDespesasData(result);
            this.dataTable.loadData(this.despesasData);
            this.dataTable.rerender();
          }
        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }

  public updateDatatable = (): void => {
    this.despesaService.getDespesas()
      .subscribe({
        next: (result: any) => {
          if (result) {
            this.despesasData = this.parseToDespesasData(result);
            this.dataTable.rerender();
          }
        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }

  public onClickNovo = (): void => {
    const modalRef = this.modalForm.modalService.open(DespesasFormComponent, { centered: true });
    modalRef.shown.subscribe(() => {
      modalRef.componentInstance.action = Action.Create;
      modalRef.componentInstance.setRefresh(this.updateDatatable);
    });
  }

  public onClickEdit = (idDespesa: number): void => {
    const modalRef = this.modalForm.modalService.open(DespesasFormComponent, { centered: true });
    modalRef.shown.subscribe(() => {
      modalRef.componentInstance.action = Action.Edit;
      modalRef.componentInstance.setRefresh(this.updateDatatable);
      modalRef.componentInstance.editDespesa(idDespesa);
    });
  }

  public onClickDelete = (idDespesa: number): void => {
    const modalRef = this.modalConfirm.open(ModalConfirmComponent, `Deseja excluir a despesa ${this.dataTable.row.descricao} ?`);
    modalRef.shown.subscribe(() => {
      modalRef.componentInstance.setConfirmButton(() => this.despesasFormComponent.deleteDespesa(idDespesa, this.updateDatatable));
    });
  }
}
