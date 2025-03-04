using System;
namespace BusinessLayer.Interface
{
	public interface IGreetingBL
	{
		string SimpleGreeting();
		string GreetingMessage(string? FirstName, string? LastName);
	}
}

