using System;
using System.Threading;
using DominionXamarinForms;

namespace DominionXamarin.Droid
{
	public class AndroidThreadHelper : ThreadHelper
	{
		private Thread thread;
		public override void Go()
		{
			this.thread.Start ();
		}

		public override void SetFunc(function f)
		{
			this.thread = new Thread(new ThreadStart(f));	
		}

		public override void Abort()
		{
			if (this.thread != null) {
				this.thread.Abort ();
			}
		}
	}
}

