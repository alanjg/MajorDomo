using Dominion;
using Dominion.Model.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionEngine.AI
{
	public class BuyListTrainingAIStrategy : BuyListAIStrategy
	{
		private int generations;
		private int leaders;
		private int challengers;
		private int gamesPerMatchup;
		private bool mutateFinalThreshold;

		public BuyListTrainingAIStrategy(GameModel gameModel, int generations, int leaders, int challengers, int gamesPerMatchup, bool mutateFinalThreshold)
			: base(gameModel)
		{
			((BaseAIChooser)this.Chooser).SetStrategy(this);
			this.generations = generations;
			this.leaders = leaders;
			this.challengers = challengers;
			this.gamesPerMatchup = gamesPerMatchup;
			this.mutateFinalThreshold = true;
		}

		public void Train(Kingdom kingdom, bool isFourThreeSplit)
		{
			BuyListTrainer trainer = new BuyListTrainer(kingdom, isFourThreeSplit);
			BuyListTrainer.GamesPerMatchup = this.gamesPerMatchup;
			BuyListTrainer.MaxChallengerCount = this.challengers;
			BuyListTrainer.MaxLeaderCount = this.leaders;
			List<BuyListEntry> best = trainer.Train(this.generations);
			if (this.mutateFinalThreshold)
			{
				best = trainer.TrainThresholds();
			}
			this.BuyList = best.Last().BuyList;
		}

		public override void OnGameStart(Kingdom kingdom)
		{
			this.GameModel.TextLog.StartTurn();
			this.GameModel.TextLog.WriteLine("Computer is planning its strategy...");
			int copperCount = this.Player.Hand.Count(c => c is Copper);
			this.Train(kingdom, copperCount == 3 || copperCount == 4);
			this.GameModel.TextLog.WriteLine("Planning complete!");
		}
	}
}
