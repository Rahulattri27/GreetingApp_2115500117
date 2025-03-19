using System;
using BusinessLayer.Interface;
using ModelLayer.Model;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Interface;
namespace BusinessLayer.Services
{
	public class GreetingBL:IGreetingBL
	{
		private readonly ILogger<GreetingBL> _logger;
		private readonly IGreetingRL _greetingRL;
		//Constructor
		public GreetingBL(ILogger<GreetingBL> logger,IGreetingRL greetingRL)
		{
			_logger = logger;
			_greetingRL = greetingRL;
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
			return _greetingRL.Add(greeting);
			

		}
		//method to get the greetings from database
        public List<GreetingModel> GetDatabaseGreeting()
		{
			return _greetingRL.GetDataBase();
		}
		public GreetingModel? FindGreetingById(int id)
		{
			_logger.LogInformation("Find the greeting on id.");
			return _greetingRL.Find(id);
		}

		//method to find the greeting on id in database
		public List<GreetingModel> FindGreeting(int userid)
		{
			_logger.LogInformation("Returing the greeting if found.");
			return _greetingRL.FindGreeting(userid);
			
		}
		//method to update the greeting on basis of id
		public bool UpdateGreeting(int id,string Message)
		{
			var response = _greetingRL.FindGreeting(id);
			if (response == null)
			{
				return false;
			}
			_greetingRL.UpdateGreeting(id, Message);
			return true;
		}
		//method to delete the greeting on basis of id
		public bool DeleteGreeting(int id)
		{
			_logger.LogInformation($"Sending{id} to Repository layer");
			return _greetingRL.DeleteGreeting(id);
		}
    }
}

