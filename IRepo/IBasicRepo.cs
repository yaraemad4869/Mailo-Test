using System.Collections.Generic;
using Mailo.Models;
namespace Mailo.IRepo
{
	public interface IBasicRepo<T> where T : class
	{
		Task<List<T>> GetAll();
		Task<T> GetByID(int id);
		void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(ICollection<T> entity);

    }
}
