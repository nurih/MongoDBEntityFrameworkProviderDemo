

public class Person
{
  public required string Id { get; set; }
  public Name? Name { get; set; }
  public int Age { get; set; } = 33;
  public override string ToString() => $"{Id} {Name} - {Age}";
}

public class Name
{
  public required string First { get; set; }
  public string? Last { get; set; }

  
  public override string ToString() => $"({First}, {Last})";

}