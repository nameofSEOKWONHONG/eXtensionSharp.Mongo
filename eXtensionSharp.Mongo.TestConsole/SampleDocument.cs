﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace eXtensionSharp.Mongo.TestConsole;

public class SampleDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public String Id { get; set; }

    [BsonElement("name")] public string Name { get; set; }

    [BsonElement("age")] public int Age { get; set; }
    [BsonElement("createdAt")] public DateTimeOffset CreatedAt { get; set; }
}

public class SampleDocumentConfiguration: IJMongoConfiguration<SampleDocument>
{
    public void Configure(JMongoCollectionBuilder<SampleDocument> collectionBuilder)
    {
        collectionBuilder.ToDocument("sample", "demo")
            .ToCollection(options =>
            {
                options.Capped = false;
                options.MaxDocuments = null;
                options.MaxSize = null;
            })
            .ToIndex(indexes =>
                indexes.CreateOne(
                new CreateIndexModel<SampleDocument>(
                    Builders<SampleDocument>.IndexKeys.Ascending(x => x.CreatedAt),
                    new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(2) }
                )
            ));
    }
}