import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { NbDialogService, NbComponentStatus, NbToastrService} from '@nebular/theme';



@Component({
  selector: 'pages',
  templateUrl: './pages.component.html',
  styleUrls: ['./pages.component.scss'],
})


export class PagesComponent implements OnInit  {

 
  isLoading = false;
  loadingSave = false;
  isAlert = false;
  alertMessage = '';
  alertStatus = '';
  pages: any[];
  pagesAll: any[];
  token: any; 
  searchValue: string;
  page = new Page();
  dialogRef: any;

  constructor(private http: HttpClient, private authService: NbAuthService, 
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {

  
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

      
    });
   
  }

  ngOnInit() {
    this.loadPages();
   
  }

  pageSearch() {
    if (this.searchValue != null) {
      
      this.pages = this.pagesAll.filter((item)=>{
        return item.title.toLowerCase().indexOf(this.searchValue.toLowerCase()) != -1;
      });

    } else {
      this.pages = this.pagesAll;
    }
   
  }
  
  private loadPages() {

    this.loadingSave = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/page/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.loadingSave = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.pagesAll = res.data;
            this.pageSearch();
          }
        }

      },
        error => {
          this.loadingSave = false;
          console.log('api/page/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  
  }

  
  

  loadPage(id) {
    
    this.router.navigate(['/pages/catalog/page', id]);
  }

  

  drop(event: CdkDragDrop<string[]>) {
    var page = this.pages[event.previousIndex];
    var pageReplace = this.pages[event.currentIndex];

   
    moveItemInArray(this.pages, event.previousIndex, event.currentIndex);
      // sort in backend

      var sortData = new SortData();
     
      sortData.itemId = page.id;
    sortData.sortOrder = pageReplace.sortOrder;
    sortData.pushDownValue = page.sortOrder >= pageReplace.sortOrder ? 1 : -1;

      let headers = new HttpHeaders().set("Authorization", "Bearer " +
        this.token).set("Content-Type", "application/json");

      //console.log(JSON.stringify(sortData));

      this.http.post('api/page/sort'
        , sortData, { headers: headers }).subscribe((res: any) => {
          this.loadingSave = false;
          //console.log(JSON.stringify(res));
          if (res.actionStatus === 'SUCCESS') {

            this.loadingSave = false;
            this.isAlert = true;
            this.alertStatus = 'success';
            this.alertMessage = "Data sorted successfully!";
            setTimeout(() => {
              this.isAlert = false;
              this.alertMessage = '';
            }, 3000);
          }
          else {

            this.loadingSave = false;
            this.isAlert = true;
            this.alertStatus = 'danger';
            this.alertMessage = "Could not sort data!";

            setTimeout(() => {
              this.isAlert = false;
              this.alertMessage = '';

            }, 3000);

          }

        },
          error => {

            this.loadingSave = false;
            this.isAlert = true;
            this.alertStatus = 'danger';
            this.alertMessage = "Could not import data!";

            setTimeout(() => {
              this.isAlert = false;
              this.alertMessage = '';

            }, 3000);

            console.log('api/page/sort', error);
            if (error.status === 401) {
              this.router.navigate(['auth/login']);
            }

          });

    
  }


  openAddPage(addPropertyDialog: TemplateRef<any>) {

    this.dialogRef= this.dialogService.open(
      addPropertyDialog,
      { context: { title: 'Add Page' } });

  }

  onFormSubmit() {


   
      this.loadingSave = true;
     
      let headers = new HttpHeaders().set("Authorization", "Bearer " +
        this.token).set("Content-Type", "application/json");

    this.http.post('api/page/create'
      , this.page, { headers: headers }).subscribe((res: any) => {
          this.loadingSave = false;
          
          if (res.actionStatus === 'SUCCESS') {

            this.dialogRef.close();

            this.loadingSave = false;
            this.toast('Success', 'Data saved successfully!', 'success');
            this.router.navigate(['/pages/catalog/page', res.data.id]);
          }
          else {

            this.loadingSave = false;
            this.toast('Error', 'Could not save data!', 'danger');
            
          }

        },
          error => {

            this.loadingSave = false;
            this.toast('Error', 'Could not save data!', 'danger');

            console.log('api/page/create', error);
            if (error.status === 401) {
              this.router.navigate(['/auth/login']);
            }

          });
    



  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}

class SortData { 
  itemId: number;   
  sortOrder: number;
  pushDownValue: number;
}

class Page {
  pageId: string;
  title: string;
  content: string; 
}


