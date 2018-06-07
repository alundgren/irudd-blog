import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms'

import { PostNewComponent } from './post-new.component';
import { MarkdownEditorComponent } from '../markdown-editor/markdown-editor.component'
import { IBlogService, IPostMetadata, BlogService, ICreatePostResult, BlogSettings } from '../blog.service';
import { Observable } from 'rxjs/Observable';
import { Mock } from 'protractor/built/driverProviders';
import { HttpClientModule } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterModule } from '@angular/router';
import { SocialUser, AuthService } from 'angular5-social-login';

class MockBlogService implements IBlogService {
  getSettings(): Observable<BlogSettings> {
    throw new Error("Method not implemented.");
  }
  createPost(markdownContent: string, title: string): Observable<ICreatePostResult> {
    throw new Error("Method not implemented.");
  }
  getContent(postId: string): Observable<string> {
    throw new Error("Method not implemented.");
  }
  getMetadata(postId: string): Observable<IPostMetadata> {
    throw new Error("Method not implemented.");
  }
  getMetadatas(): Observable<IPostMetadata[]> {
    throw new Error("Method not implemented.");
  }
}

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

describe('PostNewComponent', () => {
  let component: PostNewComponent;
  let fixture: ComponentFixture<PostNewComponent>;

  beforeEach(async(() => {
    let blogService = new MockBlogService();
    let authService = new MockAuthService("42");
    TestBed.configureTestingModule({
      declarations: [ PostNewComponent, MarkdownEditorComponent ],
      imports: [FormsModule, HttpClientModule, HttpClientTestingModule, RouterModule],
      providers: [ { provide: BlogService, useValue: blogService }, { provide: AuthService, useFactory: () => authService }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PostNewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
