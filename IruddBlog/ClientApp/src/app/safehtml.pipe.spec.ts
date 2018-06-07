import { SafehtmlPipe } from './safehtml.pipe';
import { DomSanitizer, SafeValue, SafeHtml, SafeResourceUrl, SafeScript, SafeStyle, SafeUrl } from '@angular/platform-browser';
import { SecurityContext } from '@angular/core';

class MockSanitizer extends DomSanitizer {
  sanitize(context: SecurityContext, value: string | SafeValue): string {
    throw new Error("Method not implemented.");
  }
  bypassSecurityTrustHtml(value: string): SafeHtml {
    throw new Error("Method not implemented.");
  }
  bypassSecurityTrustStyle(value: string): SafeStyle {
    throw new Error("Method not implemented.");
  }
  bypassSecurityTrustScript(value: string): SafeScript {
    throw new Error("Method not implemented.");
  }
  bypassSecurityTrustUrl(value: string): SafeUrl {
    throw new Error("Method not implemented.");
  }
  bypassSecurityTrustResourceUrl(value: string): SafeResourceUrl {
    throw new Error("Method not implemented.");
  }
}

describe('SafehtmlPipe', () => {
  it('create an instance', () => {
    const pipe = new SafehtmlPipe(new MockSanitizer());
    expect(pipe).toBeTruthy();
  });
});
