export interface IUser {

}

export interface IUserDTO {
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
  role:           string;
  lastSession:    Date | string;
}
