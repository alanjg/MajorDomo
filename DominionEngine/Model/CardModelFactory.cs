using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace Dominion
{
	public static class CardModelFactory
	{
		private static Dictionary<string, Type> nameLookup = null;
		private static Dictionary<string, Type> idLookup = null;
		private static List<CardModel> allCards = null;

		private static void EnsureDictionary()
		{
			if (nameLookup == null)
			{
				nameLookup = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
				idLookup = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
				allCards = new List<CardModel>();
#if JUPITER
				foreach (TypeInfo typeInfo in typeof(CardModelFactory).GetTypeInfo().Assembly.DefinedTypes)
				{
					Type type = typeInfo.AsType();
					if (typeof(CardModel).GetTypeInfo().IsAssignableFrom(typeInfo) && !typeInfo.IsAbstract)
#elif PCL
                foreach (TypeInfo typeInfo in typeof(CardModelFactory).GetTypeInfo().Assembly.DefinedTypes)
                {
                    Type type = typeInfo.AsType();
                    if (typeof(CardModel).GetTypeInfo().IsAssignableFrom(typeInfo) && !typeInfo.IsAbstract)
#else
				foreach (Type type in typeof(CardModelFactory).Assembly.GetTypes())
				{
					if (typeof(CardModel).IsAssignableFrom(type) && !type.IsAbstract)
#endif

                    {
                    try
						{
							CardModel card = (CardModel)Activator.CreateInstance(type);
							nameLookup[card.Name] = type;
							idLookup[card.ID] = type;
							allCards.Add(card);
						}
						catch
						{
						}
					}
				}
			}
		}

		public static List<CardModel> AllCards
		{
			get
			{
				EnsureDictionary();
				return allCards;
			}
		}

		public static CardModel GetCardModel(string name)
		{
			EnsureDictionary();
			Type cardType = null;
			if (!nameLookup.TryGetValue(name, out cardType))
			{
				cardType = idLookup[name];
			}
			if (cardType != null)
			{
				return (CardModel)Activator.CreateInstance(cardType);
			}
			else
			{
				return null;
			}
		}
	}
}