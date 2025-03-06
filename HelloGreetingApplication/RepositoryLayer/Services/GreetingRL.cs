using System;
using RepositoryLayer.Interface;
using RepositoryLayer.Context;
using ModelLayer.Model;
using Microsoft.Extensions.Logging;
namespace RepositoryLayer.Services
{
	public class GreetingRL:IGreetingRL
	{
		private readonly HelloGreetingContext _context;
		private readonly ILogger<GreetingRL> _logger;
		public GreetingRL(HelloGreetingContext context,ILogger<GreetingRL> logger)
		{
			_context = context;
			_logger = logger;

		}
		//method to add the greeting in database
		public string Add(GreetingModel greeting)
		{
			_context.Greetings.Add(greeting);
			try
			{
				_context.SaveChanges();
				return "Successfully added Greeting";
			}
			catch(Exception ex)
			{
				_logger.LogError("Error in Saving Greeting in database");
				return ex.Message;
			}
		}
		//method to get all the greetings
		public List<GreetingModel> GetDataBase()
		{	
			return _context.Greetings.ToList<GreetingModel>();
		}
		//method to find the greeting on id.
		public GreetingModel FindGreeting(int id)
		{
			_logger.LogInformation($"Finding the {id}");
			var greeting = _context.Greetings.Find(id);
			_logger.LogInformation("Returning the greeting from database");
			return greeting;
		}
		//method to update the greeting on id
		public void UpdateGreeting(int id,string Message)
		{
			_logger.LogInformation($"updating the {id}");
			var result = _context.Greetings.Find(id);
			if (result != null)
			{
				result.Message = Message;
				_context.SaveChanges();

			}
		}
		//method to delete the greeting by id
		public bool DeleteGreeting(int id)
		{
			_logger.LogInformation($"Deleting the {id}");
			var result = _context.Greetings.Find(id);
			if (result != null)
			{
				_logger.LogInformation($"Deletion complete{id}");
				_context.Greetings.Remove(result);
				_context.SaveChanges();
				return true;

			}
			return false;
		}
		
	}
}

