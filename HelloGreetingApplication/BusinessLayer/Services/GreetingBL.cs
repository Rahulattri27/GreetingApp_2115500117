using System;
using BusinessLayer.Interface;
using Microsoft.Extensions.Logging;
namespace BusinessLayer.Services
{
	public class GreetingBL:IGreetingBL
	{
		ILogger<GreetingBL> _logger;
		public GreetingBL(ILogger<GreetingBL> logger)
		{
			_logger = logger;
		}
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
			return "Hello, World";


        }
		
	}
}

