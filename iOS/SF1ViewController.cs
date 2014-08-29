using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using ShinobiCharts;

namespace ShinobiF1Project.iOS
{

	public partial class SF1ViewController : UIViewController
	{
		ShinobiChart TyreHeatChart;
		SF1Datasouce Datasource;
		SF1Delegate Delegate;
		SF1CrosshairToolTip CustomTooltip;

		public SF1ViewController ()
		{
		}

		public override void ViewDidLoad() 
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.White;

			RectangleF frame = View.Frame;
			frame.Size = new SizeF(frame.Width, frame.Height-260);
			TyreHeatChart = new ShinobiChart (frame);
			TyreHeatChart.AutoresizingMask = ~UIViewAutoresizing.None;

			TyreHeatChart.Title = @"Intermediate vs. Wet Tyre Temperature for Shinobi Cars";

			// Insert License key here
//			TyreHeatChart.LicenseKey = ""

			Datasource = new SF1Datasouce (TyreHeatChart);
			TyreHeatChart.DataSource = Datasource;
			Delegate = new SF1Delegate (Datasource);
			TyreHeatChart.Delegate = Delegate;

			SChartNumberAxis x = new SChartNumberAxis ();
			x.DefaultRange = new SChartNumberRange(new NSNumber(0), new NSNumber(1500));
			x.EnableGesturePanning = true;
			x.EnableMomentumPanning = true;
			x.EnableGestureZooming = true;
			x.EnableMomentumZooming = true;
			x.LabelFormatString = new NSString("T+%.0fms");
			TyreHeatChart.XAxis = x;

			SChartNumberAxis y = new SChartNumberAxis ();
			y.DefaultRange = new SChartNumberRange(new NSNumber(0), new NSNumber(125));
			y.LabelFormatString = new NSString("%.1f°C");
			TyreHeatChart.YAxis = y;

			View.AddSubview (TyreHeatChart);
			
			CustomTooltip = new SF1CrosshairToolTip (Datasource);
			TyreHeatChart.Crosshair.Tooltip = CustomTooltip;

			TyreHeatChart.Legend.Hidden = false;
			TyreHeatChart.Legend.Position = SChartLegendPosition.BottomMiddle;

			TyreHeatChart.ClipsToBounds = false;
		}
	}
}

