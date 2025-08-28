import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ILogin } from '../../../models/ILogin';
import { AbstractService } from '../base/AbstractService';
import { Observable } from 'rxjs';
import { IAcesso, IAuth } from '../../../models';
import { IGoogleAuth } from '../../../models/IGoogleAuth';

@Injectable({
  providedIn: 'root'
})

export class AcessoService extends AbstractService {
  constructor(public httpClient: HttpClient) {
    const ROUTE = 'Acesso';
    super(ROUTE);
  }

  signIn(login: ILogin): Observable<any> {
    return this.httpClient.post<ILogin>(`${this.routeUrl}/SignIn`, login);
  }

  signInWithGoogleAccount(auth: IGoogleAuth): Observable<IAuth> {
    return this.httpClient.post<IGoogleAuth>(`${this.routeUrl}/SignInWithGoogle`, auth);

  }

  createUsuario(acesso: IAcesso): Observable<any> {
    return this.httpClient.post<IAcesso>(`${this.routeUrl}`, acesso);
  }

  changePassword(login: ILogin): Observable<any> {
    return this.httpClient.post<ILogin>(`${this.routeUrl}/ChangePassword`, login);
  }

}
