import { TestBed, inject, async } from '@angular/core/testing';
import { BlogService } from './blog.service';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

describe('BlogService', () => {
  beforeEach(() => {
    let baseUrl = 'http://example.org:42/';

    TestBed.configureTestingModule({
      providers: [BlogService,
      { provide: 'BASE_URL', useValue: baseUrl}
     ],
     imports: [HttpClientModule, HttpClientTestingModule]
    });
  });

  it('should be created', inject([BlogService], (service: BlogService) => {
    expect(service).toBeTruthy();
  }));

  it(`gets posts from correct url`,
    async(
      inject([BlogService, HttpTestingController], (service: BlogService, backend: HttpTestingController) => {
        service.getMetadatas().subscribe();
        backend.expectOne({
          url: 'http://example.org:42/api/posts/get-metadatas',
          method: 'post'
        });        
        backend.verify();
      })
    )
  );

  it(`gets content from correct url`,
    async(
      inject([BlogService, HttpTestingController], (service: BlogService, backend: HttpTestingController) => {
        service.getContent('a42').subscribe();        
        backend.expectOne({                
          url: 'http://example.org:42/posts/a42/content.md',
          method: 'get'
        });        
        backend.verify();        
      })
    )
  ); 
  
  it(`gets metadata from correct url`,
    async(
      inject([BlogService, HttpTestingController], (service: BlogService, backend: HttpTestingController) => {
        service.getMetadata('a42').subscribe();        
        backend.expectOne({                
          url: 'http://example.org:42/posts/a42/metadata.json',
          method: 'get'
        });        
        backend.verify();        
      })
    )
  );   

});
