using MongoDB.Bson;

internal class Movie
{
    public string Id { get; set; }
    public string title { get; set; }
    public string rated { get; set; }
    public string plot { get; set; }

    public int year { get; set; }
}
