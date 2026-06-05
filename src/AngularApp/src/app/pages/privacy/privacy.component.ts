import { Component, OnInit } from '@angular/core';
import { FooterComponent } from '../../components/footer/footer.component';

@Component({
  selector: 'app-privacy',
  templateUrl: './privacy.component.html',
  styleUrls: ['./privacy.component.scss'],
  imports: [ FooterComponent],
  standalone: true
})

export class PrivacyComponent implements OnInit {

  constructor() { }

  public ngOnInit(): void { }
}