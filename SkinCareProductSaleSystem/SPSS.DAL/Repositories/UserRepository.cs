using SPSS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.DAL.Repositories
{
    public class UserRepository
    {
        private SkincareProductSaleSystemContext _context;

        public User? LoginUser(string username, string password)
        {
            _context = new SkincareProductSaleSystemContext();
            return _context.Users.FirstOrDefault(x => x.Username == username && x.Password == password);
        }

		public User GetUserById(int? id)
		{
			using (var _context = new SkincareProductSaleSystemContext())
			{
				return _context.Users.FirstOrDefault(x => x.UserId == id);
			}
		}

		public List<User> GetAll()
        {
            _context = new SkincareProductSaleSystemContext();
            return _context.Users.ToList();
        }
		public void DeleteUser(int id)
		{
			using (var _context = new SkincareProductSaleSystemContext())
			{
				var user = _context.Users.Find(id);

				if (user != null)
				{
					_context.Users.Remove(user);
					_context.SaveChanges();
				}
			}
		}
		public void Create(User obj)
        {
            _context = new();  //mỗi lần xài 1 lần new _context
            _context.Users.Add(obj); //trong ram
            _context.SaveChanges(); //chính thức tạo mới trong db
        }

        public void Update(User obj)
        {
            _context = new();  //mỗi lần xài 1 lần new _context
            _context.Users.Update(obj); //trong ram
            _context.SaveChanges(); //chính thức update trong db
        }
    }
}
