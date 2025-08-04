export interface TokenUser {
    userId: string;
    email: string;
    username: string;
}

export interface AuthModel {
    email: string;
    password: string;
}

export interface RegistrationModel {
    username: string;
    email: string;
    password: string;
}

// ***********************
// API models
// ***********************

export interface User {
    id: string;
    userName: string;
    email: string;
}

