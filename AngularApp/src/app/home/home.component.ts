import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { User } from "../models/user";
import { AuthenticationService } from "../services/authentication.service";
import { BehaviorSubject, catchError,  take ,filter,  Observable, switchMap, throwError } from "rxjs";
import { HttpStatusCode } from "@angular/common/http";

@Component({
    selector: 'home',
    templateUrl: "./home.component.html"
})
export class HomeComponent{

    userLogged = false;
    
    constructor(private router: Router, private authenticationService: AuthenticationService){
       this.authenticationService.currentUser.subscribe(s=> this.userLogged = s != null);
    }

    logout(){
        this.authenticationService.logout();
    }
}