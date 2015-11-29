using Dominion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominionJupiter
{
	public class DifficultyModel : NotifyingObject
	{
		public DifficultyModel()
		{
			switch (this.ReadDifficulty())
			{
				case 0:
					this.Scout = true;
					break;
				case 1:
					this.Advisor = true;
					break;
				case 2:
					this.Tactician = true;
					break;
				case 3:
					this.Familiar = true;
					break;
				case 4:
					this.Golem = true;
					break;
				case 5:
					this.Witch = true;
					break;
				case 6:
					this.Goons = true;
					break;
			}
		}

		public int ReadDifficulty()
		{
			Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
			object val = localSettings.Values["difficulty"];
			if (val is int)
			{
				return (int)val;
			}
			else
			{
				return 1;
			}
		}

		public int GetDifficulty()
		{
			if (this.scout) return 0;
			if (this.advisor) return 1;
			if (this.tactician) return 2;
			if (this.familiar) return 3;
			if (this.golem) return 4;
			if (this.witch) return 5;
			if (this.goons) return 6;
			// shouldn't get here
			return 1;
		}

		public void WriteDifficulty()
		{
			Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
			localSettings.Values["difficulty"] = this.GetDifficulty();
		}
		private void ClearDifficulty()
		{
			this.scout = false;
			this.advisor = false;
			this.tactician = false;
			this.familiar = false;
			this.golem = false;
			this.witch = false;
			this.goons = false;
		}

		private bool scout;
		public bool Scout
		{
			get
			{
				return this.scout;
			}
			set
			{
				this.ClearDifficulty();
				this.scout = value;
				this.OnPropertyChanged("Scout");
			}
		}

		private bool advisor;
		public bool Advisor
		{
			get
			{
				return this.advisor;
			}
			set
			{
				this.ClearDifficulty();
				this.advisor = value;
				this.OnPropertyChanged("Advisor");
			}
		}

		private bool tactician;
		public bool Tactician
		{
			get
			{
				return this.tactician;
			}
			set
			{
				this.ClearDifficulty();
				this.tactician = value;
				this.OnPropertyChanged("Tactician");
			}
		}


		private bool familiar;
		public bool Familiar
		{
			get
			{
				return this.familiar;
			}
			set
			{
				this.ClearDifficulty();
				this.familiar = value;
				this.OnPropertyChanged("Familiar");
			}
		}

		private bool golem;
		public bool Golem
		{
			get
			{
				return this.golem;
			}
			set
			{
				this.ClearDifficulty();
				this.golem = value;
				this.OnPropertyChanged("Golem");
			}
		}

		private bool witch;
		public bool Witch
		{
			get
			{
				return this.witch;
			}
			set
			{
				this.ClearDifficulty();
				this.witch = value;
				this.OnPropertyChanged("Witch");
			}
		}

		private bool goons;
		public bool Goons
		{
			get
			{
				return this.goons;
			}
			set
			{
				this.ClearDifficulty();
				this.goons = value;
				this.OnPropertyChanged("Goons");
			}
		}
	}
}
