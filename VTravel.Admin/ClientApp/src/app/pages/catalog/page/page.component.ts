import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { NbDialogService, NbToastrService, NbComponentStatus  } from '@nebular/theme';
import '../ckeditor.loader';
import 'ckeditor';

@Component({
  selector: 'page',
  templateUrl: './page.component.html',
  styleUrls: ['./page.component.scss'],
})


export class PageComponent implements OnInit  {

  aboutDialogRef: any;
 
 
  deleteDialogRef: any;
 
  aboutEditMode = false;
  isLoading = false;
  loadingPageSave = false;
  loadingAboutSave = false;
  loadingStatusSave = false;
  loadingDelete = false;
  
  isAlert = false;
  alertMessage = '';
  alertStatus = '';
  page = new Page();
 
  token: any; 
  pageAbout: string;
 

  constructor(private http: HttpClient, private authService: NbAuthService, 
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {

  
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

     
    });
   
  }

  ngOnInit() {

   
    this.loadPage();
   
    
    
    
  }
  

  loadPage() {

    let id = this.r.snapshot.paramMap.get('id');

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/page/get?id=' + id
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {

          

          this.page = res.data;
          
        }

      },
        error => {
        
          console.log('api/page/get', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }
  


  openAboutDialog(aboutDialog: TemplateRef<any>) {

    this.pageAbout = this.page.content;

    this.aboutDialogRef = this.dialogService.open(
      aboutDialog,
      { context: { title: 'Edit Content' } });
  }
  closeAboutEditMode() {
    this.pageAbout = '';
    this.aboutEditMode = false;
  }
  //
  updatePageContent() {
   

    this.loadingAboutSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/page/update-about?id=' + this.page.id
      , {"content": this.pageAbout }, { headers: headers }).subscribe((res: any) => {
        this.loadingAboutSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.page.content = this.pageAbout;
          this.pageAbout = '';
          this.aboutDialogRef.close();
       
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {

        
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingAboutSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/page/update-about', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });

  }

  updatePageStatus(pageStatus) {


    this.loadingStatusSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/page/update-status?id=' + this.page.id
      , { "pageStatus": pageStatus }, { headers: headers }).subscribe((res: any) => {
        this.loadingStatusSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadPage();         
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {

        
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingAboutSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/page/update-status', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });

  }

  openDelete(deleteDialog: TemplateRef<any>) {
    this.deleteDialogRef=this.dialogService.open(
      deleteDialog,
      { context: { title: 'Delete Page' } });
  }
  deletePage() {


    this.loadingDelete = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/page/delete?id=' + this.page.id
          , { headers: headers }).subscribe((res: any) => {
            this.loadingDelete = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Page deleted successfully!', 'success');
          this.deleteDialogRef.close();
          this.router.navigate(['/pages/catalog/pages']);
        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingDelete = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/page/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  
  

  onBasicFormSubmit() {

    

    
    this.loadingPageSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/page/update?id=' + this.page.id
      , this.page, { headers: headers }).subscribe((res: any) => {
        this.loadingPageSave = false;
      
        if (res.actionStatus === 'SUCCESS') {

          this.loadingPageSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
        }
        else {

          this.loadingPageSave = false;
          this.toast('Error', 'Could not save data!', 'danger');
         

        }

      },
        error => {

          this.loadingPageSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/page/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  

  toast(title, message, status: NbComponentStatus) {    
    this.toastrService.show(title, message, { status });
  }


}

class Page {
 
  id: number;  
  title: string;   
  content: string;
  pageStatus: string;
  sortOrder: number;  
  metaTitle: string;
  metaKeywords: string;
  metaDescription: string;
  
}

