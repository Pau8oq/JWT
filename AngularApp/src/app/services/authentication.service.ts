import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable, Optional } from "@angular/core";
import { BehaviorSubject, catchError, map, Observable } from "rxjs";
import { User } from "../models/user";
import { TokenStorageService } from "./token-storage.services";

const USER_KEY = 'auth-user';

@Injectable({providedIn: 'root'})
export class AuthenticationService{

    private currentUserSubject: BehaviorSubject<User | null>;
    public currentUser: Observable<User | null>;

    constructor(private httpClient: HttpClient, private tokenStorage: TokenStorageService) {
        let user = this.tokenStorage.getUser();

        if(user != null){
            this.currentUserSubject = new BehaviorSubject<User | null>(user);
        }
        else {
            this.currentUserSubject = new BehaviorSubject<User | null>(null);
        }

        this.currentUser = this.currentUserSubject.asObservable();
    }
    
    public get currentUserValue(): User | null{
       return this.currentUserSubject.value;
    }

    login(email: string, password: string){
        return this.httpClient.post<any>('https://localhost:7265/api/Authentication/login', {email, password })
        .pipe(map(user => {

            let t_user: User = {
                email: user.email, 
                id: user.id, 
                username: user.userName, 
                accessToken: user.jwtToken,
                refreshToken: user.refreshToken
            };

            this.tokenStorage.saveUser(t_user);
            this.tokenStorage.saveToken(t_user.accessToken);
            this.tokenStorage.saveRefreshToken(t_user.refreshToken);

            this.currentUserSubject.next(t_user);
            
            return user;
        }));
    }

    refreshToken(user: User){
        let refreshToken = user.refreshToken;

        return this.httpClient.post<any>('https://localhost:7265/api/Authentication/refresh-token', {refreshToken})
        .pipe(map(item => {

            let t_user: User = {
                email: item.email, 
                id: item.id, 
                username: item.userName, 
                accessToken: item.jwtToken,
                refreshToken: item.refreshToken
            };

            this.tokenStorage.saveUser(t_user);
            this.tokenStorage.saveToken(t_user.accessToken);
            this.tokenStorage.saveRefreshToken(t_user.refreshToken);
            
            this.currentUserSubject.next(t_user);
            
            return user;
        }));
    }

    logout(){
        this.tokenStorage.signOut();
        this.currentUserSubject.next(null);
    }

}