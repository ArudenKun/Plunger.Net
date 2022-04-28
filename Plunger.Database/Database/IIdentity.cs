using MongoDB.Bson;

namespace Plunger.Database;

/// <summary>
/// Represents an object that can be identified with an ObjectId.
/// </summary>
public interface IIdentity
{
    /// <summary>
    /// Gets or sets the ObjectId.
    /// </summary>
    ObjectId ObjectId { get; set; }
}
