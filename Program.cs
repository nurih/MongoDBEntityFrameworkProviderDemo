#define POCO
#define MOVIES
#define PEEPS

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;



#region Connection string
var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");

Console.WriteLine($"Connecting using {connectionString}");
ArgumentNullException.ThrowIfNull(connectionString, "connectionStinr is null. You must set your 'MONGODB_URI' environment variable.");

#endregion


#region Conventions and mapping
{ // not working
    BsonClassMap.RegisterClassMap<Poco>(classMap =>
    {
        classMap.AutoMap();
        classMap.MapMember(m => m.Status).SetElementName("TheStatus")
            .SetSerializer(new EnumSerializer<PocoStatus>(BsonType.String));
    });
}

{
    // Not working
    ConventionRegistry.Register(
        "EnumsAsStrings",
        new ConventionPack{
            new EnumRepresentationConvention(BsonType.String)
        },
        t =>
        {
            Console.WriteLine($"{t.Name}: {t.GetType().IsEnum} / {t.IsEnum}");
            return t.GetType().IsEnum || t.IsEnum || t.Name == "PocoStatus";
        }
    );


}
#endregion


#region EF DbContext with MongoDB

var movieContext = MongoDbContext<Movie>.Create(connectionString);

var personContext = MongoDbContext<Person>.Create(connectionString);

var pocoContext = MongoDbContext<Poco>.Create(connectionString);


#endregion

#if POCO

#region Annotate with EF property attributes

Console.WriteLine("Poco / properties / serialization");

pocoContext.Entities.Add(new Poco
{
    Marklar = Guid.NewGuid(),
    SomeDate = DateTime.Now,
    SomeOtherDate = DateTime.Now,
    Status = PocoStatus.Final
});

pocoContext.SaveChanges();
// look at MDB document - property names mapped by EF are 'Id'=> '_id', 'SomeDate' => 'ef_dt'



#endregion

#region projection
var projected = pocoContext.Entities.AsQueryable();//.Select(p => new Poco() { Id = p.Id });
foreach (var d in projected)
{
    Console.WriteLine(d.ToJson().ToString());
}


#endregion

#endif

#if MOVIES

#region movies demo
Movie? movie = movieContext.Entities.FirstOrDefault(m => m.title == "Back to the Future");

Console.WriteLine($"Movie [{movie?.Id}] [{movie?.plot}]");

var added = movieContext.Entities.Add(new Movie
{
    // no Id provided... let frameworks below do it.

    title = "Twists" + DateTime.Now.ISO8601(),
    plot = "Twist!",
    rated = "APPROVED",
    year = 2001
});

movieContext.SaveChanges(false);

Console.WriteLine($"Movie created {added.Entity.Id}");

IQueryable<Movie> filteredResults = movieContext.Entities
    .Where(m => m.year < 1916)
    .OrderBy(m => m.title)
    .ThenBy(m => m.year)
    .Take(3);

foreach (var m in filteredResults)
{
    Console.WriteLine($"{m.Id}: {m.title} ({m.year})");
}

#endregion

#endif

#if PEEPS




var person = new Person
{
    Id = "bob@bob.bob",
    Name = new Name { First = "bob", Last = "bob" }
};

personContext.Entities.Remove(person);
personContext.SaveChanges();

var entityEntry = personContext.Entities.Add(person);

Console.WriteLine($"Person {entityEntry.Entity.Id} - {entityEntry.State}");

personContext.SaveChanges();

Console.WriteLine($"Person {entityEntry.Entity.Id} - {entityEntry.State}");

var foundOne = personContext.Entities.First(p => p.Id == entityEntry.Entity.Id);

Console.WriteLine($"Found {foundOne}");

foundOne.Name.First = $"Jack {DateTime.Now.ISO8601()}";

personContext.SaveChanges();
Console.WriteLine($"{foundOne.Id} should now have 'Name.First' = {foundOne.Name.First}");

try
{

    foundOne = personContext.Entities.FirstOrDefault(p => p.Name.First == "Jack");
}
catch (InvalidOperationException e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine("^^^ Doesn't handle sub-document query (yet..)");
}


TryInsertMany(MongoDbContext<Person>.Create(connectionString));

TryBlindUpdate(MongoDbContext<Person>.Create(connectionString));

void TryInsertMany(MongoDbContext<Person> personContext)
{


    IEnumerable<Person> newOnes = [new() { Id = "kim@kim.kim" }, new() { Id = "ogg@ogg.ogg" }];

    try
    {
        personContext.AddRange(newOnes);
        personContext.SaveChanges();
    }
    catch (MongoBulkWriteException ble)
    {
        Console.WriteLine(ble.Message);
    }
}

void TryBlindUpdate(MongoDbContext<Person> personContext)
{
    const string SOME_ID = "newly@example.com";

    Console.WriteLine("Extra round trips?");

    var entityEntry = personContext.Add(new Person { Id = SOME_ID });

    Console.WriteLine($"Person {entityEntry.Entity.Id} - {entityEntry.State}");
    Console.WriteLine("Attached blindly with Id");
    Console.WriteLine($"Person {entityEntry.Entity.Id} - {entityEntry.State}");
    entityEntry.Entity.Age = 789;
    Console.WriteLine("Changed one property");
    Console.WriteLine($"Person {entityEntry.Entity.Id} - {entityEntry.State}");
    personContext.SaveChanges();

    Console.WriteLine("Saved");
    Console.WriteLine($"Person {entityEntry.Entity.Id} - {entityEntry.State}");

    var savedOne = personContext.Entities.First(p => p.Id == SOME_ID);
    savedOne.Name = new Name() { First = "Addison" };
    personContext.SaveChanges();

}



#endif