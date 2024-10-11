export interface IUserLoginDTO {
  username: string,
  password: string
}

export interface ILoginSuccess {
  // token: string,
  success: boolean
}

export interface IAuthenticated {
  isAuthenticated: boolean
}
