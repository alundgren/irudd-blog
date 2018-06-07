//This used to be just this: https://github.com/doxiaodong/ng2-simplemde
//Unfortunately this hides the simplemde-instance  which I need to handle the paste events, hence the copy and change

import {
  Component,
  OnInit,
  NgModule,
  forwardRef,
  ElementRef,
  ViewChild,
  AfterViewInit,
  Input,
  OnDestroy,
  ModuleWithProviders,
  Inject
} from '@angular/core'
import { CommonModule } from '@angular/common'
import { FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms'
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
  styleUrls: ['./markdown-editor.component.css'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => MarkdownEditorComponent),
    multi: true
  }]
})
export class MarkdownEditorComponent extends NgModelBase implements AfterViewInit, OnDestroy {  
  constructor(private httpClient: HttpClient, private socialAuthService: AuthService) {
    super()
  }

  authState : SocialUser;

  @ViewChild('simplemde') 
  textarea: ElementRef

  ngOnInit() {
    this.socialAuthService.authState.subscribe(authState => {
      this.authState = authState;
    })    
  }

  private simplemde: SimpleMDE
  private tmpValue = null

  ngAfterViewInit() {
    const config : any = { }
    config.element = this.textarea.nativeElement

    this.simplemde = new SimpleMDE(config)

    if (this.tmpValue) {
      this.simplemde.value(this.tmpValue)
      this.tmpValue = null
    }

    this.simplemde.codemirror.on('change', () => {
      this.value = this.simplemde.value()
    })

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
          var blob = items[0].getAsFile();
          var reader = new FileReader();
          reader.onload = (event: any) => {
              try {
                  let dataUrl = event.target.result;
                  if(dataUrl.startsWith('data:image/')) {
                    let headers = new HttpHeaders().set('authorization', 'Bearer ' + this.authState.idToken);
                    this.httpClient.post<IAddImageResult>('/api/temporary-images/add-as-dataurl', { dataurl: dataUrl }, { headers: headers }).subscribe(result => {
                      let pos = this.simplemde.codemirror.getCursor();
                      this.simplemde.codemirror.setSelection(pos, pos);
                      this.simplemde.codemirror.replaceSelection('![](' + result.relativeUrl + ')');                      
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

  ngOnDestroy() {
    this.simplemde = null
  }
}

