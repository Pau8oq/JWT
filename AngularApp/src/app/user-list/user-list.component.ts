import { Component } from "@angular/core";
import { User } from "../models/user";
import { UserService } from "../services/user.service";

@Component({
    selector: 'user-list',
    templateUrl: './user-list.component.html'
})
export class UserListComponent{

    public users: User[] | null = null;
    
    constructor(private userService: UserService) {
        this.userService.getAll().subscribe(x=> this.users = x);
    }
}