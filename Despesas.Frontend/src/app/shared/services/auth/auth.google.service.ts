import { Injectable } from '@angular/core';
import { environment } from "../../../../environments/environment";

declare const google: any;

@Injectable({
  providedIn: 'root'
})

export class AuthGoogleService {

  constructor() {
    this.initializeGsi();
  }

  initializeGsi() {
    window.onload = () => {
      google.accounts.id.initialize({
        client_id: environment.googleClientId,
        callback: this.handleCredentialResponse.bind(this)
      });

      google.accounts.id.renderButton(
        document.getElementById("googleSignInButton"),
        { theme: "outline", size: "large" }
      );
    };
  }

  handleCredentialResponse(response: any) {
    console.log('Encoded JWT ID token: ' + response.credential);
    // Aqui você envia o token para o seu backend
  }
}
