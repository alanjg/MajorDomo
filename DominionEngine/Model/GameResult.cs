using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
	public class PlayerResult
	{
		public Player Player { get; set; }
		public int Score { get; set; }
		public bool Won { get; set; }
		public int Turns { get; set; }
	}

	public class GameResult
	{
		public GameRecord ToGameRecord(Player player)
		{
			GameRecord record = new GameRecord();
			record.Name = player.Name;
			record.Won = this.ResultMap[player].Won;
			
			foreach (LogTurn turn in player.GameModel.TextLog.Turns)
			{
				foreach (string line in turn.Lines)
				{
					record.Log.Add(line.Trim());
				}
			}
			
			foreach (Player p in player.GameModel.Players)
			{
				PlayerRecord playerRecord = new PlayerRecord();
				playerRecord.Name = p.Name;
				playerRecord.Score = this.ResultMap[p].Score;
				playerRecord.Deck = Log.FormatSortedCards(p.AllCardsInDeck);
				record.Players.Add(playerRecord);
			}
			return record;
		}

		public List<Player> Winners { get; private set; }
		public List<PlayerResult> Results { get; private set; }
		public Dictionary<Player, PlayerResult> ResultMap { get; private set; }
		public GameResult(GameModel model)
		{
			this.ResultMap = new Dictionary<Player, PlayerResult>();
			this.Results = new List<PlayerResult>();
			this.Winners = new List<Player>();
			int highScore = int.MinValue;
			int minTurns = int.MaxValue;
			foreach (Player player in model.Players)
			{
				int points = player.Points;
				if(points > highScore || points == highScore && player.TurnCount < minTurns)
				{
					highScore = points;
					minTurns = player.TurnCount;
				}
			}

			foreach (Player player in model.Players)
			{				
				int points = player.Points;
				bool won = points == highScore && player.TurnCount == minTurns;
				PlayerResult result = new PlayerResult() { Player = player, Score = points, Turns = player.TurnCount, Won = won };
				this.Results.Add(result);
				this.ResultMap[player] = result;
				if(won)
				{
					this.Winners.Add(player);
				}
			}
		}
	}
}
