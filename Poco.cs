using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;

public class Poco
{
  public Guid Id { get; set; }


  [Column("ef_dt")]
  public DateTime SomeDate { get; set; } = DateTime.Now;

  [BsonElement("dt")]
  public DateTime SomeOtherDate { get; set; } = DateTime.Now;
}