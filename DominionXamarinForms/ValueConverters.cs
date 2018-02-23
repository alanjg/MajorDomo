using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Xamarin.Forms;
using Dominion;

namespace DominionXamarinForms
{
    public class BoolToSizeConverter : IValueConverter
    {
        public double TrueValue { get; set; }
        public double FalseValue { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? this.TrueValue : this.FalseValue;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToColorConverter : IValueConverter
    {
        
        public Color TrueValue { get; set; }
        public Color FalseValue { get; set; }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? this.TrueValue : this.FalseValue;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CardTypeToColorConverter : IValueConverter
    {
        public Color ForegroundColor { get; set; }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is CardType)
            {
                CardType type = (CardType)value;
                if ((type & CardType.Reaction) != 0) return Color.Blue;
                if ((type & CardType.Victory) != 0) return Color.Green;
                if ((type & CardType.Treasure) != 0) return Color.Yellow;
                if ((type & CardType.Duration) != 0) return Color.Red;
                if ((type & CardType.Curse) != 0) return Color.Purple;
                if ((type & CardType.Action) != 0) return this.ForegroundColor;
            }
            return Color.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CardUseTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StartingHandTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
