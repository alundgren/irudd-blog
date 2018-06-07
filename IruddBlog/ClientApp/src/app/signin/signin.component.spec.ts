import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SigninComponent } from './signin.component';
import { Observable } from 'rxjs/Observable';
import { SocialUser, AuthService } from 'angular5-social-login';

class MockAuthService {
  constructor(private idToken: string) {
    this.mockUser = new SocialUser();
    this.mockUser.idToken = idToken;
  }
  mockUser: SocialUser;
  authState : Observable<SocialUser> = new Observable<SocialUser>((observer) => {
    observer.next(this.mockUser);
  });
}

describe('SigninComponent', () => {
  let component: SigninComponent;
  let fixture: ComponentFixture<SigninComponent>;

  beforeEach(async(() => {
    let authService = new MockAuthService("42");
    TestBed.configureTestingModule({
      declarations: [ SigninComponent ],
      providers: [{ provide: AuthService, useFactory: () => authService }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SigninComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
