using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DominionPhone
{
	public class ExceptionModel
	{
		public string Message { get; set; }
		public string StackTrace { get; set; }
	}

	public partial class ErrorPage : PhoneApplicationPage
	{
		public static Exception LastException { get; set; }
		public ErrorPage()
		{
			InitializeComponent();
			this.Exceptions = new ObservableCollection<ExceptionModel>();
			this.DataContext = this;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			Exception ex = LastException;
			while (ex != null)
			{
				this.Exceptions.Add(new ExceptionModel() { Message = ex.Message, StackTrace = ex.StackTrace });
				ex = ex.InnerException;
			}
		}

		public ObservableCollection<ExceptionModel> Exceptions { get; private set; }
	}
}