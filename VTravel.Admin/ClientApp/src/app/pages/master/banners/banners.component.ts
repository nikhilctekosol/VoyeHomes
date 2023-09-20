import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { NbDialogService, NbComponentStatus, NbToastrService } from '@nebular/theme';

@Component({
  selector: 'ngx-banners',
  templateUrl: './banners.component.html',
  styleUrls: ['./banners.component.scss']
})
export class BannersComponent implements OnInit {

  isLoading = false;
  loadingSave = false;
  bannerlist: any[];
  loadingStatusSave = false;
  loadingDelete = false;
  token: any;
  banner = new BannerList();
  dialogRef: any;
  destinations: any[];
  properties: any[];
  constructor(private http: HttpClient, private authService: NbAuthService, private router: Router
    , private cd: ChangeDetectorRef, private r: ActivatedRoute, private dialogService: NbDialogService
    , private toastrService: NbToastrService) {

    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

    });
  }

  ngOnInit(): void {
    this.loadDestinations();
    this.loadProperties();
    this.loadBanners();
  }

  private loadBanners() {
    this.isLoading = true ;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/banner/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.bannerlist = res.data;
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

  loadDestinations() {
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/destination/get-list'
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.destinations = res.data;
          }
        }
      },
        error => {

          console.log('api/destination/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  loadProperties() {
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-list'
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.properties = res.data;
          }
        }
      },
        error => {

          console.log('api/property/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  openBannerNew(bannerDialogNew: TemplateRef<any>) {

    this.banner = new BannerList();
    this.banner.show_in_home = 'N';
    this.banner.destination_id = '0';
    this.banner.property_id = '0';
    this.dialogRef = this.dialogService.open(
      bannerDialogNew,
      { context: { title: 'Add Banner' } });
  }

  openBanner(bannerDialog: TemplateRef<any>, id) {

    let ct = this.bannerlist.find(_ct => _ct.id == id);

    this.banner = new BannerList();

    this.banner.id = id;
    this.banner.title = ct.title;
    this.banner.description = ct.description;
    this.banner.image_url = ct.image_url;
    this.banner.image_alt = ct.image_alt;
    this.banner.navigate_url = ct.navigate_url;
    this.banner.destination_id = ct.destination_id;
    this.banner.property_id = ct.property_id;
    this.banner.show_in_home = ct.show_in_home;
    this.banner.active = ct.active;


    this.dialogRef = this.dialogService.open(
      bannerDialog,
      { context: { title: 'Update Banner' } });


  }
  onNewBannerSubmit() {

    this.loadingSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.post('api/banner/create'
      , this.banner, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadBanners();
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

          console.log('api/banner/create', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onBannerSubmit() {
    this.loadingSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/banner/update?id=' + this.banner.id
      , this.banner, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadBanners();
          this.loadingSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.dialogRef.close();
          this.loadBanners();
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/banner/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });
  }

  openDelete(deleteDialog: TemplateRef<any>, id) {
    this.banner.id = id;

    this.dialogRef = this.dialogService.open(
      deleteDialog,
      { context: { title: 'De-activate Banner' } });
  }

  deleteBanner() {
    this.loadingDelete = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/banner/delete?id=' + this.banner.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingDelete = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Banner De-activated successfully!', 'success');
          this.dialogRef.close();
          this.router.navigate(['/pages/master/banners']);
        }
        else {
          this.toast('Error', 'Could not de-activate!', 'danger');
        }

      },
        error => {
          this.loadingDelete = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/property/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }

}

class BannerList {
  id: number;
  image_url: string ;
  image_alt: string ;
  title: string ;
  description: string ;
  navigate_url: string ;
  property_id: string ;
  destination_id: string ;
  show_in_home: string ;
  active: string ;

}
