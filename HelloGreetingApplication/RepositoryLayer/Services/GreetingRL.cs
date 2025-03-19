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
            var userExists = _context.Users.Any(u => u.UserId == greeting.UserId);
            if (!userExists)
            {
                return "User does not exist.";
            }

            _context.Greetings.Add(greeting);
            try
            {
                _context.SaveChanges();
                return "Successfully added Greeting";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Saving Greeting in database: {ex.Message}");
                return ex.Message;
            }
        }

        //method to get all the greetings
        public List<GreetingModel> GetDataBase()
		{	
			return _context.Greetings.ToList<GreetingModel>();
		}
		//method to find the greeting of id
		public GreetingModel? Find(int id)
		{
			_logger.LogInformation("Returning the id if found");
			return _context.Greetings.Find(id);
		}
		//method to find the greeting on userid.
		public List<GreetingModel> FindGreeting(int id)
		{
			_logger.LogInformation($"Finding the {id}");
			var greeting = _context.Greetings.Where(g => g.UserId == id).ToList();
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

