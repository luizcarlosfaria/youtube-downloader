using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DevWeek.Architecture.Business;

[DataContract]
public abstract class MongoEntity : Entity
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    [DataMember]
    [XmlIgnore]
    public string ID { get; set; }
}