// using Microsoft.Extensions.Options;
// using MongoDataAccess.Models;
// using MongoDB.Driver;
// using Plunger.Database.Models;

// namespace Plunger.Database;

// public partial class PlungerDatabase
// {
//     private readonly IMongoCollection<TestModel> Test;
//     private readonly IMongoCollection<SuggestionModel> Suggestion;

//     public PlungerDatabase(IOptions<PlungerDatabaseConfig> plungerDatabaseConfig)
//     {
//         var mongoClient = new MongoClient(plungerDatabaseConfig.Value.ConnectionString);

//         var mongoDatabase = mongoClient.GetDatabase(plungerDatabaseConfig.Value.DatabaseName);

//         Test = mongoDatabase.GetCollection<TestModel>(plungerDatabaseConfig.Value.TestCollection);
//         Suggestion = mongoDatabase.GetCollection<SuggestionModel>(plungerDatabaseConfig.Value.SuggestionCollection);
//     }

//     public async Task<List<TestModel>> GetAsync() =>
//         await Test.Find(_ => true).ToListAsync();

//     public async Task<TestModel?> GetAsync(string id) =>
//         await Test.Find(x => x.Id == id).FirstOrDefaultAsync();

//     public async Task CreateAsync(TestModel newBook) =>
//         await Test.InsertOneAsync(newBook);


//     public async Task UpdateAsync(string id, TestModel updatedBook) =>
//         await Test.ReplaceOneAsync(x => x.Id == id, updatedBook);

//     public async Task RemoveAsync(string id) =>
//         await Test.DeleteOneAsync(x => x.Id == id);
    
//     //===================Suggestion=============\\
//     public async Task CreateSuggestionAsync(SuggestionModel document)
//     {
//         await Suggestion.InsertOneAsync(document);
//     }

//     public async Task RemoveSuggestionAsync(ulong guildId, ulong messageId)
//     {
//         await Suggestion.DeleteOneAsync(_ => _.GuildId == guildId && _.MessageId == messageId);
//     }

//     public async Task<bool> SuggestionExistAsync(ulong guildId, ulong messageId)
//     {
//         return await Suggestion.FindAsync(_ => _.GuildId == guildId && _.MessageId == messageId).Result.AnyAsync();
//     }
// }