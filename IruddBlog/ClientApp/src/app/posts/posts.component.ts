import { Component, OnInit, Inject } from '@angular/core';
import { BlogService, IPostMetadata, BlogSettings } from '../blog.service'

@Component({
  selector: 'app-posts',
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.css']
})
export class PostsComponent implements OnInit {
  posts : IPostMetadata[];

  constructor(private blogService: BlogService) {
    this.posts = []
  }

  ngOnInit() {
    this.blogService.getMetadatas().subscribe(result => {
      this.posts = result;
    })
  }
}
