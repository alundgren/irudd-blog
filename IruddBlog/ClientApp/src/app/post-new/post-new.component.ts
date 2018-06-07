import { Component, OnInit, ViewChild } from '@angular/core';
import { BlogService } from '../blog.service'
import { MarkdownEditorComponent } from '../markdown-editor/markdown-editor.component'
import { AuthService, SocialUser } from 'angular5-social-login';

@Component({
  selector: 'app-post-new',
  templateUrl: './post-new.component.html',
  styleUrls: ['./post-new.component.css']
})
export class PostNewComponent implements OnInit {
  content: string = '';
  title: string = '';
  creatingPost: boolean = false;
  authState : SocialUser;

  isSignedIn() {
    return this.authState && this.authState.idToken;
  }

  constructor(private blogService: BlogService, private socialAuthService: AuthService) { }

  createPost() {
    this.creatingPost = true;
    this.blogService.createPost(this.content, this.title, this.authState.idToken).subscribe(result => {
      document.location.href = '/posts/' + result.postId;
    })
  }

  ngOnInit() {
    this.socialAuthService.authState.subscribe(authState => {
      this.authState = authState;
    })
  }
}
