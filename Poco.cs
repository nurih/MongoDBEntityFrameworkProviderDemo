using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;

public class Poco
{

  public Guid Id { get; set; }


  [Column("ef_dt")]
  public DateTime? SomeDate { get; set; }

  [BsonElement("dt")]
  public DateTime? SomeOtherDate { get; set; }

  // [BsonRepresentation(MongoDB.Bson.BsonType.String)] Not working.
  public PocoStatus Status { get; set; }
}

public enum PocoStatus
{
  Initial = 0,
  Interstitial,
  Final

}