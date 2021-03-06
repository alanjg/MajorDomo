﻿using System;
using DominionXamarinForms;
using System.Threading;
using Dominion;


namespace DominionXamarin.iOS
{
	public class iOSThreadHelper : ThreadHelper
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

