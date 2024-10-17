import { UserRoles } from "../constants";

export interface IUser {
  idUser:         number;
  username:       string;
  sessionActive:  boolean;
  email:          string;
  status:         boolean;
  firstName:      string;
  middleName:     string;
  firstLastname:  string;
  secondLastname: string;
  idCard:         string;
  birthDate:      Date | string;
}

export interface IUserDTO extends IUser {
  birthDate:      Date | string;
  role:           string;
  lastSession:    Date | string;
}

export interface ICreatedUserDTO extends IUser {
}

export interface INewUserDTO {
  username:       string;
  password:       string;
  firstName:      string;
  middleName:     string;
  firstLastname:  string;
  secondLastname: string;
  idCard:         string;
  birthDate:      Date | string;
  role:           number;
}

export interface IUpdateUserDTO {
  username:       string;
  firstName:      string;
  middleName:     string;
  firstLastname:  string;
  secondLastname: string;
  idCard:         string;
  birthDate:      Date | string;
  rol:            number
}

export interface IUpdatePasswordDTO {
  currentPassword: string,
  newPassword: string,
  repeatPassword: string
}

export interface IUserItemDTO {
  idUser: number;
  username: string;
  sessionActive: boolean;
  email: string;
  status: boolean;
  firstName: string;
  middleName: string;
  firstLastname: string;
  secondLastname: string;
  idCard: string;
  idSession: number;
  birthDate: Date | string;
  startDate: Date | string;
  roleName: string;
}
