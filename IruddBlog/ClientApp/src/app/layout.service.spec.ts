import { TestBed, inject } from '@angular/core/testing';

import { LayoutService } from './layout.service';
import { RouterTestingModule } from '../../node_modules/@angular/router/testing';

describe('LayoutService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [LayoutService],
      imports: [
        RouterTestingModule.withRoutes([])
      ]      
    });
  });

  it('should be created', inject([LayoutService], (service: LayoutService) => {
    expect(service).toBeTruthy();
  }));
});
