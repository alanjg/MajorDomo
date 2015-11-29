using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;
using System.Threading;

namespace Dominion
{
	/*
	public class DefaultChooser : ChooserBase
	{
		ManualResetEvent waitHandle = new ManualResetEvent(true);
		private static DefaultChooser defaultChooser;

		protected DefaultChooser()
		{
		}

		public static IChooser Instance
		{
			get
			{
				if (defaultChooser == null)
				{
					defaultChooser = new DefaultChooser();
				}
				return defaultChooser;
			}
		}

		public override void ChooseBoolean<T>(string choiceTitle, string choiceText, T choice, BooleanChoiceType choiceType, HandleChoice<bool> choiceHandler)
		{
			bool chosen = false;
			waitHandle.Reset();
			App.UIDispatcher.BeginInvoke(new Action(
				() =>
			{
				DialogPrompt.Prompt(choiceTitle, choiceText, choiceResult =>
				{
					chosen = choiceResult; 
					waitHandle.Set();
				});
			}));
			waitHandle.WaitOne();
			choiceHandler(chosen);
		}

		protected override void MakeChoice<T>(string choiceText, IEnumerable<T> choices, int minChoices, int maxChoices, ChoiceType choiceType, HandleChoices<T> choiceHandler)
		{			
			IEnumerable<T> selectedChoices = null;
			waitHandle.Reset();
			App.UIDispatcher.BeginInvoke(new Action(
				() =>
				{
					ChooserWindow<T> window = new ChooserWindow<T>(choiceText, choices.Select(c => c), minChoices, maxChoices);
					window.Closed += (o, e) =>
					{
						if (window.DialogResult.HasValue && window.DialogResult.Value)
						{
							selectedChoices = window.SelectedChoices;				
						}
						waitHandle.Set();
					};
					window.Show();
				}));
			waitHandle.WaitOne();
			choiceHandler(selectedChoices);
		}
	}
	 * */
}
