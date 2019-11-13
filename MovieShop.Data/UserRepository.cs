using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MovieShop.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace MovieShop.Data
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(MovieShopDbContext dbContext):base(dbContext)
        {

        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Purchase>> GetuserPurchaseMovies(int userid)
        {
            var userMovies = await _dbContext.Purchase.Where(p => p.UserId == 
            userid).Include(p => p.Movie).ToListAsync();
            return userMovies;
        }
    }
}
