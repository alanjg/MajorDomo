using Dominion.Model.Actions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dominion
{
	public class CollectionSynchronizer
	{
		private IList<CardModel> sourceCollection;
		private ObservableCollection<CardViewModel> targetCollection;
		private object collectionSyncLock = new object();
		private List<CardModel> pendingCollectionRemoves = new List<CardModel>();
		private List<CardModel> pendingCollectionAdds = new List<CardModel>();

		private bool sort;
		public CollectionSynchronizer(IList<CardModel> sourceCollection, ObservableCollection<CardViewModel> targetCollection)
			:this(sourceCollection, targetCollection, true)
		{
		}

		public CollectionSynchronizer(IList<CardModel> sourceCollection, ObservableCollection<CardViewModel> targetCollection, bool sort)
		{
			this.sourceCollection = sourceCollection;
			this.targetCollection = targetCollection;
			this.sort = sort;

			((INotifyCollectionChanged)this.sourceCollection).CollectionChanged += sourceCollection_CollectionChanged;
		}

		private void sourceCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			lock (this.collectionSyncLock)
			{ 
				if (e.Action == NotifyCollectionChangedAction.Reset)
				{
					this.pendingCollectionAdds.Clear();
					this.pendingCollectionRemoves.Clear();
					this.pendingCollectionRemoves.AddRange(targetCollection.Select(c => c.CardModel));
					this.pendingCollectionAdds.AddRange(sourceCollection);
				}
				else if (e.Action == NotifyCollectionChangedAction.Add)
				{
					foreach (CardModel card in e.NewItems)
					{
						if (!this.pendingCollectionRemoves.Remove(card))
						{
							this.pendingCollectionAdds.Add(card);
						}
					}
				}
				else if (e.Action == NotifyCollectionChangedAction.Remove)
				{
					foreach (CardModel card in e.OldItems)
					{
						if (!this.pendingCollectionAdds.Remove(card))
						{
							this.pendingCollectionRemoves.Add(card);
						}
					}
				}
			}

			ViewModelDispatcher.BeginInvoke(new Action(delegate()
			{
				lock (this.collectionSyncLock)
				{
					foreach (CardModel card in this.pendingCollectionRemoves)
					{
						if (this.sort)
						{
							int index = BinarySearch(targetCollection, card);
							Debug.Assert(index < targetCollection.Count, "CollectionSynchronizer's binary search is broken");
							if (targetCollection[index].CardModel != card)
							{
								for (int delta = 1; delta < targetCollection.Count; delta++)
								{
									if(index - delta >= 0 && targetCollection[index - delta].CardModel == card)
									{
										index = index - delta;
										break;
									}
									else if(index + delta < targetCollection.Count && targetCollection[index + delta].CardModel == card)
									{
										index = index + delta;
										break;
									}
								}
							}
							targetCollection.RemoveAt(index);
						}
						else
						{
							targetCollection.Remove(targetCollection.First(item => item.CardModel == card));
						}
					}

					foreach (CardModel card in this.pendingCollectionAdds)
					{
						CardViewModel cvm = new CardViewModel(card);
						if (this.sort)
						{
							int index = BinarySearch(targetCollection, cvm.CardModel);
							targetCollection.Insert(index, cvm);
#if DEBUG
							for (int i = 0; i + 1 < targetCollection.Count; i++)
							{
								Debug.Assert(CollectionSynchronizer.comparer.Compare(targetCollection[i].CardModel, targetCollection[i + 1].CardModel) <= 0, "CollectionSynchronizer's binary search is broken");
							}
#endif
						}
						else
						{
							targetCollection.Add(cvm);
						}
					}
					this.pendingCollectionAdds.Clear();
					this.pendingCollectionRemoves.Clear();
				}
			}));
		}

		public static int LinearSearch(ObservableCollection<CardViewModel> collection, CardModel item)
		{
			int index = 0;
			while (index < collection.Count)
			{
				int comp = CollectionSynchronizer.comparer.Compare(collection[index].CardModel, item);
				if (comp >= 0)
				{
					return index;
				}
				index++;
			}
			return index;
		}


		private int BinarySearch(ObservableCollection<CardViewModel> collection, CardModel item)
		{
			int low = 0;
			int high = collection.Count;
			while (low < high)
			{
				int mid = (low + high) / 2;
				int result = CollectionSynchronizer.comparer.Compare(collection[mid].CardModel, item);
				if (result == 0)
				{
					return mid;
				}
				else if (result < 0)
				{
					low = mid + 1;
				}
				else
				{
					high = mid - 1;
				}
			}
			if (low < collection.Count && CollectionSynchronizer.comparer.Compare(collection[low].CardModel, item) < 0)
			{
				return low + 1;
			}
			else
			{
				return low;
			}
		}

		private static HandCardComparer comparer = new HandCardComparer();
	}
}
