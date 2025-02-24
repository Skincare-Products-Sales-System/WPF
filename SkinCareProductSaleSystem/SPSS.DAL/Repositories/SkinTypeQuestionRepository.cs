using Newtonsoft.Json;
using SPSS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.DAL.Repositories
{
    public class SkinTypeQuestionRepository
    {
        public List<SkinTypeQuestion> GetAllQuestions()
        {
            using (var _context = new SkincareProductSaleSystemContext())
            {
                return _context.SkinTypeQuestions.ToList();
            }
        }
        public SkinTypeQuestion GetQuestion(int id)
        {
               using (var _context = new SkincareProductSaleSystemContext())
            {
                return _context.SkinTypeQuestions.Find(id);
            }
        }
        public Dictionary<string, string> GetAnswerOptions(SkinTypeQuestion question)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(question.AnswerOptions);
        }
    }
}
