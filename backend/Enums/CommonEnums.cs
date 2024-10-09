namespace backend.Enums;

public enum UsernameValidationResult
{
    Success = 1,
    TooShort,
    TooLong,
    Duplicate,
    MissingNumber,
    MissingUppercase,
    ContainsInvalidCharacters
}

public enum UserRoleEnum
{
  Admin = 1,
  User
}

public enum PermissionsEnum
{
  CanViewWelcome = 1,
  CanViewDashboard,
  SearchUsers,
  CanUpdateOtherUserProfiles
}
