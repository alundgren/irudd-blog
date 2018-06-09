import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MarkdownViewerComponent } from './markdown-viewer.component';
import { Pipe, PipeTransform } from '@angular/core';
import { SafeHtml } from '@angular/platform-browser';
import { SafehtmlPipe } from '../safehtml.pipe';

@Pipe({name: 'safehtml'})
class MockHtmlPipe implements PipeTransform {
    transform(value: string): SafeHtml {
        //Do stuff here, if you want
        return value;
    }
}

describe('MarkdownViewerComponent', () => {
  let component: MarkdownViewerComponent;
  let fixture: ComponentFixture<MarkdownViewerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MarkdownViewerComponent, MockHtmlPipe ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MarkdownViewerComponent);
    component = fixture.componentInstance;
    component.markdownContent = '**title**<pre>public class Kitten { public int NrOfSpots {get; set;} }</pre>'
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.renderedHtml).toBe('<p><strong>title</strong><pre class="prettyprint">public class Kitten { public int NrOfSpots {get; set;} }</pre></p>')
  });
});
