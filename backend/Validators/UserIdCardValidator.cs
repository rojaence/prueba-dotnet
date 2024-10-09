using System.ComponentModel.DataAnnotations;

public class UserIdCardValidator : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var idCard = value as string;
        if (string.IsNullOrEmpty(idCard))
        {
            return new ValidationResult("La identificacion es obligatorio.");
        }

        // Comprobar si hay 4 números iguales consecutivos
        for (int i = 0; i < idCard.Length - 3; i++)
        {
            if (idCard[i] == idCard[i + 1] && idCard[i] == idCard[i + 2] && idCard[i] == idCard[i + 3])
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        return ValidationResult.Success;
    }
}
