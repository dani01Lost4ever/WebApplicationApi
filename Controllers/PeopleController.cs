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
                        Cognome= entity.Cognome,
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

        /// <summary>
        /// The get method ask for the number of questions that the user wants to display and filters It based on Province
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="province"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ByProvince")]
        public async Task<IEnumerable<User>> RetrieveQuestionsByProvince(
            int quantity,
            string province
        )
        {
            string connectionString =
                "DefaultEndpointsProtocol=https;AccountName=peopledata;"
                + "AccountKey=VtpmJrw2Ps5WiCFiQNX7sDxYqH736dR5TpoBa45lYGIgAwtjLaoD273LRg21hCfHy1zb8PBuYWd6ACDbpcwIEA==;"
                + "TableEndpoint=https://peopledata.table.cosmos.azure.com/;";
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            string tableName = "People";
            CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);
            // in order to verify If It exists otherwise It is going to create a new one
            await cloudTable.CreateIfNotExistsAsync();

            // Here I filter based on Province
            TableQuery<User> tableQuery = new TableQuery<User>()
                .Where(
                    TableQuery.GenerateFilterCondition(
                        "Provincia",
                        QueryComparisons.Equal,
                        province
                    )
                )
                .Take(quantity);
            // We set the continuation token to null to verify It
            TableContinuationToken token = null;

            List<User> userList = new();

            do
            {
                TableQuerySegment<User> segment = await cloudTable.ExecuteQuerySegmentedAsync(
                    tableQuery,
                    token
                );
                foreach (User entity in segment)
                {
                    User user = new User()
                    {
                        Email = entity.PartitionKey,
                        DomandaID = entity.RowKey,
                        Nome = entity.Nome,
                        Cognome = entity.Cognome,
                        Indirizzo = entity.Indirizzo,
                        CAP = entity.CAP,
                        Comune = entity.Comune,
                        Provincia = entity.Provincia,
                        DataPresentazioneDomanda = entity.Timestamp,
                        StatoDellaDomanda = entity.StatoDellaDomanda
                    };
                    userList.Add(user);
                }
                token = segment.ContinuationToken;
            } while (token != null);
            userList.Reverse();
            return userList.Take(quantity);
        }

        [HttpPost]
        [Route("GetQuestionsTimeOffsetAndState")]
        public async Task<IEnumerable<User>> LastQuestions(DateTimeOffset startDate, DateTimeOffset endDate, int quantity, string statoDellaDomanda)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=peopledata;AccountKey=VtpmJrw2Ps5WiCFiQNX7sDxYqH736dR5TpoBa45lYGIgAwtjLaoD273LRg21hCfHy1zb8PBuYWd6ACDbpcwIEA==;TableEndpoint=https://peopledata.table.cosmos.azure.com/";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            string tableName = "People";
            CloudTable table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();

            TableQuery<User> query = new TableQuery<User>()
                .Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, startDate),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThanOrEqual, endDate)
                    )
                )
                .Where(TableQuery.GenerateFilterCondition("StatoDellaDomanda", QueryComparisons.Equal, statoDellaDomanda))
                .Take(quantity);

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
                        Cognome = entity.Cognome,
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
            return items;
        }



        [HttpPost]
        [Route("GetQuestionsTimeOffset")]
        public async Task<IEnumerable<User>> LastQuestionsOffset(DateTimeOffset startDate, DateTimeOffset endDate, int quantity)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=peopledata;AccountKey=VtpmJrw2Ps5WiCFiQNX7sDxYqH736dR5TpoBa45lYGIgAwtjLaoD273LRg21hCfHy1zb8PBuYWd6ACDbpcwIEA==;TableEndpoint=https://peopledata.table.cosmos.azure.com/";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            string tableName = "People";
            CloudTable table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();

            TableQuery<User> query = new TableQuery<User>()
               .Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, startDate),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThanOrEqual, endDate)
                    )
                )
                .Take(quantity);

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
                        Cognome = entity.Cognome,
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
            return items;
        }


        [Route("userprovince")]
        [HttpGet]
        public async Task<List<UserProvince>> UserProvinceAsync()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=peopledata;AccountKey=VtpmJrw2Ps5WiCFiQNX7sDxYqH736dR5TpoBa45lYGIgAwtjLaoD273LRg21hCfHy1zb8PBuYWd6ACDbpcwIEA==;TableEndpoint=https://peopledata.table.cosmos.azure.com/";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            string tableName = "People";
            CloudTable table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();

            TableQuery<User> query = new TableQuery<User>();
            TableContinuationToken continuationToken = null;

            List<UserProvince> userProvince = new List<UserProvince>();
            List<UserProvince> data = new List<UserProvince>();
            do
            {
                TableQuerySegment<User> segment = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                List<User> list = new List<User>();
                list = segment.OrderBy(o => o.Provincia).ToList();
                foreach (var line in list.GroupBy(info => info.Provincia)
                                        .Select(group => new {
                                            Provincia = group.Key,
                                            Count = group.Count()
                                        })
                                        .OrderBy(x => x.Provincia))
                {
                    UserProvince newUserProvince = new UserProvince()
                    {
                        provincia = line.Provincia,
                        n_requests = line.Count
                    };
                    data.Add(newUserProvince);
                }
            } while (continuationToken != null);

            return data;
        }


    }
    public class UserProvince
    {
        public string provincia { get; set; }
        public int n_requests { get; set; }
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

