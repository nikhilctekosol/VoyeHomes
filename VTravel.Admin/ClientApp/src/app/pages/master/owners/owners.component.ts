import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { NbDialogService, NbComponentStatus, NbToastrService } from '@nebular/theme';

@Component({
  selector: 'ngx-owners',
  templateUrl: './owners.component.html',
  styleUrls: ['./owners.component.scss']
})
export class OwnersComponent implements OnInit {

  isLoading = false;
  loadingSave = false;
  owners: any[];
  token: any;
  owner = new Owner();
  dialogRef: any;
  constructor(private http: HttpClient, private authService: NbAuthService,
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {


    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();


    });
  }

  ngOnInit(): void {
    this.loadOwners();
  }

  private loadOwners() {

    this.isLoading = true ;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/owner/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.owners = res.data;

          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/owner/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  openOwnerNew(ownerDialogNew: TemplateRef<any>) {

    this.owner = new Owner();
    this.owner.id = 0;
    this.dialogRef = this.dialogService.open(
      ownerDialogNew,
      { context: { title: 'Add Owner' } });


  }

  onNewOwnerSubmit() {

    this.loadingSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.post('api/owner/create'
      , this.owner, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadOwners();
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

          console.log('api/owner/create', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  openOwner(ownerDialog: TemplateRef<any>, id) {

    let own = this.owners.find(_own => _own.id == id);

    this.owner = new Owner();

    this.owner.id = id;
    this.owner.name = own.name;
    this.owner.address = own.address;
    this.owner.gstno = own.gstno;
    this.owner.pan = own.pan;
    this.owner.ownershiptype = own.ownershiptype;

    this.dialogRef = this.dialogService.open(
      ownerDialog,
      { context: { title: 'Update Owner' } });


  }

  onOwnerSubmit() {

    this.loadingSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/owner/update'
      , this.owner, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadOwners();
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

          console.log('api/owner/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  openOwnerDelete(deleteOwnerDialog: TemplateRef<any>, id) {

    this.owner = new Owner();

    this.owner.id = id;

    this.dialogRef = this.dialogService.open(
      deleteOwnerDialog,
      { context: { title: 'Delete Owner' } });
  }

  deleteOwner() {


    this.loadingSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/owner/delete?id=' + this.owner.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Owner deleted successfully!', 'success');
          this.dialogRef.close();
          this.loadOwners();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/owner/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  validateGST(): boolean {
    const gstPattern = /^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}[Z]{1}[0-9A-Z]{1}$/;

    if (this.owner.gstno && !gstPattern.test(this.owner.gstno)) {
      this.toast('Error', 'Enter a valid GST Number!', 'danger');
      this.owner.gstno = '';
      return false;
    }

    // All checks passed, the dates are valid
    return true ;
  }

  validatePAN(): boolean {
    const panPattern = /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/;

    if (this.owner.pan && !panPattern.test(this.owner.pan)) {
      this.toast('Error', 'Enter a valid PAN!', 'danger');
      this.owner.pan = '';
      return false;
    }

    // All checks passed, the dates are valid
    return true ;
  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}
class Owner {
  id: number;
  name: string ;
  address: string ;
  gstno: string ;
  pan: string ;
  ownershiptype: string ;
}
