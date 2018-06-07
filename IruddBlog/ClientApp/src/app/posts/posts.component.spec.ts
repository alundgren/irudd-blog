import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { PostsComponent } from './posts.component';
import { BlogService, IPostMetadata, IBlogService, ICreatePostResult } from '../blog.service';
import { Observable } from 'rxjs/Observable';
import { equal } from 'assert';

class MockBlogService implements IBlogService {
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
    return new Observable<IPostMetadata[]>((observer) => {
      setTimeout(() => {
        observer.next([{ postId : 'p1', publicationDate: new Date(), title: 'First post' }, { postId : 'p2', publicationDate: new Date(), title: 'Second post' }])  
      }, 10);
    })
  }
}

describe('PostsComponent', () => {
  let component: PostsComponent;
  let fixture: ComponentFixture<PostsComponent>;
  
  beforeEach(async(() => {
    const blogService = new MockBlogService();
    TestBed.configureTestingModule({
      declarations: [ PostsComponent ],
      providers: [ { provide: BlogService, useValue: blogService }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PostsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have two posts after initial load', () => {
    fixture.detectChanges();
    fixture.whenStable().then(() => { equal(2, component.posts.length); });    
  });  
});