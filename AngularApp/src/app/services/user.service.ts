import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { User } from "../models/user";

@Injectable({providedIn: 'root'})
export class UserService{

    constructor(private httpClient: HttpClient)
    {}

    register(name: string, email: string, password: string){
        return this.httpClient.post("https://localhost:7265/api/Authentication/register",{name, email, password});
    }   

    getAll() {
        return this.httpClient.get("https://localhost:7265/api/Authentication/get-all")
        .pipe(map(data=>{
                if(Array.isArray(data)){
                    return data.map(item=> {
                        let user:User = {
                            email: item.email, 
                            id: item.id, 
                            username: item.userName, 
                            accessToken: item.jwtToken,
                            refreshToken: item.refreshToken
                        }
                        return user;
                    });
                }
                else{
                    return null;
                }
        }));
    }
}
