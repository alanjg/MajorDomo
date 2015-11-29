using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Dominion;
using Dominion.Model.Actions;

namespace Dominion
{
	public class LogViewModel : NotifyingObject
	{
		private Log log;
		private ObservableCollection<LogTurn> turns;
		private object logLock = new object();
		public LogViewModel(Log log)
		{
			this.log = log;
			this.turns = new ObservableCollection<LogTurn>();
			this.turns.Add(new LogTurn());
			this.log.LogUpdated += log_LogUpdated;
		}

		private void log_LogUpdated(object sender, LogUpdatedEventArgs e)
		{
			ViewModelDispatcher.BeginInvoke(new Action(delegate()
			{				
				if (e.AddedTurn)
				{
					this.turns.Add(new LogTurn());
				}
				else
				{
					this.turns[this.turns.Count - 1].Lines.Add(e.Text.Trim());
				}
				this.OnPropertyChanged("Text");				
			}));
			
		}

		public string Text
		{
			get { return this.log.Text; }
		}

		public ObservableCollection<LogTurn> Turns
		{
			get
			{
				return this.turns;
			}
		}
	}
}
