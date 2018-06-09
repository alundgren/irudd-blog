import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PostComponent } from './post.component';
import { BlogService, IBlogService, ICreatePostResult, IPostMetadata, BlogSettings } from '../blog.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of'
import { ActivatedRoute, ParamMap, convertToParamMap } from '@angular/router';
import { RouterTestingModule } from "@angular/router/testing";
import { Pipe, PipeTransform } from '@angular/core';
import { SafeHtml } from '@angular/platform-browser';
import { MarkdownViewerComponent } from '../markdown-viewer/markdown-viewer.component'

@Pipe({name: 'safehtml'})
class MockHtmlPipe implements PipeTransform {
    transform(value: string): SafeHtml {
        //Do stuff here, if you want
        return value;
    }
}

class MockBlogService implements IBlogService {
  getSettings(): Observable<BlogSettings> {
    throw new Error("Method not implemented.");
  }
  createPost(markdownContent: string, title: string): Observable<ICreatePostResult> {
    throw new Error("Method not implemented.");
  }
  getContent(postId: string): Observable<string> {
    return new Observable<string>((observer) => observer.next('First post content!'));
  }
  getMetadata(postId: string): Observable<IPostMetadata> {
    return new Observable<IPostMetadata>((observer) => observer.next({
      PostId: 'p1',
      Title: 'First post',
      PublicationDate: new Date()
    }));
  }
  getMetadatas(): Observable<IPostMetadata[]> {
    return new Observable<IPostMetadata[]>((observer) => {
      setTimeout(() => {
        observer.next([{ PostId : 'p1', PublicationDate: new Date(), Title: 'First post' }, { PostId : 'p2', PublicationDate: new Date(), Title: 'Second post' }])  
      }, 10);
    })
  }
}

describe('PostComponent', () => {
  let component: PostComponent;
  let fixture: ComponentFixture<PostComponent>;

  beforeEach(async(() => {
    let blogService = new MockBlogService();
    TestBed.configureTestingModule({
      declarations: [ PostComponent, MockHtmlPipe, MarkdownViewerComponent ],
      providers: [{ provide: BlogService, useValue: blogService}, 
        { provide: ActivatedRoute, useValue: { paramMap: Observable.of(convertToParamMap({id: 'p1' }))}} ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PostComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
