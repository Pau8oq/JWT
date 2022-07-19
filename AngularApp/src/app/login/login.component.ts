import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { first } from "rxjs";
import { AuthenticationService } from "../services/authentication.service";

@Component({
    selector: 'login',
    templateUrl: './login.component.html'
})
export class LoginComponent{

    email: string = "";
    password: string = "";

    constructor(private router: Router, private authenticationService: AuthenticationService) {
        if(this.authenticationService.currentUserValue){
            this.router.navigate(['']);
        }
    }

    login() {
        this.authenticationService.login(this.email, this.password)
        .pipe(first())
        .subscribe(data=>{
            console.log(data);
            this.router.navigate(['/home']);
        })
    }
}