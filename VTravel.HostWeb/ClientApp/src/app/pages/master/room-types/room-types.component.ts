import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { NbDialogService, NbComponentStatus, NbToastrService} from '@nebular/theme';



@Component({
  selector: 'tags',
  templateUrl: './room-types.component.html',
  styleUrls: ['./room-types.component.scss'],
})


export class RoomTypesComponent implements OnInit  {

 
  isLoading = false;
  loadingSave = false; 
  types: any[]; 
  token: any;   
  roomType = new RoomType();
  dialogRef: any;

  constructor(private http: HttpClient, private authService: NbAuthService, 
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {

  
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

      
    });
   
  }

  ngOnInit() {
    this.loadRoomTypes();
   
  }

  private loadRoomTypes() {

    this.isLoading = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/roomtype/get-list'
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
          console.log('api/roomtype/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  
  }

  openRoomTypeNew(tagDialogNew: TemplateRef<any>) {

    this.roomType = new RoomType();
    this.dialogRef = this.dialogService.open(
      tagDialogNew,
      { context: { title: 'Add RoomType' } });


  }

  openRoomType(tagDialog: TemplateRef<any>, id) {

    let tg = this.types.find(_tg => _tg.id == id);

    this.roomType = new RoomType();

    this.roomType.id = id;
    
    this.roomType.typeName = tg.typeName;

    this.dialogRef = this.dialogService.open(
      tagDialog,
      { context: { title: 'Update RoomType' } });


  }

  openRoomTypeDelete(deleteRoomTypeDialog: TemplateRef<any>, id) {

    this.roomType = new RoomType();

    this.roomType.id = id;

    this.dialogRef = this.dialogService.open(
      deleteRoomTypeDialog,
      { context: { title: 'Delete RoomType' } });
  }

  onNewRoomTypeSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    
    this.http.post('api/roomtype/create'
      , this.roomType, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadRoomTypes();
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

          console.log('api/roomType/create', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onRoomTypeSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/roomtype/update?id=' + this.roomType.id
      , this.roomType, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadRoomTypes();
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

          console.log('api/roomType/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  deleteRoomType() {


    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/roomtype/delete?id=' + this.roomType.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'RoomType deleted successfully!', 'success');
          this.dialogRef.close();
          this.loadRoomTypes();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/roomType/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}



class RoomType {
  id: number;
  typeName: string; 
}


