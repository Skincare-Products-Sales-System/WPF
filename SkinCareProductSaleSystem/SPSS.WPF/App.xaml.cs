using SPSS.DAL.Models;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SPSS.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		public static User AuthenticatedUser { get; set; }

	}

}
