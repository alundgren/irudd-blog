import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

export interface IPostMetadata {
  PostId: string,
  Title: string,
  PublicationDate: Date
}

export interface IBlogService {
    getContent(postId: string) : Observable<string>,
    getMetadata(postId: string) : Observable<IPostMetadata>,
    getMetadatas() : Observable<IPostMetadata[]>,
    createPost(markdownContent: string, title: string, idToken: string) : Observable<ICreatePostResult>,
    getSettings() : Observable<BlogSettings>
}

interface IBeginCreatePostResult {
    postId: string,
    localImageUrls: string[]
}

export interface ICreatePostResult {
    postId: string,
}

export interface IGoogleSettings {
    blogOwnerUserId: string
    clientId: string
}

export class BlogSettings {
    googleSettings : IGoogleSettings
}

@Injectable()
export class BlogService implements IBlogService {
  constructor(private httpClient: HttpClient,  @Inject('BASE_URL')private baseUrl: string) {

  }

  getSettings() : Observable<BlogSettings> {
    return this.httpClient.post<BlogSettings>(this.baseUrl + 'api/settings', {});
  }

  getContent(postId: string) : Observable<string> {
      //The whole responseType text as json is a hack. See: https://github.com/angular/angular/issues/18586
      //responseType seems to just be a mess.
      return this.httpClient.get<string>(this.baseUrl + 'posts/' + postId + '/content.md', { responseType: 'text' as 'json' });
  }

  getMetadata(postId: string) : Observable<IPostMetadata> {
    return this.httpClient.get<IPostMetadata>(this.baseUrl + 'posts/' + postId + '/metadata.json');     
  }

  getMetadatas() : Observable<IPostMetadata[]> {
      return this.httpClient.post<IPostMetadata[]>(this.baseUrl + 'api/posts/get-metadatas', {});
  }
//TODO: step two auth: https://medium.com/@ryanchenkie_40935/angular-authentication-using-the-http-client-and-http-interceptors-2f9d1540eb8
  createPost(markdownContent: string, title: string, idToken: string) : Observable<ICreatePostResult> {
      //NOTE: If we ever start using the local service worker image cache again the split this up into begin and commit again and upload the local images between
      return new Observable<ICreatePostResult>((observer) => {
        let headers = new HttpHeaders().set('authorization', 'Bearer ' + idToken);
        this.httpClient.post<IBeginCreatePostResult>(this.baseUrl + 'api/posts/begin-create', { markdownContent: markdownContent, title: title }, { headers: headers}).subscribe(r1 => {
            if(r1.localImageUrls && r1.localImageUrls.length > 0) {
                throw new Error("local image urls are not supported!");
            }
            let postId = r1.postId;
            this.httpClient.post(this.baseUrl + 'api/posts/commit-create', { postId: postId }, { headers: headers}).subscribe(r2 => {
                observer.next({ postId: postId });
            })
        })
      })
  }
}