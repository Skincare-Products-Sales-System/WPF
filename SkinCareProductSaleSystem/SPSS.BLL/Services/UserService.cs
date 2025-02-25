using SPSS.DAL.Models;
using SPSS.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.BLL.Services
{
    public class UserService
    {
        private UserRepository _userRepository = new();
        public List<User> GetAllUsers()
        {
            return _userRepository.GetAll();
        }
        public User? Authenticate(string username, string password)
        {
            return _userRepository.LoginUser(username, password);
        }

        public void UpdateSkinType(User user, string skinType)
        {
            user.SkinType = skinType;
            _userRepository.Update(user);
        }
        public User GetUserById(int? id)
        {
            return _userRepository.GetUserById(id);
        }
        public void SignUp(User obj)
        {
             _userRepository.Create(obj);
        }
        public void DeleteUser(int id)
        {
            _userRepository.DeleteUser(id);
        }
        
    }
}
