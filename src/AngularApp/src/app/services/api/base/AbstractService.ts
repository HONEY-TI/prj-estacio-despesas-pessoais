import { environment } from "../../../../environments/environment";
export abstract class AbstractService {
  protected routeUrl: string;
  private baseUrl:string = `${environment.BASE_URL}`;
  constructor(route: string) {
    this.routeUrl = `${ this.baseUrl }/${route}`;
  }
}
