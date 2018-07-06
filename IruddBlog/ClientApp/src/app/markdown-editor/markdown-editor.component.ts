//This used to be just this: https://github.com/doxiaodong/ng2-simplemde
//Unfortunately this hides the simplemde-instance  which I need to handle the paste events, hence the copy and change

import {
  Component,
  OnInit,
  NgModule,
  forwardRef,
  ElementRef,
  ViewChild,
  Input,
  OnDestroy,
  ModuleWithProviders,
  Inject,
  Output,
  EventEmitter,
  AfterContentInit,
  AfterViewInit
} from '@angular/core'
import { CommonModule } from '@angular/common'
import { FormsModule, NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms'
import * as SimpleMDE from 'simplemde'
import { NgModelBase } from './ngmodelbase';

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { AuthService, SocialUser } from 'angular5-social-login';

interface IDocumentOnPaste {
  onpaste : ((evt : any) => void)
}

interface IAddImageResult {
  relativeUrl : string
}

@Component({
  selector: 'app-markdown-editor',
  templateUrl: './markdown-editor.component.html',
  styleUrls: ['./markdown-editor.component.css']
})
export class MarkdownEditorComponent implements OnInit  {
  @ViewChild('angularref') 
  textarea: ElementRef

  @Output() markdownChanged = new EventEmitter<string>();

  constructor(private httpClient: HttpClient, private socialAuthService: AuthService) {
  
  }

  authState : SocialUser;

  ngOnInit() {
    var h: HTMLTextAreaElement
    h = this.textarea.nativeElement
    h.addEventListener('input', () => {
      this.markdownChanged.emit(h.value);
    })
    this.socialAuthService.authState.subscribe(authState => {
      this.authState = authState;
    })

    let converter = new Markdown.Converter();
    converter.hooks.chain('postNormalization', (text, rgb) => {      
      return text;
    })
    let editor = new Markdown.Editor(converter);    
    editor.run();

    let documentWithOnPaste : IDocumentOnPaste = (<any>document);
    documentWithOnPaste.onpaste = (event) => {
      try {
          let isTargetedAtEditor = false;
          for(let el of event.path) {
              if(el.id == 'pastetarget') {
                  isTargetedAtEditor = true
              }
          }
          if(!isTargetedAtEditor) {
              return;
          }
          
          var items = event.clipboardData.items;
          if(items.length != 1 || items[0].kind != 'file') {
            return;
          }

          event.preventDefault();
          var blob = items[0].getAsFile();
          var reader = new FileReader();
          reader.onload = (event: any) => {
              try {
                  let dataUrl = event.target.result;
                  if(dataUrl.startsWith('data:image/')) {
                    let headers = new HttpHeaders().set('authorization', 'Bearer ' + this.authState.idToken);
                    this.httpClient.post<IAddImageResult>('/api/temporary-images/add-as-dataurl', { dataurl: dataUrl }, { headers: headers }).subscribe(result => {                      
                      this.insertTextAtCursor(this.textarea.nativeElement, '![](' + result.relativeUrl + ')');
                      this.textarea.nativeElement.dispatchEvent(new Event('input')) //Pagedown doesnt listen to change only input, keypress and keydown so this is just to fire the change detection
                    })
                  }
              } catch(err2) {
                  console.log(err2)
              }
          };
          reader.readAsDataURL(blob);
      } catch (err) {
          console.log(err)
      }
    }
  }

  insertTextAtCursor(myField : HTMLTextAreaElement, myValue: string) {
    if (myField.selectionStart || myField.selectionStart == 0) {
        var startPos = myField.selectionStart;
        var endPos = myField.selectionEnd;
        myField.value = myField.value.substring(0, startPos)
            + myValue
            + myField.value.substring(endPos, myField.value.length);
        myField.selectionStart = startPos + myValue.length; 
        myField.selectionEnd = startPos + myValue.length;
    } else {
        myField.value += myValue;
    }
  }
}

