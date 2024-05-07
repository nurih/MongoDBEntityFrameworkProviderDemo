using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;

public class Poco
{

  [BsonId] // designate field as _id
  public Guid Marklar { get; set; }


  [Column("ef_dt")]
  public DateTime? SomeDate { get; set; }

  [BsonElement("dt")]
  public DateTime? SomeOtherDate { get; set; }

  // [BsonRepresentation(MongoDB.Bson.BsonType.String)] //Throws
  public PocoStatus Status { get; set; }

  public override string ToString()
  {
    return $"[{Marklar}]: {SomeDate}, {SomeOtherDate} {Status}";
  }
}
