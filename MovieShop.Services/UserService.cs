using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MovieShop.Entities;
using MovieShop.Data;
using Microsoft.AspNetCore.Mvc;


namespace MovieShop.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICryptoService _cryptoService;
        public UserService(IUserRepository userRepository,ICryptoService cryptoService)
        {
            _userRepository = userRepository;
            _cryptoService = cryptoService;
        }
        public async Task<User> Createuser(string email, string password, string firstName, string lastName)
        {
            var dbUser = await _userRepository.GetUserByEmail(email);
            // validate if it exists in the db
            if (dbUser!=null )
            {
                return null;
            }

            var salt = _cryptoService.CreateSalt();
            var hashPassword = _cryptoService.HashPassword(password, salt);
            var user = new User
            {
                Email = email,
                FirstName=firstName,
                LastName=lastName,
                HashedPassword=hashPassword,
                Salt=salt,

            };

            var createdUser = await _userRepository.AddAsync(user);
            return createdUser;

            
        }

        public async Task<IEnumerable<Purchase>> GetPurchases(int userid)
        {
            return await _userRepository.GetuserPurchaseMovies(userid);
        }

        public async Task<User> ValidateUser(string email, string password)
        {
            var dbUser = await _userRepository.GetUserByEmail(email);
            if (dbUser == null)
            {
                return null;
            }
            //get salt and hashcode
            var salt = dbUser.Salt;
            var hashPassword = dbUser.HashedPassword;
            var newHash = _cryptoService.HashPassword(password, salt);
            if( hashPassword != newHash)
            {
                return null;
            }
            return dbUser;


        }
    }
}
