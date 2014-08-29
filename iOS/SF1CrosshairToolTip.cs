using System;
using System.Drawing;
using ShinobiCharts;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace ShinobiF1Project.iOS
{
	public partial class SF1CrosshairToolTip : SChartCrosshairTooltip
	{
		public ShinobiChart TooltipChart;
		public SF1Datasouce Datasource;

		public SF1CrosshairToolTip (SF1Datasouce datasource)
		{
			Datasource = datasource;
			SetUpChart ();
		}

		public void SetUpChart() {
			RectangleF chartFrame = new RectangleF();
			chartFrame.Location = new PointF(5.0f, 0);
			chartFrame.Size = new SizeF(400f, 200f);
			TooltipChart = new ShinobiChart(chartFrame);

			// Insert License key here
//			TooltipChart.LicenseKey = "";

			TooltipChart.DataSource = Datasource;
			TooltipChart.Title = "";

			// xAxis
			SChartCategoryAxis x = new SChartCategoryAxis();
			x.AxisPosition = SChartAxisPosition.Reverse;
			TooltipChart.XAxis = x;

			// Temperature yAxis °C
			SChartNumberAxis y1 = new SChartNumberAxis();
			y1.DefaultRange = new SChartNumberRange(new NSNumber(0), new NSNumber(100));
			y1.MajorTickFrequency = new NSNumber(20);
			y1.LabelFormatString = new NSString("%.1f°C");
			TooltipChart.YAxis = y1;

			// Temperature yAxis °F
			SChartNumberAxis y2 = new SChartNumberAxis();
			y2.DefaultRange = new SChartNumberRange(new NSNumber(32), new NSNumber(212));
			y2.MajorTickFrequency = new NSNumber(40);
			y2.LabelFormatString = new NSString("%.1f°F");
			y2.AxisPosition = SChartAxisPosition.Reverse;
			TooltipChart.AddYAxis(y2);

			// Legend
			TooltipChart.Legend.Hidden = false;
			TooltipChart.Legend.Style.Font = UIFont.SystemFontOfSize (10);
			TooltipChart.Legend.Style.HorizontalPadding = new NSNumber(1.0);
			TooltipChart.Legend.Style.VerticalPadding = new NSNumber(1.0);
			TooltipChart.Legend.Position = SChartLegendPosition.BottomMiddle;
			TooltipChart.Legend.MaxSeriesPerLine = 4;
			TooltipChart.Legend.AutosizeLabels = true;
			TooltipChart.Legend.Style.MarginWidth = new NSNumber(10.0);

			AddSubview(TooltipChart);
		}

		public override void LayoutContents ()
		{
			base.LayoutContents ();

			// Layer tweaks
			Layer.CornerRadius = 5f;
			Layer.BorderWidth = 3f;
			Layer.BorderColor = UIColor.Gray.CGColor;

			// Background Color
			BackgroundColor = UIColor.White;

			// Configure Label (from superclass)
			Label.Font = UIFont.SystemFontOfSize(12);
			Label.TextColor = UIColor.Black;
			Label.BackgroundColor = UIColor.White;
			Label.SizeToFit();

			// Resize tooltip
			RectangleF tempFrame = Frame;
			tempFrame.Size = new SizeF (TooltipChart.Frame.Size.Width + 10f, Label.Frame.Size.Height + TooltipChart.Frame.Size.Height + 15f);
			Frame = tempFrame;

			// Position Label
			tempFrame = Label.Frame;
			tempFrame.Location = new PointF ((Frame.Size.Width - tempFrame.Size.Width)/2f, 5f);
			Label.Frame = tempFrame;

			// Position Chart
			tempFrame = TooltipChart.Frame;
			tempFrame.Location = new PointF (5f, Label.Frame.Size.Height + 10f);
			TooltipChart.Frame = tempFrame;
		}

		protected override void OnSettingPosition (SChartPoint pos, SChartCanvas canvas)
		{
			LayoutContents ();

			// Position tooltip
			RectangleF tempFrame = Frame;
			float x, y;
			x = (float)pos.X - tempFrame.Size.Width/2f;

			// Keep within horizontal bounds
			if (x + tempFrame.Size.Width > canvas.GLView.Frame.Location.X + canvas.GLView.Bounds.Size.Width)  {
				x = canvas.GLView.Frame.Location.X + canvas.GLView.Bounds.Size.Width - tempFrame.Size.Width;
			} else if (x < 0f) {
				x = 0f;
			}
			// Pin underneath chart
			y = Datasource.MainChart.Frame.Location.Y + Datasource.MainChart.Frame.Size.Height;

			tempFrame.Location = new PointF (x, y);

			Frame = tempFrame;
		}

		protected override void OnSettingDataPoint (SChartData dataPoint, SChartSeries series, ShinobiChart chart)
		{
			Datasource.setTooltipSeries (series);
			TooltipChart.ReloadData ();
			TooltipChart.RedrawChart ();
			Label.Text = "Car " + series.Title + " at " + chart.XAxis.StringForId(dataPoint.XValue);
		}
	}
}