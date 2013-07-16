namespace Server.DomainObjects
{
  public class SystemElement
  {
    public long Id { get; set; }

    public string Name { get; set; }

    public long ParentSystemId { get; set; } 
  }
}