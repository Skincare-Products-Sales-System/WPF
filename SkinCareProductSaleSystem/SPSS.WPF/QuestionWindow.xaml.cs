using SPSS.BLL.Services;
using SPSS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SPSS.WPF
{
	public partial class QuestionWindow : Window
	{
		private readonly UserService _userService;
		private readonly UserAnswerService _userAnswerService;
		private readonly SkinTypeQuestionService _skinTypeQuestionService;
		private int _currentQuestionIndex;
		private List<SkinTypeQuestion> _questions;
		private Dictionary<int, string> _userAnswers;

		public User AuthenticatedUser { get; set; }

		public QuestionWindow()
		{
			InitializeComponent();

			// Initialize services
			_userService = new UserService();
			_userAnswerService = new UserAnswerService();
			_skinTypeQuestionService = new SkinTypeQuestionService();
			_userAnswers = new Dictionary<int, string>();

			LoadQuestions();
			InitializeQuiz();
		}

		private void LoadQuestions()
		{
			try
			{
				_questions = _skinTypeQuestionService.GetAllSkinTypeQuestions();
				if (_questions == null || !_questions.Any())
				{
					MessageBox.Show("No questions found. Please check the database connection.", "Error",
						MessageBoxButton.OK, MessageBoxImage.Error);
					Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading questions: {ex.Message}", "Error",
					MessageBoxButton.OK, MessageBoxImage.Error);
				Close();
			}
		}

		private void InitializeQuiz()
		{
			_currentQuestionIndex = 0;
			UpdateProgressBar();
			DisplayQuestion(_currentQuestionIndex);
		}

		private void DisplayQuestion(int index)
		{
			if (!IsValidQuestionIndex(index))
			{
				HandleInvalidQuestion();
				return;
			}

			var question = _questions[index];
			textblockQuestion.Text = $"Question {index + 1} of {_questions.Count}\n\n{question.QuestionText}";

			DisplayAnswerOptions(question);
			UpdateNavigationButtons();
			RestoreUserAnswer();
		}

		private bool IsValidQuestionIndex(int index)
		{
			return index >= 0 && index < _questions.Count;
		}

		private void HandleInvalidQuestion()
		{
			textblockQuestion.Text = "No more questions available.";
			DisableAllOptions();
			UpdateNavigationButtons();
		}

		private void DisplayAnswerOptions(SkinTypeQuestion question)
		{
			var options = _skinTypeQuestionService.GetAnswerOptions(question);

			UpdateRadioButton(radioButtonA, "A", options);
			UpdateRadioButton(radioButtonB, "B", options);
			UpdateRadioButton(radioButtonC, "C", options);
			UpdateRadioButton(radioButtonD, "D", options);
		}

		private void UpdateRadioButton(RadioButton radioButton, string optionKey, Dictionary<string, string> options)
		{
			if (options.ContainsKey(optionKey))
			{
				radioButton.Content = options[optionKey];
				radioButton.IsEnabled = true;
				radioButton.Visibility = Visibility.Visible;
			}
			else
			{
				radioButton.IsEnabled = false;
				radioButton.Visibility = Visibility.Collapsed;
			}
			radioButton.IsChecked = false;
		}

		private void DisableAllOptions()
		{
			radioButtonA.IsEnabled = radioButtonB.IsEnabled =
			radioButtonC.IsEnabled = radioButtonD.IsEnabled = false;
		}

		private void UpdateNavigationButtons()
		{
			btnPrevious.Visibility = (_currentQuestionIndex == 0) ?
				Visibility.Collapsed : Visibility.Visible;

			bool isLastQuestion = _currentQuestionIndex == _questions.Count - 1;
			btnNext.Visibility = isLastQuestion ? Visibility.Collapsed : Visibility.Visible;
			btnSubmit.Visibility = isLastQuestion ? Visibility.Visible : Visibility.Collapsed;
		}

		private void RestoreUserAnswer()
		{
			if (_userAnswers.TryGetValue(_currentQuestionIndex, out string selectedAnswer))
			{
				var radioButtons = new[] { radioButtonA, radioButtonB, radioButtonC, radioButtonD };
				var optionLetters = new[] { "A", "B", "C", "D" };

				for (int i = 0; i < radioButtons.Length; i++)
				{
					if (optionLetters[i] == selectedAnswer)
					{
						radioButtons[i].IsChecked = true;
						break;
					}
				}
			}
		}

		private void Option_Click(object sender, RoutedEventArgs e)
		{
			if (sender is RadioButton selectedOption && selectedOption.IsChecked == true)
			{
				string answer = selectedOption == radioButtonA ? "A" :
							  selectedOption == radioButtonB ? "B" :
							  selectedOption == radioButtonC ? "C" : "D";

				_userAnswers[_currentQuestionIndex] = answer;
				UpdateProgressBar();
			}
		}

		private void UpdateProgressBar()
		{
			double progress = ((double)_userAnswers.Count / _questions.Count) * 100;
			progressBarQuest.Value = progress;
		}

		private void Previous_Click(object sender, RoutedEventArgs e)
		{
			if (_currentQuestionIndex > 0)
			{
				_currentQuestionIndex--;
				DisplayQuestion(_currentQuestionIndex);
			}
		}

		private void Next_Click(object sender, RoutedEventArgs e)
		{
			if (_currentQuestionIndex < _questions.Count - 1)
			{
				_currentQuestionIndex++;
				DisplayQuestion(_currentQuestionIndex);
			}
		}

		private void Submit_Click(object sender, RoutedEventArgs e)
		{
			if (!ValidateAllQuestionsAnswered())
			{
				MessageBox.Show("Please answer all questions before submitting.",
					"Incomplete Quiz", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (MessageBox.Show("Are you sure about your skin condition assessment?",
				"Confirm Submission", MessageBoxButton.YesNo, MessageBoxImage.Question)
				== MessageBoxResult.Yes)
			{
				ProcessQuizResults();
			}
		}

		private bool ValidateAllQuestionsAnswered()
		{
			return _userAnswers.Count == _questions.Count;
		}

		private void ProcessQuizResults()
		{
			try
			{
				string skinType = CalculateSkinType();
				SaveUserAnswers(skinType);

				MessageBox.Show($"Your skin type has been analyzed as: {skinType}\n\n" +
					"Thank you for completing the skin assessment!",
					"Analysis Complete", MessageBoxButton.OK, MessageBoxImage.Information);

				NavigateToMainWindow();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error saving results: {ex.Message}",
					"Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private string CalculateSkinType()
		{
			var answerCount = new Dictionary<string, int>
			{
				{ "A", 0 }, { "B", 0 }, { "C", 0 }, { "D", 0 }
			};

			foreach (var answer in _userAnswers.Values)
			{
				answerCount[answer.Trim()]++;
			}

			var mostSelected = answerCount.OrderByDescending(x => x.Value).First().Key;

			return mostSelected switch
			{
				"A" => "Oily (Da dầu)",
				"B" => "Dry (Da khô)",
				"C" => "Normal (Da thường)",
				"D" => "Combination (Da hỗn hợp)",
				_ => "Undefined"
			};
		}

		private void SaveUserAnswers(string skinType)
		{
			// Update user's skin type
			_userService.UpdateSkinType(AuthenticatedUser, skinType);

			// Save individual answers
			foreach (var answer in _userAnswers)
			{
				var userAnswer = new UserAnswer
				{
					QuestionId = _questions[answer.Key].QuestionId,
					AnswerText = answer.Value,
					UserId = AuthenticatedUser.UserId
				};
				_userAnswerService.Create(userAnswer);
			}
		}

		private void NavigateToMainWindow()
		{
			var mainWindow = new MainWindow { AuthenticatedUser = AuthenticatedUser };
			mainWindow.Show();
			Close();
		}
	}
}