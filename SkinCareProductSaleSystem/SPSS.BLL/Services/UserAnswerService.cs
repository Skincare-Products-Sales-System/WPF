using SPSS.DAL.Models;
using SPSS.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.BLL.Services
{
    public class UserAnswerService
    {
        private UserAnswerRepository _userAnswerRepository = new();

        public void Create(UserAnswer obj)
        {
            _userAnswerRepository.Create(obj);
        }
    }
}
