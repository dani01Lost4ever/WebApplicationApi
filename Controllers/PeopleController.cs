using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        [HttpPost]
        [Route("LastQuestions")]
        public async Task<IEnumerable<User>> LastQuestions(int quantity)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=peopledata;AccountKey=VtpmJrw2Ps5WiCFiQNX7sDxYqH736dR5TpoBa45lYGIgAwtjLaoD273LRg21hCfHy1zb8PBuYWd6ACDbpcwIEA==;TableEndpoint=https://peopledata.table.cosmos.azure.com/";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            string tableName = "People";
            CloudTable table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();

            TableQuery<User> query = new TableQuery<User>(); // Sort by descending timestamp and take N records
            TableContinuationToken continuationToken = null;
            List<User> items = new List<User>();

            do
            {
                TableQuerySegment<User> segment = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                foreach (User entity in segment)
                {
                    User user = new User()
                    {
                        Email = entity.PartitionKey,
                        DomandaID = entity.RowKey,
                        Nome = entity.Nome,
                        Indirizzo = entity.Indirizzo,
                        CAP = entity.CAP,
                        Comune = entity.Comune,
                        Provincia = entity.Provincia,
                        DataPresentazioneDomanda = entity.Timestamp,
                        StatoDellaDomanda = entity.StatoDellaDomanda
                    };
                    items.Add(user);
                }
                continuationToken = segment.ContinuationToken;
            } while (continuationToken != null);
            items.Reverse();
            return items.Take(quantity);
        }
    }
    public class User : TableEntity
    {
        public string DomandaID { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string Indirizzo { get; set; }
        public string CAP { get; set; }
        public string Comune { get; set; }
        public string Provincia { get; set; }
        public string Email { get; set; }
        public DateTimeOffset DataPresentazioneDomanda { get; set; }
        public string StatoDellaDomanda { get; set; }
    }
}

