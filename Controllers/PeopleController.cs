using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Text.RegularExpressions;

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        [HttpPost]
        [Route("LastQuestions")]
        public async Task<ActionResult<IEnumerable<User>>> LastQuestions(int quantity)
        {
            try
            {
                if (quantity is int && quantity > 0) { }
                else throw new InvalidDataException("Check quantity, must be greater than 0");

            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
            try
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
                int retrievedRecords = 0;

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
                            DataPresentazioneDomanda = entity.DataPresentazioneDomanda,
                            StatoDellaDomanda = entity.StatoDellaDomanda
                        };
                        items.Add(user);
                        retrievedRecords++;
                        if (retrievedRecords >= quantity) break;

                    }
                    continuationToken = segment.ContinuationToken;
                } while (continuationToken != null && retrievedRecords < quantity);
                items.Reverse();
                return items;
            }catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
            
        }

        [HttpPost]
        [Route("ByProvince")]
        public async Task<ActionResult<IEnumerable<User>>> RetrieveQuestionsByProvince(int quantity, string province)
        {
            try
            {
                if (quantity is int && quantity > 0) { }
                else throw new InvalidDataException("CHeck quantity, must be greater than 0");
                string pattern = @"^\p{L}+$";
                if (Regex.IsMatch(province, pattern)){ }
                else throw new InvalidDataException("CHeck province, must be only letters");

            }
            catch (Exception ex)
            {
                return StatusCode(400, ex);
            }

            try
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
                    );
                    
                // We set the continuation token to null to verify It
                TableContinuationToken token = null;

                List<User> userList = new();
                int retrievedRecords = 0;

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
                            DataPresentazioneDomanda = entity.DataPresentazioneDomanda,
                            StatoDellaDomanda = entity.StatoDellaDomanda
                        };
                        userList.Add(user);
                        retrievedRecords++;
                        if (retrievedRecords >= quantity) break;

                    }
                    token = segment.ContinuationToken;
                } while (token != null && retrievedRecords < quantity);
                userList.Reverse();
                return userList;

            }catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        [Route("GetQuestionsTimeOffsetAndState")]
        public async Task<ActionResult<IEnumerable<User>>> LastQuestions(DateTime startDate, DateTime endDate, int quantity, string statoDellaDomanda)
        {
            try
            {
                List<int> acceptedValues = new List<int> { 1, 2, 3, 4 };
                if (startDate is DateTime && endDate is DateTime) { }
                else throw new InvalidCastException("Check Dates");
                if (quantity is int && quantity > 0) { }
                else throw new InvalidDataException("CHeck quantity, must be greater than 0");
                int statoDellaDomandaInt;
                if (int.TryParse(statoDellaDomanda, out statoDellaDomandaInt) && acceptedValues.Contains(statoDellaDomandaInt)) { }
                else throw new InvalidCastException("The state must be 1,2,3 or 4");
            }
            catch(Exception ex) {
                return StatusCode(400, ex);
            }

            try
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=peopledata;AccountKey=VtpmJrw2Ps5WiCFiQNX7sDxYqH736dR5TpoBa45lYGIgAwtjLaoD273LRg21hCfHy1zb8PBuYWd6ACDbpcwIEA==;TableEndpoint=https://peopledata.table.cosmos.azure.com/";
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                string tableName = "People";
                CloudTable table = tableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();


                DateTimeOffset startDateO = new DateTimeOffset(startDate);
                DateTimeOffset endDateO = new DateTimeOffset(endDate);
                string startDateString = TableQuery.GenerateFilterConditionForLong("DataPresentazioneDomanda", QueryComparisons.GreaterThanOrEqual, startDateO.UtcDateTime.Ticks);
                string endDateString = TableQuery.GenerateFilterConditionForLong("DataPresentazioneDomanda", QueryComparisons.LessThanOrEqual, endDateO.UtcDateTime.Ticks);
                string finalFilter = TableQuery.CombineFilters(startDateString, TableOperators.And, endDateString);
                string state = TableQuery.GenerateFilterCondition("StatoDellaDomanda", QueryComparisons.Equal, statoDellaDomanda);
                TableQuery<User> query = new TableQuery<User>().Where(finalFilter).Where(state);


                TableContinuationToken continuationToken = null;
                List<User> items = new List<User>();
                int retrievedRecords = 0;

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
                            DataPresentazioneDomanda = entity.DataPresentazioneDomanda,
                            StatoDellaDomanda = entity.StatoDellaDomanda
                        };
                        items.Add(user);
                        retrievedRecords++;
                        if (retrievedRecords >= quantity) break;
                    }
                    continuationToken = segment.ContinuationToken;
                } while (continuationToken != null && retrievedRecords < quantity);

                items.Reverse();
                return items;
            }catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
            
        }



        [HttpPost]
        [Route("GetQuestionsTimeOffset")]
        public async Task<ActionResult<IEnumerable<User>>> LastQuestionsOffset(DateTime startDate, DateTime endDate, int quantity)
        {
            try
            {
                if (startDate is DateTime && endDate is DateTime) { }
                else throw new InvalidCastException();
            }catch (Exception ex){ return StatusCode(400, ex); }

            try
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=peopledata;AccountKey=VtpmJrw2Ps5WiCFiQNX7sDxYqH736dR5TpoBa45lYGIgAwtjLaoD273LRg21hCfHy1zb8PBuYWd6ACDbpcwIEA==;TableEndpoint=https://peopledata.table.cosmos.azure.com/";
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                string tableName = "People";
                CloudTable table = tableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();


                DateTimeOffset startDateO = new DateTimeOffset(startDate);
                DateTimeOffset endDateO = new DateTimeOffset(endDate);
                string startDateString = TableQuery.GenerateFilterConditionForLong("DataPresentazioneDomanda", QueryComparisons.GreaterThanOrEqual, startDateO.UtcDateTime.Ticks);
                string endDateString = TableQuery.GenerateFilterConditionForLong("DataPresentazioneDomanda", QueryComparisons.LessThanOrEqual, endDateO.UtcDateTime.Ticks);
                string finalFilter = TableQuery.CombineFilters(startDateString, TableOperators.And, endDateString);
                TableQuery<User> query = new TableQuery<User>().Where(finalFilter);

                TableContinuationToken continuationToken = null;
                List<User> items = new List<User>();
                int retrievedRecords = 0;

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
                            DataPresentazioneDomanda = entity.DataPresentazioneDomanda,
                            StatoDellaDomanda = entity.StatoDellaDomanda
                        };
                        items.Add(user);
                        retrievedRecords++;
                        if (retrievedRecords >= quantity) break;
                    }
                    continuationToken = segment.ContinuationToken;
                } while (continuationToken != null && retrievedRecords < quantity);

                items.Reverse();
                return items;
            }catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        [Route("userprovince")]
        [HttpGet]
        public async Task<ActionResult<List<UserProvince>>> UserProvinceAsync()
        {
            try
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
            catch (Exception ex) {
                return StatusCode(500, ex);
            }
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
        public DateTime DataPresentazioneDomanda { get; set; }
        public string StatoDellaDomanda { get; set; }
    }
}

