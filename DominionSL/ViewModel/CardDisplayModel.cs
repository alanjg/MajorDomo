using System;

namespace DominionSL
{
	public abstract class CardDisplayModel
	{
		public string ImagePath { get; set; }
		public string Text { get; set; }
	}

	public class ImageCardDisplayModel : CardDisplayModel
	{
		public ImageCardDisplayModel(string path) { this.ImagePath = path; this.Text = string.Empty; }
	}

	public class TextCardDisplayModel : CardDisplayModel
	{
		public TextCardDisplayModel(string text) { this.Text = text; this.ImagePath = @"Images/blank.png"; }
	}
}
