namespace backend.Models;

public class LoginAttempt
{
    public int IdAttempt { get; set; }
    public int IdUser { get; set; }
    public bool Resolved { get; set; }
    public DateTime DateAttempt { get; set; } = DateTime.Now;
}