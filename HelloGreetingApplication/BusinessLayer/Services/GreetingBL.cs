using System;
using BusinessLayer.Interface;
using ModelLayer.Model;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Interface;
using StackExchange.Redis;
using System.Text.Json;
using RepositoryLayer.Services;

namespace BusinessLayer.Services
{
	public class GreetingBL:IGreetingBL
	{
		private readonly ILogger<GreetingBL> _logger;
		private readonly IGreetingRL _greetingRL;
		private readonly IDatabase _cache;
		private const string cacheKey = "AllGreetings";
		private readonly RabbitMQPublisher _rabbitMQPublisher;
		//Constructor
		public GreetingBL(ILogger<GreetingBL> logger,IGreetingRL greetingRL,IDatabase cache,RabbitMQPublisher rabbitMQPublisher)
		{
			_logger = logger;
			_greetingRL = greetingRL;
			_cache = cache;
			_rabbitMQPublisher = rabbitMQPublisher;
		}
		/// <summary>
		/// Generate Hello,World as greeting 
		/// </summary>
		/// <returns>Hello,World</returns>
		public string SimpleGreeting()
		{
			_logger.LogInformation("Returning Hello world from Business Layer");
			return "Hello,World";
			
		}
		public string GreetingMessage(string? FirstName,string? LastName)
		{
			_logger.LogInformation("Generating Greeting Message");
			if(!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
			{
				_logger.LogInformation("Returning full name greeting");
				return $"Hello {FirstName} {LastName}";
			}
			if (!string.IsNullOrEmpty(FirstName))
			{
				_logger.LogInformation("Returning FirstName greeting");
				return $"Hello {FirstName} ";
			}
            if (!string.IsNullOrEmpty(LastName))
            {
                _logger.LogInformation("Returning FirstName greeting");
                return $"Hello {LastName} ";
            }
            _logger.LogInformation("Returning Default greeting");
            return "Hello, World";


        }
		//method to create the greeting 
		public string Create(GreetingModel greeting)
		{
			_logger.LogInformation("Adding the greeting to repository Layer");
			string result = _greetingRL.Add(greeting);
			_cache.KeyDelete(cacheKey);
			_cache.KeyDeleteAsync($"UserGreeting{greeting.UserId}");
			_logger.LogInformation("Cache cleared after adding a new greeting. ");
			var message = new
			{
				GreetingId = greeting.Id,
				UserID = greeting.UserId,
				Message = greeting.Message
			};
			_rabbitMQPublisher.PublishMessage("GreetingQueue", message);
			_logger.LogInformation("Greeting creation event published to rabbitmq");

			return result;
			

		}
		//method to get the greetings from database
        public async Task<List<GreetingModel>> GetDatabaseGreeting()
		{
			_logger.LogInformation("Fetching greetings from cache or database");
			string cachedGreetings = await _cache.StringGetAsync(cacheKey);
			if (!string.IsNullOrEmpty(cachedGreetings))
			{
				_logger.LogInformation("Returning from Cache");
				return JsonSerializer.Deserialize<List<GreetingModel>>(cachedGreetings);
			}
			var greetings=  _greetingRL.GetDataBase();
			await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(greetings), TimeSpan.FromMinutes(10));
			_logger.LogInformation("Cached greeting for future request.");
			return greetings;
		}
		public GreetingModel? FindGreetingById(int id)
		{
			_logger.LogInformation("Find the greeting on id.");
			return _greetingRL.Find(id);
		}

		//method to find the greeting on id in database
		public async Task<List<GreetingModel>> FindGreeting(int userid)
		{
			string cacheUser = $"UserGreeting{userid}";
			string cachedGreetings = await _cache.StringGetAsync(cacheUser);
			if (!string.IsNullOrEmpty(cachedGreetings))
			{
				_logger.LogInformation($"Returning greeting from cache");
				return JsonSerializer.Deserialize<List<GreetingModel>>(cachedGreetings);
			}
			var greeting =_greetingRL.FindGreeting(userid);
			if (greeting != null)
			{
				await _cache.StringSetAsync(cacheUser, JsonSerializer.Serialize(greeting), TimeSpan.FromMinutes(10));
				_logger.LogInformation($"Greetings from cache");
			}
			_logger.LogInformation("Returing the greeting if found.");
			return greeting ;
			
		}
		//method to update the greeting on basis of id
		public async Task<bool> UpdateGreeting(int id,string Message)
		{
			var response = _greetingRL.Find(id);
			if (response == null)
			{
				return false;
			}
			
			_greetingRL.UpdateGreeting(id, Message);
			await _cache.KeyDeleteAsync(cacheKey);
			await _cache.KeyDeleteAsync($"UserGreeting{response.UserId}");
			_logger.LogInformation($"Cache cleared after update");
			return true;
		}
		//method to delete the greeting on basis of id
		public async Task<bool> DeleteGreeting(int id)
		{
			_logger.LogInformation($"Sending{id} to Repository layer");
			
			var response = _greetingRL.Find(id);
			if (response!=null)
			{
				_logger.LogInformation("Clearing the cache");
				await _cache.KeyDeleteAsync(cacheKey);
				await _cache.KeyDeleteAsync($"UserGreeting{response.UserId}");
                return  _greetingRL.DeleteGreeting(id);

            }
			_logger.LogInformation($"{id} not found");
			return false;
		}

    }
}

