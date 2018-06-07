import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
import { getAllExtensions } from 'showdown';
import axios from 'axios';
import { AuthServiceConfig, GoogleLoginProvider } from 'angular5-social-login';
import { BlogSettings } from './app/blog.service';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

//NOTE: This whole thing exists soley to make the damn DI actually work. The problem is that AuthServiceConfig just wont work with an async dependany ... should work with APP_INITIALIZR but just doesnt
axios.post(getBaseUrl() + 'api/settings', {}).then(settings => {  
  let config = new AuthServiceConfig(
      [
        {
          id: GoogleLoginProvider.PROVIDER_ID,
          provider: new GoogleLoginProvider(settings.data.googleSettings.clientId)
        },
      ]
  )
  const providers = [
    { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
    { provide: AuthServiceConfig, useFactory: (() => config), deps: [] }, //<-- this line is the only reason for this whole axios massive hack crap
    { provide: BlogSettings, useFactory: (() => settings), deps: [] },
  ];
  
  if (environment.production) {
    enableProdMode();
  }
  
  platformBrowserDynamic(providers).bootstrapModule(AppModule)
    .catch(err => console.log(err));
})

