import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { NbDialogService, NbComponentStatus, NbToastrService} from '@nebular/theme';



@Component({
  selector: 'users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss'],
})


export class UsersComponent implements OnInit  {

 
  isLoading = false;
  loadingSave = false; 
  users: any[];  
  token: any;   
  user = new User();
  dialogRef: any;

  constructor(private http: HttpClient, private authService: NbAuthService, 
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {

  
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

      
    });
   
  }

  ngOnInit() {

    this.loadUsers();
   
  
  }


  private loadUsers() {

    this.isLoading = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/user/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.users = res.data;
           
          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/user/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  
  }

  openUserNew(userDialogNew: TemplateRef<any>) {

    this.user = new User();
    this.dialogRef = this.dialogService.open(
      userDialogNew,
      { context: { title: 'Add User' } });


  }

  openUser(userDialog: TemplateRef<any>, id) {

    let ct = this.users.find(_ct => _ct.id == id);

    this.user = new User();

    this.user.id = id;
    
    this.user.userName = ct.userName;
    this.user.nameOfUser = ct.nameOfUser;
    this.user.userRole = ct.userRole;

    this.dialogRef = this.dialogService.open(
      userDialog,
      { context: { title: 'Update User' } });


  }

  openUserDelete(deleteUserDialog: TemplateRef<any>, id) {

    this.user = new User();

    this.user.id = id;

    this.dialogRef = this.dialogService.open(
      deleteUserDialog,
      { context: { title: 'Delete User' } });
  }

  onNewUserSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

        
    this.http.post('api/user/create'
      , this.user, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadUsers();
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

          console.log('api/user/create', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onUserSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/user/update?id=' + this.user.id
      , this.user, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadUsers();
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

          console.log('api/user/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  deleteUser() {


    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/user/delete?id=' + this.user.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'User deleted successfully!', 'success');
          this.dialogRef.close();
          this.loadUsers();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/user/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}



class User {
  id: number;
  userName: string;
  nameOfUser: string;
  userRole: string;
}


