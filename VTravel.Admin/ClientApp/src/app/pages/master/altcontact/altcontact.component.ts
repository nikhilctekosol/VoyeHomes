import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { NbDialogService, NbComponentStatus, NbToastrService } from '@nebular/theme';

@Component({
  selector: 'ngx-altcontact',
  templateUrl: './altcontact.component.html',
  styleUrls: ['./altcontact.component.scss']
})
export class AltcontactComponent implements OnInit {

  isLoading = false;
  loadingSave = false;
  contactlist: any[] = [];
  token: any;
  contact = new Contact();
  dialogRef: any;
  constructor(private http: HttpClient, private authService: NbAuthService,
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {

    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();
    });
  }

  ngOnInit(): void {
    this.loadContacts();
  }

  loadContacts() {
    this.isLoading = true ;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/contact/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.contactlist = res.data;
          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/contact/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  openContactNew(contactDialogNew: TemplateRef<any>) {
    this.contact = new Contact();
    this.contact.id = 0;
    this.dialogRef = this.dialogService.open(
      contactDialogNew,
      { context: { title: 'Add Contact' } });
  }

  onContactNewSubmit() {
    this.loadingSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.post('api/contact/create'
      , this.contact, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {
          this.loadContacts();
          this.loadingSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.dialogRef.close();
          this.contact = new Contact();
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

        }

      },
        error => {
          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/contact/create', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  openContact(contactDialog: TemplateRef<any>, id) {

    let c = this.contactlist.find(_c => _c.id == id);

    this.contact = new Contact();

    this.contact.id = id;
    this.contact.name = c.name;
    this.contact.contactno = c.contactno;

    this.dialogRef = this.dialogService.open(
      contactDialog,
      { context: { title: 'Update Contact' } });


  }

  onContactSubmit() {
    this.loadingSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.post('api/contact/update'
      , this.contact, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {
          this.loadContacts();
          this.loadingSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.dialogRef.close();
          this.contact = new Contact();
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

        }

      },
        error => {
          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/contact/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  openDContactDelete(deleteContactDialog: TemplateRef<any>, id) {

    this.contact = new Contact();

    this.contact.id = id;

    this.dialogRef = this.dialogService.open(
      deleteContactDialog,
      { context: { title: 'De-activate Contact' } });
  }

  openAContactDelete(deleteContactDialog: TemplateRef<any>, id) {

    this.contact = new Contact();

    this.contact.id = id;

    this.dialogRef = this.dialogService.open(
      deleteContactDialog,
      { context: { title: 'Activate Contact' } });
  }

  deleteContact(mode) {
    this.loadingSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/contact/delete?id=' + this.contact.id + '&&mode=' + mode
      , { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Contact de-activated successfully!', 'success');
          this.dialogRef.close();
          this.loadContacts();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/contact/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });
  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}

class Contact {
  id: number;
  name: string ;
  contactno: string ;
  isactive: string ;
}
