
using Microsoft.EntityFrameworkCore;

using MongoDB.EntityFrameworkCore.ValueGeneration;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using MongoDB.EntityFrameworkCore.Infrastructure;
using MongoDB.Bson;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ChangeTracking;



internal class MongoDbContext<T> : DbContext where T : class
{
    public DbSet<T> Entities { get; init; }
    public readonly IMongoClient MongoClient;
    public readonly string? DatabaseName;


    public static MongoDbContext<T> Create(string connectionString, Action<MongoDbContextOptionsBuilder>? optionsAction = null)
    {
        var mongoUrl = MongoUrl.Create(connectionString);
        var client = new MongoClient(mongoUrl);
        MongoDbContext<T> result = MongoDbContext<T>.Create(client.GetDatabase((string?)mongoUrl.DatabaseName), optionsAction);
        return result;
    }

    public static MongoDbContext<T> Create(IMongoDatabase database, Action<MongoDbContextOptionsBuilder>? optionsAction = null)
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<MongoDbContext<T>>();
        DbContextOptions<MongoDbContext<T>> dbContextOptions = dbContextOptionsBuilder
                            .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName, optionsAction)
                            .Options;

        MongoDbContext<T> mdbContext = new(dbContextOptions);

        return mdbContext;
    }

    public MongoDbContext(DbContextOptions options) : base(options)
    {
        MongoOptionsExtension mongoOptionsExtension = options.GetExtension<MongoOptionsExtension>();
        ArgumentNullException.ThrowIfNull(mongoOptionsExtension, "mongoOptionsExtension");
        ArgumentNullException.ThrowIfNull(mongoOptionsExtension.MongoClient, "mongoOptionsExtension.MongoClient");

        this.MongoClient = mongoOptionsExtension.MongoClient;
        this.DatabaseName = mongoOptionsExtension.DatabaseName;

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<T>().ToCollection(typeof(T).Name);
        modelBuilder.Entity<Poco>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<PocoStatus>(v)
            );

        modelBuilder.Entity<Movie>().Property(e => e.Id).HasConversion(
            v => ObjectId.Parse(v),
            v => v.ToString()
            );

        modelBuilder.Entity<Movie>()
            .Property(e => e.Id)
            .HasValueGenerator<MyOwnValueGenerator>();
    }

    public class MyOwnValueGenerator : ValueGenerator<string>
    {
        public override bool GeneratesTemporaryValues => false;

        public override string Next(EntityEntry entry) => ObjectId.GenerateNewId().ToString();
    }
}