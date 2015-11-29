using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DominionSL
{
    public partial class GameTypePicker : ChildWindow
    {
		public GameTypePicker()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

		public bool IsServerGame
		{
			get { return this.ServerRadioButton.IsChecked == true; }
		}

		public string ServerAddress
		{
			get { return this.AddressTextBox.Text; }
		}

		public string Username
		{
			get { return this.UsernameTextBox.Text; }
		}
    }
}

