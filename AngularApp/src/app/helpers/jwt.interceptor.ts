import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpStatusCode } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, catchError,  take ,filter,  Observable, switchMap, throwError } from "rxjs";
import { AuthenticationService } from "../services/authentication.service";

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

    private isRefreshing = false;
    private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);
    
    constructor(private authenticationService: AuthenticationService, private router: Router)
    {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        
        req = this.addAuthenticationTokenIfNeeded(req);

        return next.handle(req).pipe(catchError(err=>{
            if(err.status === HttpStatusCode.Unauthorized && !this.isRefreshing){
                return this.handleUnauthorizedError(req, next);  
            }
            else if(err.status === HttpStatusCode.Unauthorized && this.isRefreshing){
                this.authenticationService.logout();
                this.router.navigate(['/login']);
                return throwError(() => console.log('re-login'));
            }
            else{
                return throwError(() => console.log("error"));
            }
           
        }));
    }

    private handleUnauthorizedError(request: HttpRequest<any>, next: HttpHandler){
        if(!this.isRefreshing){
            this.isRefreshing = true;
            this.refreshTokenSubject.next(null);
            let user = this.authenticationService.currentUserValue;

            if(user){
                return this.authenticationService.refreshToken(user).pipe(
                    switchMap((item:any)=>{
                        this.isRefreshing = false;
                        this.refreshTokenSubject.next(item.JwtToken);
                        return next.handle(this.addAuthenticationTokenIfNeeded(request));
                    })
                )
            }
        }

        return this.refreshTokenSubject.pipe(
            filter(token => token !== null),
            take(1),
            switchMap((token) => next.handle(this.addAuthenticationTokenIfNeeded(request)))
        );
    }

    private addAuthenticationTokenIfNeeded(request: HttpRequest<any>): HttpRequest<any> {
        
        let currentUser =  this.authenticationService.currentUserValue;

        if (!request.url.match(/(https|http):\/\/localhost:7265\//)) {
            return request;
        }

        if(currentUser && currentUser.accessToken){
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${currentUser.accessToken}`
                }
            })
        }

        return request;
      }

}