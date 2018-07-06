import { Component } from '@angular/core';
import { LayoutService } from '../layout.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  useWideContainer: boolean;

  constructor(private layout: LayoutService) {
    this.layout.useWideContainer.subscribe(x => {
      this.useWideContainer = x;
    })
  }  
}
