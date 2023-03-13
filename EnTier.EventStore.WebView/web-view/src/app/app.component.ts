import { Component } from '@angular/core';
import {ModalBlur} from "../services/modal-blur";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'web-view';

  blurCss:string='';

  constructor(private svcBlur:ModalBlur) {

    svcBlur.blurChange.subscribe(css => this.blurCss=css);
  }
}
