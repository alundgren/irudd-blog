import { Component, Input, OnChanges, SimpleChanges, OnInit, AfterViewChecked } from '@angular/core';

@Component({
  selector: 'app-markdown-viewer',
  templateUrl: './markdown-viewer.component.html',
  styleUrls: ['./markdown-viewer.component.css']
})
export class MarkdownViewerComponent implements OnChanges, OnInit, AfterViewChecked {
    renderedHtml: string;
    @Input('content') markdownContent: string;
  
    constructor() {
      
    }

    ngOnInit(): void {
      this.synchHtml()
    }

    ngOnChanges(changes: SimpleChanges): void {
      this.synchHtml()
    }

    ngAfterViewChecked(): void {
      PR.prettyPrint();
    }    

    private synchHtml() {
      if(this.markdownContent) {
        let converter = new Markdown.Converter();
        converter.hooks.chain("postConversion", function (text, rbg) {
          return text ? text.replace('<pre>', '<pre class="prettyprint">') : text;
        });
        this.renderedHtml =  converter.makeHtml(this.markdownContent);
      } else {
        this.renderedHtml = '';
      }    
    }

}
