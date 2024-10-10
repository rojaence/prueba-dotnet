namespace backend.Models;

public class Session 
{
  public int IdSession { get; set; }
  public int IdUser { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime? EndDate { get; set; }
}
