import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { NbDialogService, NbComponentStatus, NbToastrService} from '@nebular/theme';



@Component({
  selector: 'tags',
  templateUrl: './booking-channels.component.html',
  styleUrls: ['./booking-channels.component.scss'],
})


export class BookingChannelsComponent implements OnInit  {

 
  isLoading = false;
  loadingSave = false; 
  channels: any[]; 
  token: any;   
  bookingChannel = new BookingChannel();
  dialogRef: any;

  constructor(private http: HttpClient, private authService: NbAuthService, 
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {

  
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

      
    });
   
  }

  ngOnInit() {
    this.loadBookingChannels();
   
  }

  private loadBookingChannels() {

    this.isLoading = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/bookingchannel/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.channels = res.data;
           
          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/bookingchannel/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  
  }

  openBookingChannelNew(tagDialogNew: TemplateRef<any>) {

    this.bookingChannel = new BookingChannel();
    this.dialogRef = this.dialogService.open(
      tagDialogNew,
      { context: { title: 'Add BookingChannel' } });


  }

  openBookingChannel(tagDialog: TemplateRef<any>, id) {

    let tg = this.channels.find(_tg => _tg.id == id);

    this.bookingChannel = new BookingChannel();

    this.bookingChannel.id = id;
    
    this.bookingChannel.channelName = tg.channelName;

    this.dialogRef = this.dialogService.open(
      tagDialog,
      { context: { title: 'Update BookingChannel' } });


  }

  openBookingChannelDelete(deleteBookingChannelDialog: TemplateRef<any>, id) {

    this.bookingChannel = new BookingChannel();

    this.bookingChannel.id = id;

    this.dialogRef = this.dialogService.open(
      deleteBookingChannelDialog,
      { context: { title: 'Delete BookingChannel' } });
  }

  onNewBookingChannelSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    
    this.http.post('api/bookingchannel/create'
      , this.bookingChannel, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadBookingChannels();
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

          console.log('api/bookingChannel/create', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onBookingChannelSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/bookingchannel/update?id=' + this.bookingChannel.id
      , this.bookingChannel, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadBookingChannels();
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

          console.log('api/bookingChannel/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  deleteBookingChannel() {


    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/bookingchannel/delete?id=' + this.bookingChannel.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'BookingChannel deleted successfully!', 'success');
          this.dialogRef.close();
          this.loadBookingChannels();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/bookingChannel/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}



class BookingChannel {
  id: number;
  channelName: string; 
}


