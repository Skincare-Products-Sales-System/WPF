using SPSS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.DAL.Repositories
{
    public class UserAnswerRepository
    {
        public void Create(UserAnswer obj)
        {
            using (var _context = new SkincareProductSaleSystemContext())
            {
                _context.UserAnswers.Add(obj);
                _context.SaveChanges();
            }
        }
    }
}
