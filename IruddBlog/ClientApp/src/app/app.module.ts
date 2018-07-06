import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import {
  SocialLoginModule,
  AuthServiceConfig,
  GoogleLoginProvider
} from "angular5-social-login";
import { environment } from '../environments/environment'

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { PostsComponent } from './posts/posts.component';
import { BlogService, BlogSettings } from './blog.service';
import { PostNewComponent } from './post-new/post-new.component'
import { MarkdownEditorComponent } from './markdown-editor/markdown-editor.component';
import { PostComponent } from './post/post.component'
import { SigninComponent } from './signin/signin.component';
import { MarkdownViewerComponent } from './markdown-viewer/markdown-viewer.component';
import { SafehtmlPipe } from './safehtml.pipe';
import { LayoutService } from './layout.service';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    PostsComponent,
    PostNewComponent,
    MarkdownEditorComponent,
    PostComponent,
    SafehtmlPipe,
    SigninComponent,
    MarkdownViewerComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: PostsComponent, pathMatch: 'full' },
      { path: 'posts/:id', component: PostComponent },
      { path: 'newpost', component: PostNewComponent },
      { path: 'signin', component: SigninComponent }
    ]),
    SocialLoginModule
  ],
  providers: [BlogService, LayoutService],
  bootstrap: [AppComponent]
})
export class AppModule { }
