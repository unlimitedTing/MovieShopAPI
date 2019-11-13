using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MovieShop.Entities;

namespace MovieShop.Data
{
    public interface IUserRepository:IRepository<User>
    {
        Task<User> GetUserByEmail(string email);
        Task<IEnumerable<Purchase>> GetuserPurchaseMovies(int userid);
    }
}
