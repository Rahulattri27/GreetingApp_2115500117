using System;
using ModelLayer.Model;

namespace BusinessLayer.Interface
{
	public interface IGreetingBL
	{
		string SimpleGreeting();
		string GreetingMessage(string? FirstName, string? LastName);
		string Create(GreetingModel greeting);
		List<GreetingModel> GetDatabaseGreeting();
		GreetingModel FindGreeting(int id);

    }
}

