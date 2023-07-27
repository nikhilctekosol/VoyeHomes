import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { NbDialogService, NbComponentStatus, NbToastrService} from '@nebular/theme';



@Component({
  selector: 'tags',
  templateUrl: './property-types.component.html',
  styleUrls: ['./property-types.component.scss'],
})


export class PropertyTypesComponent implements OnInit  {

 
  isLoading = false;
  loadingSave = false; 
  types: any[]; 
  token: any;   
  propertyType = new PropertyType();
  dialogRef: any;

  constructor(private http: HttpClient, private authService: NbAuthService, 
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {

  
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

      
    });
   
  }

  ngOnInit() {
    this.loadPropertyTypes();
   
  }

  private loadPropertyTypes() {

    this.isLoading = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/propertytype/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.types = res.data;
           
          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/propertytype/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  
  }

  openPropertyTypeNew(tagDialogNew: TemplateRef<any>) {

    this.propertyType = new PropertyType();
    this.dialogRef = this.dialogService.open(
      tagDialogNew,
      { context: { title: 'Add PropertyType' } });


  }

  openPropertyType(tagDialog: TemplateRef<any>, id) {

    let tg = this.types.find(_tg => _tg.id == id);

    this.propertyType = new PropertyType();

    this.propertyType.id = id;
    
    this.propertyType.typeName = tg.typeName;

    this.dialogRef = this.dialogService.open(
      tagDialog,
      { context: { title: 'Update PropertyType' } });


  }

  openPropertyTypeDelete(deletePropertyTypeDialog: TemplateRef<any>, id) {

    this.propertyType = new PropertyType();

    this.propertyType.id = id;

    this.dialogRef = this.dialogService.open(
      deletePropertyTypeDialog,
      { context: { title: 'Delete PropertyType' } });
  }

  onNewPropertyTypeSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    
    this.http.post('api/propertytype/create'
      , this.propertyType, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadPropertyTypes();
          this.loadingSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.dialogRef.close();
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/propertyType/create', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onPropertyTypeSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/propertytype/update?id=' + this.propertyType.id
      , this.propertyType, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadPropertyTypes();
          this.loadingSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.dialogRef.close();
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/propertyType/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  deletePropertyType() {


    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/propertytype/delete?id=' + this.propertyType.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'PropertyType deleted successfully!', 'success');
          this.dialogRef.close();
          this.loadPropertyTypes();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/propertyType/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}



class PropertyType {
  id: number;
  typeName: string; 
}


