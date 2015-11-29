using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
	public interface IDispatcher
	{
		void BeginInvoke(Action action);
	}

	public static class ViewModelDispatcher
	{
		public static void SetDispatcher(IDispatcher dispatcher)
		{
			ViewModelDispatcher.dispatcher = dispatcher;
		}
		private static IDispatcher dispatcher = null;

		public static void BeginInvoke(Action action)
		{
			if (ViewModelDispatcher.dispatcher != null)
			{
				dispatcher.BeginInvoke(action);
			}
			else
			{
				action();
			}
		}
	}
}
