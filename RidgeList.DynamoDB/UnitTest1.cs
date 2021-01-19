using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using RidgeList.Domain;

namespace RidgeList.DynamoDB
{
    public class DynamoDbRepository : IWishlistRepository
    {
        private readonly AmazonDynamoDBClient _client;

        public DynamoDbRepository(AmazonDynamoDBClient client)
        {
            _client = client;
        }
        
        public async Task Save(Wishlist wishlist)
        {
            var table = Table.LoadTable(_client, "wishlists");
            var doc = Document.FromJson(Newtonsoft.Json.JsonConvert.SerializeObject(wishlist));

            await table.PutItemAsync(doc);
        }

        public async Task<Wishlist> Load(Guid id)
        {
            Primitive hash = new Primitive(id.ToString(), true);
            
            var table = Table.LoadTable(_client, "wishlists");
            var f = await table.GetItemAsync(hash);
            var g = Newtonsoft.Json.JsonConvert.DeserializeObject<Wishlist>(f.Values.Single().AsString());
            return g;
        }

        public Task<IEnumerable<WishlistSummary>> GetWishlistSummaries(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public static IWishlistRepository Create()
        {
            var ddbConfig = new AmazonDynamoDBConfig();
            ddbConfig.ServiceURL = "http://localhost:8000";
            return new DynamoDbRepository(new AmazonDynamoDBClient(ddbConfig));
        }
    }
}