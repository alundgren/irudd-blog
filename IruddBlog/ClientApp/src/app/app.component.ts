import { Component } from '@angular/core';
import { LayoutService } from './layout.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = "Irudd's blog";

  useWideContainer: boolean;

  constructor(private layout: LayoutService) {
    this.layout.useWideContainer.subscribe(x => {
      this.useWideContainer = x;
    })
  } 
}
