import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { first } from "rxjs";
import { UserService } from "../services/user.service";

@Component({
    selector: 'register',
    templateUrl: './register.component.html'
})
export class RegisterComponent{

    name: string = "";
    email: string = "";
    password: string = "";

    constructor(private router: Router, private userService: UserService) {
    }

    register() {

        this.userService.register(this.name, this.email, this.password)
        .pipe(first())
        .subscribe(data=>{
            this.router.navigate(['/home']);
        })
    }
}