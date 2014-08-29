using System;
using ShinobiCharts;
using MonoTouch.Foundation;

namespace ShinobiF1Project.iOS
{
	public class SF1Delegate : SChartDelegate
	{
		SF1Datasouce Datasource;

		public SF1Delegate (SF1Datasouce datasource)
		{
			Datasource = datasource;
		}

		protected override void OnCrosshairMoved (ShinobiChart chart, MonoTouch.Foundation.NSObject x, MonoTouch.Foundation.NSObject y)
		{
			int val = ((NSNumber)x).IntValue / 10;
			Datasource.MainSelectedIndex = new NSNumber (val);
		}
	}
}

