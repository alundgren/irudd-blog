import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class LayoutService {
  public useWideContainer: Observable<boolean>

  constructor(private router: Router) {
    this.useWideContainer = new Observable<boolean>(observer => {
      this.router.events.subscribe(x => {
        observer.next(this.router.isActive('newpost', false));
      })
    })
  }

}
