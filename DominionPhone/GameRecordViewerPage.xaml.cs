﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DominionPhone
{
	public partial class GameRecordViewerPage : PhoneApplicationPage
	{
		public GameRecordViewerPage()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			this.DataContext = RecordsPage.CurrentRecord;
		}
	}
}