using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Dominion;
using Dominion.Model.Actions;

namespace DominionSL
{
	public class LogViewModel : INotifyPropertyChanged
	{
		private Log log;
		public LogViewModel(Log log)
		{
			this.log = log;
			this.log.LogUpdated += log_LogUpdated;
		}

		void log_LogUpdated(object sender, LogUpdatedEventArgs e)
		{
			App.UIDispatcher.BeginInvoke(new Action(delegate()
			{
				this.OnPropertyChanged("Text");//e.Text
			}));
		
		}


		public string Text
		{
			get { return this.log.Text; }
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion
	}
}
