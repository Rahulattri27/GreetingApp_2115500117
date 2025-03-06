using System;
using ModelLayer.Model;
namespace RepositoryLayer.Interface
{
	public interface IGreetingRL
	{
		string Add(GreetingModel greeting);
		List<GreetingModel> GetDataBase();
		GreetingModel FindGreeting(int id);
		void UpdateGreeting(int id, string Message);
		bool DeleteGreeting(int id);

    }
}

