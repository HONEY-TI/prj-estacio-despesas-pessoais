import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-cookie-consent',
  templateUrl: './cookie-consent.component.html',
  styleUrls: ['./cookie-consent.component.scss'],
  imports: [CommonModule] ,
  standalone: true
})

export class CookieConsentComponent implements OnInit {
  showBanner = false;

  public ngOnInit() {
    const consent = localStorage.getItem('cookie-consent');
    if (!consent) {
      this.showBanner = true;
    }
  }

  public acceptCookies = () =>  {
    localStorage.setItem('cookie-consent', 'accepted');
    this.showBanner = false;
  }

  public declineCookies = () => {
    localStorage.setItem('cookie-consent', 'declined');
    this.showBanner = false;
  }
}
