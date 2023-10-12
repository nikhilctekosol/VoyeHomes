import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { NbDialogService, NbComponentStatus, NbToastrService} from '@nebular/theme';



@Component({
  selector: 'properties',
  templateUrl: './properties.component.html',
  styleUrls: ['./properties.component.scss'],
})


export class PropertiesComponent implements OnInit  {

 
  isLoading = false;
  loadingSave = false;
  isAlert = false;
  alertMessage = '';
  alertStatus = '';
  properties: any[];
  propertiessusp: any[];
  propertiesActive: any[];
  propertiesInactive: any[];
  stores: any[];
  propertyTypes: any[];
  subCategories: any[];
  token: any; 
  searchValue: string;
  property = new Property();
  dialogRef: any;

  constructor(private http: HttpClient, private authService: NbAuthService, 
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {

  
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

      
    });
   
  }

  ngOnInit() {
    this.loadProperties();
    this.loadSuspendedProperties();
    this.loadPropertyTypes();
  }

  propertySearch() {
    if (this.searchValue != null) {

      this.properties = this.propertiesActive.filter((item) => {
        return item.title.toLowerCase().indexOf(this.searchValue.toLowerCase()) != -1;
      });

    } else {
      this.properties = this.propertiesActive;
    }

  }

  propertySuspendedSearch() {
    if (this.searchValue != null) {

      this.propertiessusp = this.propertiesInactive.filter((item) => {
        return item.title.toLowerCase().indexOf(this.searchValue.toLowerCase()) != -1;
      });

    } else {
      this.propertiessusp = this.propertiesInactive;
    }

  }

  private loadProperties() {

    this.loadingSave = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-active-list'
      , { headers: headers }).subscribe((res: any) => {

        this.loadingSave = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.propertiesActive = res.data;
            this.propertySearch();
          }
        }

      },
        error => {
          this.loadingSave = false;
          console.log('api/property/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  
  }

  private loadSuspendedProperties() {

    this.loadingSave = true ;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-inactive-list'
      , { headers: headers }).subscribe((res: any) => {

        this.loadingSave = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.propertiesInactive = res.data;
            this.propertySuspendedSearch();
          }
        }

      },
        error => {
          this.loadingSave = false;
          console.log('api/property/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  private loadPropertyTypes() {

    this.loadingSave = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/propertytype/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.loadingSave = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.propertyTypes = res.data;
          
          }
        }

      },
        error => {
          this.loadingSave = false;
          console.log('api/propertytype/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }
  

  loadProperty(id) {
    //this.router.navigate(['../property', id]);
    this.router.navigate(['/pages/catalog/property', id]);
  }

  

  drop(event: CdkDragDrop<string[]>, mode) {

    if (mode == 1) {
      var property = this.properties[event.previousIndex];
      var propertyReplace = this.properties[event.currentIndex];

      moveItemInArray(this.properties, event.previousIndex, event.currentIndex);
    }
    else {
      var property = this.propertiessusp[event.previousIndex];
      var propertyReplace = this.propertiessusp[event.currentIndex];

      moveItemInArray(this.propertiessusp, event.previousIndex, event.currentIndex);
    }

      // sort in backend

      var sortData = new SortData();
     
      sortData.itemId = property.id;
      sortData.sortOrder = propertyReplace.sortOrder;
      sortData.pushDownValue = property.sortOrder >= propertyReplace.sortOrder ? 1 : -1;

      let headers = new HttpHeaders().set("Authorization", "Bearer " +
        this.token).set("Content-Type", "application/json");

      //console.log(JSON.stringify(sortData));

      this.http.post('api/property/sort'
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

            console.log('api/catalog/sort-property', error);
            if (error.status === 401) {
              this.router.navigate(['auth/login']);
            }

          });

    
  }


  openAddProperty(addPropertyDialog: TemplateRef<any>) {

    this.dialogRef= this.dialogService.open(
      addPropertyDialog,
      { context: { title: 'Add Property' } });

  }

  onFormSubmit() {


   
      this.loadingSave = true;
     
      let headers = new HttpHeaders().set("Authorization", "Bearer " +
        this.token).set("Content-Type", "application/json");

    this.http.post('api/property/create'
      , this.property, { headers: headers }).subscribe((res: any) => {
          this.loadingSave = false;
          
          if (res.actionStatus === 'SUCCESS') {

            this.dialogRef.close();

            this.loadingSave = false;
            this.toast('Success', 'Data saved successfully!', 'success');
            this.router.navigate(['/pages/catalog/property', res.data.id]);
          }
          else {

            this.loadingSave = false;
            this.toast('Error', 'Could not save data!', 'danger');
            
          }

        },
          error => {

            this.loadingSave = false;
            this.toast('Error', 'Could not save data!', 'danger');

            console.log('api/property/create', error);
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

class Property {
  propertyTypeId: string;
  title: string; 
}


