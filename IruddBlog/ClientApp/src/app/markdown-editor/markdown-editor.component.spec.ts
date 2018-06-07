import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MarkdownEditorComponent } from './markdown-editor.component';
import { IBlogService, IPostMetadata, BlogService } from '../blog.service';
import { Observable } from 'rxjs/Observable';
import { Mock } from 'protractor/built/driverProviders';
import { HttpClientModule } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AuthService, SocialUser } from 'angular5-social-login';

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

describe('MarkdownEditorComponent', () => {
  let component: MarkdownEditorComponent;
  let fixture: ComponentFixture<MarkdownEditorComponent>;

  beforeEach(async(() => {
    let authService = new MockAuthService("42");
    TestBed.configureTestingModule({
      declarations: [ MarkdownEditorComponent ],
      imports: [HttpClientModule, HttpClientTestingModule],
      providers: [{ provide: AuthService, useFactory: () => authService }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MarkdownEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
