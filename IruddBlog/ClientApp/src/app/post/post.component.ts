import { Component, OnInit, Pipe } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { BlogService } from '../blog.service';
import { Converter } from 'showdown';
import { MarkdownViewerComponent } from '../markdown-viewer/markdown-viewer.component'

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css']
})
export class PostComponent implements OnInit {
  postId : string;
  publicationDate: Date = null;
  content: string = 'Loading...';
  title: string = 'Loading...';
  markdownConverter : Converter

  constructor(private route: ActivatedRoute,
    private blogService: BlogService) { 
      this.markdownConverter = new Converter();
    }

  ngOnInit() {
    this.route.paramMap.subscribe(p => {
      this.postId = p.get('id');
      this.blogService.getContent(this.postId).subscribe(c => {
        this.blogService.getMetadata(this.postId).subscribe(m => {
          this.content = c;//this.markdownConverter.makeHtml(c);
          this.title = m.Title;
          this.publicationDate = m.PublicationDate;
        })
      })
    });
  }

}
