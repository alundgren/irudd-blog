import { Component, OnInit } from '@angular/core';

import {
  AuthService,
  GoogleLoginProvider,
  SocialUser
} from 'angular5-social-login';

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  styleUrls: ['./signin.component.css']
})
export class SigninComponent implements OnInit {
  authState : SocialUser = null;

  constructor(private socialAuthService: AuthService) {
    this.socialAuthService.authState.subscribe(authState => {
      this.authState = authState; 
    })
  }

  isSignedIn() {
    return this.authState && this.authState.idToken
  }
  
  signInWithGoogle(event: Event) {
    if(event) {
      event.preventDefault();
    }
    console.log('sign in: ' + GoogleLoginProvider.PROVIDER_ID);
    this.socialAuthService.signIn(GoogleLoginProvider.PROVIDER_ID);
  }

  signOut(event: Event) {
    if(event) {
      event.preventDefault();
    }
    this.socialAuthService.signOut().then(r => console.log(r))
  }

  ngOnInit() {
  }

}
