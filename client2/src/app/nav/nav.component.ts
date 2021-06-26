import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_Services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};

  constructor(public accountService: AccountService, private route: Router
    ,private toastr: ToastrService) { 
  }

  ngOnInit(): void {
  }

  login() {
    this.accountService.login(this.model).subscribe(responce => {
      this.route.navigateByUrl('/members');
    }, error => {
      this.toastr.error(error);
      this.toastr.success("Ninja");
      this.toastr.warning("Dengai");
    });
  }
  logout() {
    this.accountService.logout();
    this.route.navigateByUrl('/');
  }

}
