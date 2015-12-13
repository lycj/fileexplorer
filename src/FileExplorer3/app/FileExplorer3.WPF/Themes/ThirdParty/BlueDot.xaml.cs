using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace FileExplorer.WPF.Animations
{
	
	public partial class BlueDot : UserControl
	{private Storyboard _sb ;
	public BlueDot()
		{
			InitializeComponent();
			_sb = FindResource("GrowStoryBoard") as Storyboard;
		

		}
		#region BeginTime

		/// <summary>
		/// BeginTime Dependency Property
		/// </summary>
		public static readonly DependencyProperty BeginTimeProperty =
				DependencyProperty.Register("BeginTime", typeof(TimeSpan), typeof(BlueDot),
						new FrameworkPropertyMetadata((TimeSpan)TimeSpan.FromMilliseconds(0),
								new PropertyChangedCallback(OnBeginTimeChanged)));

		/// <summary>
		/// Gets or sets the BeginTime property.  This dependency property 
		/// indicates what time the animation should start.
		/// </summary>
		public TimeSpan BeginTime
		{
			get { return (TimeSpan)GetValue(BeginTimeProperty); }
			set { SetValue(BeginTimeProperty, value); }
		}

		/// <summary>
		/// Handles changes to the BeginTime property.
		/// </summary>
		private static void OnBeginTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((BlueDot)d).OnBeginTimeChanged(e);
		}

		/// <summary>
		/// Provides derived classes an opportunity to handle changes to the BeginTime property.
		/// </summary>
		protected virtual void OnBeginTimeChanged(DependencyPropertyChangedEventArgs e)
		{
			_sb.BeginTime = (TimeSpan)e.NewValue;
		}

		#endregion

        

        
	}
}
