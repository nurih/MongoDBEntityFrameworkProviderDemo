#define MOVIES
#define PEEPS
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;



#region Connection string
var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");

Console.WriteLine($"Connecting using {connectionString}");
ArgumentNullException.ThrowIfNull(connectionString, "connectionStinr is null. You must set your 'MONGODB_URI' environment variable.");

#endregion

#if MOVIES

#region EF DbContext with MongoDB

var movieContext = MongoDbContext<Movie>.Create(connectionString);

var personContext = MongoDbContext<Person>.Create(connectionString);

var pocoContext = MongoDbContext<Poco>.Create(connectionString);


#endregion

#region Annotate with EF

pocoContext.Documents.Add(new Poco { Id = Guid.NewGuid() });
pocoContext.SaveChanges();
// look at MDB document - property names mapped by EF are 'Id'=> '_id', 'SomeDate' => 'ef_dt'
#endregion

#region movies demo
Movie? movie = movieContext.Documents.FirstOrDefault(m => m.title == "Back to the Future");

Console.WriteLine($"Movie [{movie?.Id}] [{movie?.plot}]");

var added = movieContext.Documents.Add(new Movie
{
    // no Id provided... let frameworks below do it.
    title = "Twists" + DateTime.Now.ISO8601(),
    plot = "Twist!",
    rated = "APPROVED",
    year = 2001
});

movieContext.SaveChanges(false);

Console.WriteLine($"Movie created {added.Entity.Id}");

IQueryable<Movie> filteredResults = movieContext.Documents
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

personContext.Documents.Remove(person);
personContext.SaveChanges();

var entityEntry = personContext.Documents.Add(person);

Console.WriteLine($"Person {entityEntry.Entity.Id} - {entityEntry.State}");

personContext.SaveChanges();

Console.WriteLine($"Person {entityEntry.Entity.Id} - {entityEntry.State}");

var foundOne = personContext.Documents.FirstOrDefault(p => p.Id == "li@mo.rw");

Console.WriteLine($"Found {foundOne}");

foundOne.Name.First = $"Jack {DateTime.Now.ISO8601()}";

personContext.SaveChanges();
Console.WriteLine($"{foundOne.Id} should now have Name.First {foundOne.Name.First}");

try
{

    foundOne = personContext.Documents.FirstOrDefault(p => p.Name.First == "Jack");
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

    Console.WriteLine("Extra round trips?");

    var entityEntry = personContext.Attach(new Person { Id = "newly@example.com" });
    Console.WriteLine($"Person {entityEntry.Entity.Id} - {entityEntry.State}");
    Console.WriteLine("Attached blindly with Id");
    Console.WriteLine($"Person {entityEntry.Entity.Id} - {entityEntry.State}");
    entityEntry.Entity.Age = 42;
    Console.WriteLine("Changed one property");
    Console.WriteLine($"Person {entityEntry.Entity.Id} - {entityEntry.State}");

    personContext.SaveChanges();
    Console.WriteLine("Saved");
    Console.WriteLine($"Person {entityEntry.Entity.Id} - {entityEntry.State}");
}



#endif