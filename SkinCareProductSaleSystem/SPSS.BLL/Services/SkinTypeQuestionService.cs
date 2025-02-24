using SPSS.DAL.Models;
using SPSS.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.BLL.Services
{
    public class SkinTypeQuestionService
    {
        private SkinTypeQuestionRepository _skinTypeQuestionRepository = new();

        public List<SkinTypeQuestion> GetAllSkinTypeQuestions()
        {
            return _skinTypeQuestionRepository.GetAllQuestions();
        }
        public SkinTypeQuestion GetQuestion(int id)
        {
            return _skinTypeQuestionRepository.GetQuestion(id);
        }

        public Dictionary<string, string> GetAnswerOptions(SkinTypeQuestion question)
        {
            return _skinTypeQuestionRepository.GetAnswerOptions(question);
        }
    }
}
