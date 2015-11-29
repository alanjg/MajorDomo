using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace DominionJupiter
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is bool)
			{
				return (bool)value ? Visibility.Visible : Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}

	public class BoolToCollapsedConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is bool)
			{
				return (bool)value ? Visibility.Collapsed : Visibility.Visible;
			}
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}

	public class BoolToBrushConverter : IValueConverter
	{
		public Brush TrueValue { get; set; }
		public Brush FalseValue { get; set; }
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is bool)
			{
				return (bool)value ? this.TrueValue : this.FalseValue;
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
	public class CardTypeToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is CardType)
			{
				CardType type = (CardType)value;
				if ((type & CardType.Reaction) != 0) return new SolidColorBrush(Colors.Blue);
				if ((type & CardType.Victory) != 0) return new SolidColorBrush(Colors.Green);
				if ((type & CardType.Treasure) != 0) return new SolidColorBrush(Colors.Yellow);
				if ((type & CardType.Duration) != 0) return new SolidColorBrush(Colors.Orange);
				if ((type & CardType.Curse) != 0) return new SolidColorBrush(Colors.Purple);
				if ((type & CardType.Action) != 0) return new SolidColorBrush(Colors.White);
			}
			return new SolidColorBrush(Colors.White);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
	public class IntToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is int)
			{
				return ((int)value) != 0 ? Visibility.Visible : Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
	public class CardUseTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is CardUseType)
			{
				CardUseType type = (CardUseType)value;
				switch (type)
				{
					case CardUseType.Random:
						return "Random";
					case CardUseType.RandomByCardsFromSet:
						return "Random by cards from set";
					case CardUseType.Use:
						return "Always used";
					case CardUseType.DoNotUse:
						return "Never used";
				}
			}
			return "Random by cards from set";
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}

	public class StartingHandTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is StartingHandType)
			{
				StartingHandType type = (StartingHandType)value;
				switch (type)
				{
					case StartingHandType.Random:
						return "Random";
					case StartingHandType.RandomSameStartingHands:
						return "Random with same split";
					case StartingHandType.FourThreeSplit:
						return "Both have Four/Three Coppers";
					case StartingHandType.FiveTwoSplit:
						return "Both have Five/Two Coppers";
				}
			}
			return "Random";
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}	
}
