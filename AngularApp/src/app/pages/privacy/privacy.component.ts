import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-privacy',
  templateUrl: './privacy.component.html',
  styleUrls: ['./privacy.component.scss'],
  standalone: true
})

export class PrivacyComponent implements OnInit {

  constructor() { }

  ngOnInit() { }
}


@Component({
  selector: 'app-privacy',
  templateUrl: './privacy.component.html',
  standalone: true
})

export class RedirectPrivacyComponent implements OnInit {
  ngOnInit() {
    window.location.href = '/#/privacy';
}
}