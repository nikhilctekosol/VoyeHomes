import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { NbDialogService, NbComponentStatus, NbToastrService} from '@nebular/theme';



@Component({
  selector: 'destinations',
  templateUrl: './destinations.component.html',
  styleUrls: ['./destinations.component.scss'],
})


export class DestinationsComponent implements OnInit  {

 
  isLoading = false;
  loadingSave = false; 
  
  destinations: any[];
  token: any;   
  destination = new Destination();
  dialogRef: any;

  constructor(private http: HttpClient, private authService: NbAuthService, 
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {

  
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

      
    });
   
  }

  ngOnInit() {

    this.loadDestinations();
  
  }



  private loadDestinations() {

    this.isLoading = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/destination/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.destinations = res.data;
           
          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/destination/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  
  }

  openDestinationNew(destinationDialogNew: TemplateRef<any>) {

    this.destination = new Destination();
    this.dialogRef = this.dialogService.open(
      destinationDialogNew,
      { context: { title: 'Add Destination' } });


  }

  openDestination(destinationDialog: TemplateRef<any>, id) {

    let ct = this.destinations.find(_ct => _ct.id == id);

    this.destination = new Destination();

    this.destination.id = id;
    
    this.destination.title = ct.title;
    this.destination.thumbnail = ct.thumbnail;
    this.destination.description = ct.description;

   

    this.dialogRef = this.dialogService.open(
      destinationDialog,
      { context: { title: 'Update Destination' } });//53211/


  }

  openDestinationDelete(deleteDestinationDialog: TemplateRef<any>, id) {

    this.destination = new Destination();

    this.destination.id = id;

    this.dialogRef = this.dialogService.open(
      deleteDestinationDialog,
      { context: { title: 'Delete Destination' } });
  }

  onNewDestinationSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    
    this.http.post('api/destination/create'
      , this.destination, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadDestinations();
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

          console.log('api/destination/create', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onDestinationSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/destination/update?id=' + this.destination.id
      , this.destination, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadDestinations();
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

          console.log('api/destination/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  deleteDestination() {


    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/destination/delete?id=' + this.destination.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Destination deleted successfully!', 'success');
          this.dialogRef.close();
          this.loadDestinations();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/destination/update', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}



class Destination {
  id: number;
  title: string;
  description: string;
  thumbnail: string;
 
}


