using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Dominion
{
	public class LogUpdatedEventArgs : EventArgs
	{
		public string Text { get; set; }
		public bool AddedTurn { get; set; }
	}

	public delegate void LogTextEventHandler(object sender, LogUpdatedEventArgs e);

	public class LogTurn
	{
		public ObservableCollection<string> Lines { get; private set; }
		public LogTurn()
		{
			this.Lines = new ObservableCollection<string>();
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			foreach (string line in this.Lines)
			{
				builder.Append(line);
			}
			return builder.ToString();
		}
	}

	public class Log
	{
		private class SuppressLogToken : IDisposable
		{
			private Log log;
			public SuppressLogToken(Log log)
			{
				this.log = log;
				this.log.suppressLogCounter++;
			}

			public void Dispose()
			{
				this.log.suppressLogCounter--;
			}
		}

		private int suppressLogCounter = 0;
		public bool IsSuppressingLogging { get { return this.suppressLogCounter != 0; } }

		public event LogTextEventHandler LogUpdated;

		private StringBuilder builder;
		private StringBuilder currentLineBuilder; 

		private ObservableCollection<LogTurn> turns = new ObservableCollection<LogTurn>();
		public ObservableCollection<LogTurn> Turns
		{
			get { return this.turns; }
		}

		public LogTurn CurrentTurn
		{
			get { return this.turns[this.turns.Count - 1]; }
		}

		public string Text
		{
			get { return builder.ToString(); }
		}

		public Log()
		{
			this.builder = new StringBuilder();
			this.currentLineBuilder = new StringBuilder();
			this.turns.Add(new LogTurn());
		}

		public IDisposable SuppressLogging()
		{
			return new SuppressLogToken(this);
		}

		public void Write(string text)
		{
			if (!this.IsSuppressingLogging)
			{
				this.builder.Append(text);
				this.currentLineBuilder.Append(text);
			}
		}

		public void WriteLine(string text)
		{
			if (!this.IsSuppressingLogging)
			{
				this.builder.AppendLine(text);
				this.currentLineBuilder.AppendLine(text);
				string currentLine = this.currentLineBuilder.ToString();
				this.CurrentTurn.Lines.Add(currentLine);
				this.currentLineBuilder.Clear();
				this.OnLogUpdated(currentLine, false);
			}
		}

		public void WriteLine()
		{
			if (!this.IsSuppressingLogging)
			{
				this.builder.AppendLine();
				this.currentLineBuilder.AppendLine();
				string currentLine = this.currentLineBuilder.ToString();
				this.currentLineBuilder.Clear();
				this.CurrentTurn.Lines.Add(currentLine);
				this.OnLogUpdated(currentLine, false);
			}
		}

		public void StartTurn()
		{
			if (!this.IsSuppressingLogging)
			{
				Debug.Assert(this.currentLineBuilder.Length == 0);
				this.turns.Add(new LogTurn());
				this.currentLineBuilder.Clear();
				this.OnLogUpdated(string.Empty, true);
			}
		}

		public static string FormatSortedCards(IEnumerable<CardModel> cards)
		{
			Dictionary<string, CardModel> namedCards = new Dictionary<string, CardModel>();
#if !SILVERLIGHTDESKTOP
			SortedDictionary<string, int> counts = new SortedDictionary<string, int>();
#else
			Dictionary<string, int> counts = new Dictionary<string, int>();
#endif
			foreach (CardModel card in cards)
			{
				int count = 0;
				counts.TryGetValue(card.Name, out count);
				counts[card.Name] = count + 1;
				namedCards[card.Name] = card;
			}

			string play = "";
			int index = 0;
			foreach (KeyValuePair<string, int> pair in counts)
			{
				index++;
				if (pair.Value == 1)
				{
					if (IsVowel(pair.Key[0]))
					{
						play += "an ";
					}
					else
					{
						play += "a ";
					}
				}
				else
				{
					play += pair.Value + " ";
				}
				
				if (pair.Value == 1)
				{
					play += pair.Key;
				}
				else
				{
					play += namedCards[pair.Key].PluralName;
				}
				if (index != counts.Count && index != counts.Count - 1)
				{
					play += ", ";
				}
				if (index == counts.Count - 1)
				{
					play += " and ";
				}
			}
			return play;
		}

		public static bool IsVowel(char c)
		{
			c = char.ToUpper(c);
			return c == 'A' || c == 'E' || c == 'I' || c == 'O' || c == 'U';
		}

		public void WriteSortedCards(IEnumerable<CardModel> cards)
		{
			if (!this.IsSuppressingLogging)
			{
				this.Write(Log.FormatSortedCards(cards));
			}
		}

		private void OnLogUpdated(string text, bool addedTurn)
		{
			if (!this.IsSuppressingLogging)
			{
				if (this.LogUpdated != null)
				{
					this.LogUpdated(this, new LogUpdatedEventArgs() { Text = text, AddedTurn = addedTurn });
				}
			}
		}
	}

	public class Logger
	{
		private Log globalLog;
		private List<Log> playerLogs;
		public Logger(GameModel gameModel, int playerCount)
		{
			this.globalLog = new Log();
			this.playerLogs = new List<Log>();
			for(int i=0;i<playerCount;i++)
			{
				this.playerLogs.Add(new Log());
			}
		}
	}
}
