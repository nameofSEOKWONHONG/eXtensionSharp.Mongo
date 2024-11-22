using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eXtensionSharp.Mongo;

public class JMongoObjectBase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public String Id { get; set; }
    
    [BsonElement("create_by")]
    public string CreatedBy { get; set; }
    
    [BsonElement("created_on")]
    public DateTime CreatedOn { get; set; }
    
    [BsonElement("modified_by")]
    public string ModifiedBy { get; set; }
    
    [BsonElement("modified_on")]
    public DateTime? ModifiedOn { get; set; }    
}