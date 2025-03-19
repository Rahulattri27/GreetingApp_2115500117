using System;
using ModelLayer.Model;
namespace RepositoryLayer.Interface
{
	public interface IGreetingRL
	{
		string Add(GreetingModel greeting);
		List<GreetingModel> GetDataBase();
		List<GreetingModel> FindGreeting(int id);
		void UpdateGreeting(int id, string Message);
		bool DeleteGreeting(int id);
		public GreetingModel? Find(int id);

    }
}

