using System;
using ModelLayer.Model;

namespace BusinessLayer.Interface
{
	public interface IGreetingBL
	{
		string SimpleGreeting();
		string GreetingMessage(string? FirstName, string? LastName);
		string Create(GreetingModel greeting);
		Task<List<GreetingModel>> GetDatabaseGreeting();
		Task<List<GreetingModel>> FindGreeting(int id);
		Task<bool> UpdateGreeting(int id, string Message);
		Task<bool> DeleteGreeting(int id);
		GreetingModel? FindGreetingById(int id);
    }
}

