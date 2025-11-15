import { Component, ViewChild } from "@angular/core";
import dayjs from "dayjs";
import { ReceitasFormComponent } from "./receitas-form/receitas.form.component";
import { BarraFerramentaBase, DataTableComponent, AlertComponent, ModalFormComponent, ModalConfirmComponent, AlertType } from "../../components";
import { ReceitaDataSet, ReceitaColumns } from "../../models/datatable-config/receitas";
import { IReceita, Action } from "../../models";
import { MenuService, ReceitaService  } from "../../services";

@Component({
  selector: 'app-receitas',
  templateUrl: './receitas.component.html',
  styleUrls: ['./receitas.component.scss'],
  standalone: false
})
export class ReceitasComponent implements BarraFerramentaBase {
  @ViewChild(DataTableComponent) dataTable: DataTableComponent;
  receitasData: ReceitaDataSet[] = [];
  columns = ReceitaColumns;

  constructor(
    private menuService: MenuService,
    public modalAlert: AlertComponent,
    public modalForm: ModalFormComponent,
    public modalConfirm: ModalConfirmComponent,
    public receitaService: ReceitaService,
    private receitasFormComponent: ReceitasFormComponent
  ) { menuService.setMenuSelecionado(4); }

  ngOnInit() {
    this.menuService.setMenuSelecionado(4);
    this.initializeDataTable();
  }

  initializeDataTable = () => {
    this.receitaService.getReceitas()
      .subscribe({
        next: (result: IReceita[]) => {
          if (result) {
            this.receitasData = this.parseToReceitaData(result);
            this.dataTable.loadData(this.receitasData);
            this.dataTable.rerender();
          }

        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }

  updateDatatable = () => {
    this.receitaService.getReceitas()
      .subscribe({
        next: (result: any) => {
          if (result) {
            this.receitasData = this.parseToReceitaData(result);
            this.dataTable.rerender();
          }
        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }

  parseToReceitaData(receitas: IReceita[]): ReceitaDataSet[] {
    return receitas.map((receita: IReceita) => ({
      id: receita.id,
      data: dayjs(receita.data).format('DD/MM/YYYY'),
      categoria: receita.categoria.descricao,
      descricao: receita.descricao,
      valor: `${receita.valor.toLocaleString('pt-BR', {
        style: 'currency',
        currency: 'BRL',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      })}`
    }));
  }

  onClickNovo = () => {
    const modalRef = this.modalForm.modalService.open(ReceitasFormComponent, { centered: true });
    modalRef.shown.subscribe(() => {
      modalRef.componentInstance.action = Action.Create;
      modalRef.componentInstance.setRefresh(this.updateDatatable);
    });
  }

  onClickEdit = (idReceita: number) => {
    const modalRef = this.modalForm.modalService.open(ReceitasFormComponent, { centered: true });
    modalRef.shown.subscribe(() => {
      modalRef.componentInstance.action = Action.Edit;
      modalRef.componentInstance.setRefresh(this.updateDatatable);
      modalRef.componentInstance.editReceita(idReceita);
    });
  }

  onClickDelete = (idReceita: number) => {
    const modalRef = this.modalConfirm.open(ModalConfirmComponent, `Deseja excluir a receita ${this.dataTable.row.descricao} ?`);
    modalRef.shown.subscribe(() => {
      modalRef.componentInstance.setConfirmButton(() => {
        this.receitasFormComponent.deleteReceita(idReceita, () => { this.updateDatatable(); });
      });
    });
  }
}
