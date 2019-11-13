using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MovieShop.Data;
using MovieShop.Entities;

namespace MovieShop.Services
{
    public interface IUserService
    {
        Task<User> ValidateUser(string email, string password);
        Task<User> Createuser(string email,string password, string firstName, string lastName);

        Task<IEnumerable<Purchase>> GetPurchases(int userid);

    }
}
