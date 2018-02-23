using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Actions;

namespace Dominion
{
    public class HandCardComparer : IComparer<CardModel>
    {
        public int Compare(CardModel x, CardModel y)
        {
            if (x.Is(CardType.Action))
            {
                if (y.Is(CardType.Action))
                {
                    string xName = x.Name;
                    string yName = y.Name;
                    if (x is BandOfMisfits) xName = "BandOfMisfits";
                    if (y is BandOfMisfits) yName = "BandOfMisfits";
                    return xName.CompareTo(yName);
                }
                else
                {
                    return -1;
                }
            }
            else if (x.Is(CardType.Treasure))
            {
                if (y.Is(CardType.Action))
                {
                    return 1;
                }
                else if (y.Is(CardType.Treasure))
                {
                    return x.Name.CompareTo(y.Name);
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y.Is(CardType.Action) || y.Is(CardType.Treasure))
                {
                    return 1;
                }
                else
                {
                    return x.Name.CompareTo(y.Name);
                }
            }
        }
    }
}
