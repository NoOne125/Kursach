using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace API
{
    public class DynamoDbClient : IDynamoDbClient, IDisposable
    {
        public string _tableName_main;
        public string _tableName_place;
        private readonly IAmazonDynamoDB _dynamoDb;

        public DynamoDbClient(IAmazonDynamoDB dynamoDB)
        {
            _dynamoDb = dynamoDB;
            _tableName_main = Constants.TableName_1;
            _tableName_place = Constants.TableName_2;
        }

        public async Task<List<string>> GetDataByUser(string chat_id, bool route)
        {

            var item = new GetItemRequest
            {
                TableName = _tableName_main,
                Key = new Dictionary<string, AttributeValue>
                    {
                        {"chat_id", new AttributeValue{S = chat_id} }
                    }
            };

            var response = await _dynamoDb.GetItemAsync(item);

            if (response.Item == null || !response.IsItemSet)
                return null;

            var result = response.Item.ToClass<UserDbRepository>();

            List<string> answer;
            if (route)
            {
                answer = result.name_ro;
                if (answer[0] == "5fgh213454dfge657r54365q32")
                    answer = new List<string>();
            }
            else
                answer = result.name_pl;
            return answer;
        }

        public async Task<List<string>> GetDataByPlaylist(string chat_id, string name_pl, bool route)
        {
            var item = new GetItemRequest
            {
                TableName = _tableName_main,
                Key = new Dictionary<string, AttributeValue>
                    {
                        {"chat_id", new AttributeValue{S = chat_id} }
                    }
            };

            var response = await _dynamoDb.GetItemAsync(item);

            if (response.Item == null || !response.IsItemSet)
                return null;

            var result = response.Item.ToClass<UserDbRepository>();

            int index;

            if (route)
            {
                index = result.name_ro.IndexOf(name_pl);
            }
            else
            {
                index = result.name_pl.IndexOf(name_pl);
            }

            List<string> answer;
            if (route)
                answer = result.routes[index];
            else
                answer = result.playlists[index];
            if (answer[0] == "Empty")
                answer = new List<string>();
            return answer;
        }

        public async Task<PlaceDbRepository> GetDataByPlace(string place_id)
        {
            var item = new GetItemRequest
            {
                TableName = _tableName_place,
                Key = new Dictionary<string, AttributeValue>
                    {
                        {"place_id", new AttributeValue{S = place_id} }
                    }
            };

            var response = await _dynamoDb.GetItemAsync(item);

            if (response.Item == null || !response.IsItemSet)
                return null;

            var result = response.Item.ToClass<PlaceDbRepository>();

            return result;
        }

        public async Task<bool> PostDataToDbUser(string chat_id)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName_main,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"chat_id", new AttributeValue { S = chat_id } },
                    {"name_pl", new AttributeValue { SS = new List<string>(){ "Любимые"} } },
                    {"name_ro", new AttributeValue { SS = new List<string>() { "5fgh213454dfge657r54365q32" } } },
                    {"playlists", new AttributeValue { L = new List<AttributeValue>(){ new AttributeValue() {SS = { "Empty" } } } } },
                    {"routes", new AttributeValue { L = new List<AttributeValue>(){ new AttributeValue() {SS = { "Empty" } } } } }
                }
            };

            var response = await _dynamoDb.PutItemAsync(request);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> PostDataToDbPlace(PlaceDbRepository data)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName_place,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"place_id", new AttributeValue { S = data.place_id } },
                    {"name", new AttributeValue { S = data.name } },
                    {"vicinity", new AttributeValue { S = data.vicinity } },
                    {"rating", new AttributeValue { N = data.rating.ToString().Replace(",", ".")} },
                    {"location", new AttributeValue { NS = new List<string>(){data.location[0].ToString().Replace(",", "."), data.location[1].ToString().Replace(",", ".") } } }
                }
            };

                var response = await _dynamoDb.PutItemAsync(request);

                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;

        }

        public async Task<bool> UpdateDataIntoDbPlaylist(string chat_id, string playlist, string place_id, bool route, bool delete)
        {
            var item = new GetItemRequest
            {
                TableName = _tableName_main,
                Key = new Dictionary<string, AttributeValue>
                    {
                        {"chat_id", new AttributeValue{S = chat_id} }
                    }
            };

            var response = await _dynamoDb.GetItemAsync(item);

            var user = response.Item.ToClass<UserDbRepository>();

            Dictionary<string, AttributeValueUpdate> updates = new Dictionary<string, AttributeValueUpdate>();
            if (route)
            {
                if (delete)
                {
                    user.routes[user.name_ro.IndexOf(playlist)].Remove(place_id);
                    if (user.routes[user.name_ro.IndexOf(playlist)].Count == 0)
                    {
                        user.routes[user.name_ro.IndexOf(playlist)].Add("Empty");
                    }
                }
                else
                {
                    if (user.routes[user.name_ro.IndexOf(playlist)].Contains(place_id))
                        user.routes[user.name_ro.IndexOf(playlist)].Add(place_id);
                    else
                        return false;
                    if (user.routes[user.name_ro.IndexOf(playlist)][0] == "Empty")
                    {
                        user.routes[user.name_ro.IndexOf(playlist)].RemoveAt(0);
                    }
                }
                List<AttributeValue> loc_1 = new List<AttributeValue>();
                for (int i = 0; i < user.routes.Count; i++)
                {
                    loc_1.Add(new AttributeValue { SS = user.routes[i] });
                }
                updates["routes"] = new AttributeValueUpdate() { Action = AttributeAction.PUT, Value = new AttributeValue() { L = loc_1 } };
            }
            else
            {
                if (delete)
                {
                    user.playlists[user.name_pl.IndexOf(playlist)].Remove(place_id);
                    if (user.playlists[user.name_pl.IndexOf(playlist)].Count == 0)
                    {
                        user.playlists[user.name_pl.IndexOf(playlist)].Add("Empty");
                    }
                }
                else
                {
                    user.playlists[user.name_pl.IndexOf(playlist)].Add(place_id);
                    if (user.playlists[user.name_pl.IndexOf(playlist)][0] == "Empty")
                    {
                        user.playlists[user.name_pl.IndexOf(playlist)].RemoveAt(0);
                    }
                }
                List<AttributeValue> loc_1 = new List<AttributeValue>();
                for (int i = 0; i < user.playlists.Count; i++)
                {
                    loc_1.Add(new AttributeValue { SS = user.playlists[i] });
                }
                updates["playlists"] = new AttributeValueUpdate() { Action = AttributeAction.PUT, Value = new AttributeValue() { L = loc_1 } };
            }
            var request = new UpdateItemRequest
            {
                TableName = _tableName_main,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "chat_id", new AttributeValue { S = chat_id } }
                },
                AttributeUpdates = updates
            };
            var response_0 = await _dynamoDb.UpdateItemAsync(request);
            return response_0.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        public async Task<bool> UpdateDataIntoDbUser(string chat_id, string playlist, bool route, bool delete)
        {
            var item = new GetItemRequest
            {
                TableName = _tableName_main,
                Key = new Dictionary<string, AttributeValue>
                    {
                        {"chat_id", new AttributeValue{S = chat_id} }
                    }
            };

            var response = await _dynamoDb.GetItemAsync(item);

            var user = response.Item.ToClass<UserDbRepository>();

            Dictionary<string, AttributeValueUpdate> updates = new Dictionary<string, AttributeValueUpdate>();
            if (route)
            {
                if (delete)
                {
                    int index = user.name_ro.IndexOf(playlist);
                    user.name_ro.RemoveAt(index);
                    user.routes.RemoveAt(index);
                    if (user.name_ro.Count == 0)
                    {
                        user.name_ro.Add("5fgh213454dfge657r54365q32");
                        user.routes.Add(new List<string>() { "Empty" });
                    }
                }
                else
                {
                    user.name_ro.Add(playlist);
                    user.routes.Add(new List<string>() { "Empty" });
                    if (user.name_ro[0] == "5fgh213454dfge657r54365q32")
                    {
                        user.name_ro.RemoveAt(0);
                        user.routes.RemoveAt(0);
                    }
                }
                List<AttributeValue> loc = new List<AttributeValue>();
                foreach (var k in user.routes)
                {
                    loc.Add(new AttributeValue() { SS = k });
                }
                updates["routes"] = new AttributeValueUpdate() { Action = AttributeAction.PUT, Value = new AttributeValue() { L = loc } };
                updates["name_ro"] = new AttributeValueUpdate() { Action = AttributeAction.PUT, Value = new AttributeValue() { SS = user.name_ro } };
            }
            else
            {
                if (delete)
                {
                    int index = user.name_pl.IndexOf(playlist);
                    user.name_pl.RemoveAt(index);
                    user.playlists.RemoveAt(index);
                }
                else
                {
                    user.name_pl.Add(playlist);
                    user.playlists.Add(new List<string>() { "Empty"});
                }
                List<AttributeValue> loc = new List<AttributeValue>();
                foreach (var k in user.playlists)
                {
                    loc.Add(new AttributeValue() { SS = k });
                }
                updates["playlists"] = new AttributeValueUpdate() { Action = AttributeAction.PUT, Value = new AttributeValue() { L = loc } };
                updates["name_pl"] = new AttributeValueUpdate() { Action = AttributeAction.PUT, Value = new AttributeValue() { SS = user.name_pl } };
            }
            var request = new UpdateItemRequest
            {
                TableName = _tableName_main,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "chat_id", new AttributeValue { S = chat_id } }
                },
                AttributeUpdates = updates
            };

            try
            {
                var response_0 = await _dynamoDb.UpdateItemAsync(request);

                return response_0.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                Console.WriteLine("Here is your error \n" + e);
                return false;
            }
        }

        public void Dispose()
        {
            
            _dynamoDb.Dispose();
        }
    }
}
