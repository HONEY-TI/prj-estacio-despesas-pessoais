import { Component } from '@angular/core';
import { AuthService } from './services/auth/auth.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  providers: [AuthService],
  standalone: false
})

export class AppComponent  {
  constructor() {

  }
}
