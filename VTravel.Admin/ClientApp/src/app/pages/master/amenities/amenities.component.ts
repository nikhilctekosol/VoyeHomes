import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { NbDialogService, NbComponentStatus, NbToastrService} from '@nebular/theme';



@Component({
  selector: 'amenities',
  templateUrl: './amenities.component.html',
  styleUrls: ['./amenities.component.scss'],
})


export class AmenitiesComponent implements OnInit  {

 
  isLoading = false;
  loadingSave = false; 
  amenities: any[]; 
  token: any;   
  amenity = new Amenity();
  dialogRef: any;

  constructor(private http: HttpClient, private authService: NbAuthService, 
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {

  
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

      
    });
   
  }

  ngOnInit() {
    this.loadAmenities();
   
  }

  private loadAmenities() {

    this.isLoading = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/amenity/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.amenities = res.data;
           
          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/amenity/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  
  }

  openAmenityNew(amenityDialogNew: TemplateRef<any>) {

    this.amenity = new Amenity();
    this.dialogRef = this.dialogService.open(
      amenityDialogNew,
      { context: { title: 'Add Amenity' } });


  }

  openAmenity(amenityDialog: TemplateRef<any>, id) {

    let am = this.amenities.find(_am => _am.id == id);

    this.amenity = new Amenity();

    this.amenity.id = id;
    
    this.amenity.amenityName = am.amenityName;
    this.amenity.image1 = am.image1;
    this.dialogRef = this.dialogService.open(
      amenityDialog,
      { context: { title: 'Update Amenity' } });


  }

  openAmenityDelete(deleteAmenityDialog: TemplateRef<any>, id) {

    this.amenity = new Amenity();

    this.amenity.id = id;

    this.dialogRef = this.dialogService.open(
      deleteAmenityDialog,
      { context: { title: 'Delete Amenity' } });
  }

  onNewAmenitySubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    
    this.http.post('api/amenity/create'
      , this.amenity, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadAmenities();
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

          console.log('api/amenity/create', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onAmenitySubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/amenity/update?id=' + this.amenity.id
      , this.amenity, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadAmenities();
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

          console.log('api/amenity/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  deleteAmenity() {


    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/amenity/delete?id=' + this.amenity.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Amenity deleted successfully!', 'success');
          this.dialogRef.close();
          this.loadAmenities();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/amenity/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}



class Amenity {
  id: number;
  amenityName: string;
  image1: string; 
}


