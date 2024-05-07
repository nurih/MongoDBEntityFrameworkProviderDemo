# MongoDB Entity Framework Provider Demo

## What

A simplistic application that uses the official MongoDB Entity Framework Core Provider. 

NuGet: [NuGet Gallery | MongoDB.EntityFrameworkCore 7.0.0-preview.1](https://www.nuget.org/packages/MongoDB.EntityFrameworkCore)

Repo: [mongodb/mongo-efcore-provider: MongoDB Entity Framework Core Provider (github.com)](https://github.com/mongodb/mongo-efcore-provider)

Docs: [MongoDB Entity Framework Core Provider — Entity Framework](https://www.mongodb.com/docs/entity-framework/current/)

## Why

Entity Framework has long been available to attempt to smooth unify and unify the tasks of storing .NET entities in a backing relational database.

The MongoDB provider for EF is an effort to give similar EF user experience, but with a document database backing.

> Note: The current provider is in preview status. 
>
> So... this will age nicely :-)


## Notes

1. In the 8.0 GA release, both `BsonElementAttribute()` (MongoDB) and `ColumnAttribute()` (EF / Data Annotation) are respected. You can name the MongoDB side field and keep your POCO names as you wish.
1. Applying `[BsonRepresentation(..)]` to a POCO attribute throws.
1. Applying `[BsonId]` attribute to a field on a POCO works - you can choose which property will map to the MongoDB document's `_id` field.
1. Using the ModelBuilder to set property behavior such as conversion POCO &lt;--&gt; BSON, value generation etc demo updated, for specific properties.
