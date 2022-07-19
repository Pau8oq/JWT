export class User{
    id: number;
    username: string;
    email: string;
    accessToken: string;
    refreshToken: string
    
    constructor(id: number, username: string, email: string, accessToken: string, refreshToken: string){
        this.email = email;
        this.id = id;
        this.username = username;
        this.accessToken= accessToken;
        this.refreshToken = refreshToken;
    }
}