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
		
	}
}

