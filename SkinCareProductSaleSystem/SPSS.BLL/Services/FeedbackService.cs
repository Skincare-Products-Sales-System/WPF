using SPSS.DAL.Models;
using SPSS.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.BLL.Services
{
	public class FeedbackService
	{
		private FeedbackRepository _repository = new();

		public List<Feedback> GetFeedbackByProductId(int id)
		{
			return _repository.GetFeedbackByProductId(id);
		}

		public void AddFeedback(Feedback feedback)
		{
			_repository.CreateFeedback(feedback);
		}
	}
}
