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

    }
}

