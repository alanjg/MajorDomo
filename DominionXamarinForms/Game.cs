using System;
using Dominion;

namespace DominionXamarinForms
{
	public abstract class ThreadHelper
	{
		public delegate void function();
		public abstract void Go();
		public abstract void SetFunc(function f);
		public abstract void Abort();
	}

	public class LocalGame : Game
	{
		public LocalGame()
		{
			LocalGamePageModel gamePageModel = new LocalGamePageModel();
			this.GamePageModel = gamePageModel;
		}

		public override void PlayGame()
		{
			App.ThreadHelperInstance.SetFunc(this.gameThreadStart);
			this.GamePageModel.SetupGame();
			App.ThreadHelperInstance.Go();
		}

		private void gameThreadStart()
		{
			this.GamePageModel.PlayGame();
		}

		public override void CancelGame()
		{
			App.ThreadHelperInstance.Abort();
		}

		public override void ExitGame()
		{
			App.CurrentGame = null;
		}
	}
}

