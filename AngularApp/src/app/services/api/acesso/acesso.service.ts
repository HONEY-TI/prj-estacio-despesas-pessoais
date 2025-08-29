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

  public signIn(login: ILogin): Observable<any> {
    return this.httpClient.post<ILogin>(`${this.routeUrl}/SignIn`, login);
  }

  public signInWithGoogleAccount(auth: IGoogleAuth): Observable<IAuth> {
    return this.httpClient.post<IGoogleAuth>(`${this.routeUrl}/SignInWithGoogle`, auth);

  }

  public createUsuario(acesso: IAcesso): Observable<any> {
    return this.httpClient.post<IAcesso>(`${this.routeUrl}`, acesso);
  }

  public changePassword(login: ILogin): Observable<any> {
    return this.httpClient.post<ILogin>(`${this.routeUrl}/ChangePassword`, login);
  }

  public refreshToken(refreshToken: string): Observable<any> {
    return this.httpClient.get(`${this.routeUrl}/refreshtoken/${refreshToken}`);
  }
}
