using System;
using System.Collections.Generic;
using ShinobiCharts;
using MonoTouch.Foundation;


namespace ShinobiF1Project.iOS
{


	public class SF1Datasouce : SChartDataSource
	{

		public ShinobiChart MainChart, ToolTipChart;
		private Dictionary<string, NSNumber>[] S1HeatData, S2HeatData;
		private string ToolTipSeriesTitle;
		public NSNumber MainSelectedIndex;

		private const string REAR_LEFT_TYRE_HEAT = "rear_left_tyre_heat";
		private const string FRONT_LEFT_TYRE_HEAT = "front_left_tyre_heat";
		private const string REAR_RIGHT_TYRE_HEAT = "rear_right_tyre_heat";
		private const string FRONT_RIGHT_TYRE_HEAT = "front_right_tyre_heat";
		private const string AVERAGE_TYRE_HEAT = "average_tyre_heat";
	
		public SF1Datasouce (ShinobiChart chart)
		{
			MainChart = chart;
			MainSelectedIndex = 0;
			ToolTipSeriesTitle = "";
			SetTyreData ();
		}

		public void SetTyreData() {
			// Car 1 Data - Intermediate Tyres

			double rndH, flheat, rlheat, frheat, rrheat;
			S1HeatData = new Dictionary<string, NSNumber>[1800];
			S2HeatData = new Dictionary<string, NSNumber>[1800];

			Random r = new Random();

			for (int i=0; i < 1800; i++) {

				Dictionary<string, NSNumber> data = new Dictionary<string, NSNumber>();

				if (i==0) {
					rndH = 70.0;
				} else {
					Dictionary<string, NSNumber> tempData = new Dictionary<string, NSNumber>(S1HeatData[i-1]);

					rndH = tempData [AVERAGE_TYRE_HEAT].DoubleValue;
					rndH = rndH - 2.0 + (r.Next (400)) / 100.0;
				}

				if (rndH>100.0) {
					rndH = 99.0 + (r.Next (400)) / 100.0;
				} else if (rndH < 40.0) {
					rndH = 40.0 + (r.Next (400)) / 100.0;
				}

				flheat = rndH - 2.0 + (r.Next (400)) / 100.0;
				rlheat = rndH - 2.0 + (r.Next (400)) / 100.0;
				frheat = rndH - 2.0 + (r.Next (400)) / 100.0;
				rrheat = rndH - 2.0 + (r.Next (400)) / 100.0;

				// Set heat values
				data[REAR_LEFT_TYRE_HEAT] = new NSNumber (rlheat);
				data[FRONT_LEFT_TYRE_HEAT] = new NSNumber (flheat);
				data[REAR_RIGHT_TYRE_HEAT] = new NSNumber (rrheat);
				data[FRONT_RIGHT_TYRE_HEAT] = new NSNumber (frheat);
				data [AVERAGE_TYRE_HEAT] = new NSNumber ((flheat + rlheat + frheat + rrheat) / 4.0);

				S1HeatData[i] = data;
			}


			// Car 2 Data - Wet Tyres

			for (int i=0; i < 1800; i++) {

				Dictionary<string, NSNumber> data = new Dictionary<string, NSNumber>();

				Dictionary<string, NSNumber> tempData = new Dictionary<string, NSNumber>(S1HeatData[i]);
				rndH = tempData [AVERAGE_TYRE_HEAT].DoubleValue;

				rndH = rndH - 1.0 + (r.Next (400)) / 100.0;

				if (rndH>50.0) {
					rndH = 49.0 + (r.Next (400)) / 100.0;
				} else if (rndH < 30.0) {
					rndH = 30.0 + (r.Next (400)) / 100.0;
				}

				flheat = rndH - 2.0 + (r.Next (400)) / 100.0;
				rlheat = rndH - 2.0 + (r.Next (400)) / 100.0;
				frheat = rndH - 2.0 + (r.Next (400)) / 100.0;
				rrheat = rndH - 2.0 + (r.Next (400)) / 100.0;

				// Set heat values
				data[REAR_LEFT_TYRE_HEAT] = new NSNumber (rlheat);
				data[FRONT_LEFT_TYRE_HEAT] = new NSNumber (flheat);
				data[REAR_RIGHT_TYRE_HEAT] = new NSNumber (rrheat);
				data[FRONT_RIGHT_TYRE_HEAT] = new NSNumber (frheat);
				data [AVERAGE_TYRE_HEAT] = new NSNumber ((flheat + rlheat + frheat + rrheat) / 4.0);

				S2HeatData[i] = data;
			}
		}

		public void setTooltipSeries(SChartSeries series) {
			ToolTipSeriesTitle = new NSString(series.Title);
		}

		public override int GetNumberOfSeries (ShinobiChart chart)
		{
			if (string.Equals (MainChart.Title, chart.Title)) {
				return 2;
			}
			return 4;
		}

		public override SChartSeries GetSeries (ShinobiChart chart, int dataSeriesIndex)
		{
			if (string.Equals (MainChart.Title, chart.Title)) {

				// Main chart - heat/time

				SChartLineSeries lineSeries = new SChartLineSeries();

				if (dataSeriesIndex==0) {
					lineSeries.Title = new NSString("S1 - Intermediate Tyres");
				} else {
					lineSeries.Title = new NSString("S2 - Wet Tyres");
				}

				lineSeries.CrosshairEnabled = true;
				lineSeries.SelectionMode = SChartSelection.Point;

				return lineSeries;

			} else {

				// Tooltip chart

				SChartColumnSeries columnSeries = new SChartColumnSeries();

				switch (dataSeriesIndex) {
				case 0:
					columnSeries.Title = "Front Left ";
					break;

				case 1:
					columnSeries.Title = new NSString("Front Right ");
					break;

				case 2:
					columnSeries.Title = new NSString("Rear Left ");
					break;

				case 3:
					columnSeries.Title = new NSString("Rear Right ");
					break;
				}

				return columnSeries;
			}

		}

		public override int GetNumberOfDataPoints (ShinobiChart chart, int dataSeriesIndex)
		{
			int carIndex = 1;

			if (string.Equals (MainChart.Title, chart.Title)) {
				carIndex = dataSeriesIndex;
			} else {

				return 1;

				// Tooltip chart - find out which series on the main chart the crosshair is on.

				SChartSeries firstSeries = (SChartSeries)MainChart.Series.GetValue(0);
				if (firstSeries.Selected) {
					carIndex = 0;
				}
			}

			if (carIndex==0) {
				return S1HeatData.Length;
			} else {
				return S2HeatData.Length;
			}
		}

		public override SChartData GetDataPoint (ShinobiChart chart, int dataIndex, int dataSeriesIndex)
		{
			Dictionary<string, NSNumber> tyreData; 
			SChartDataPoint dp;
			SChartDataPoint datapoint = new SChartDataPoint();

			if (string.Equals (MainChart.Title, chart.Title)) {

				if (dataSeriesIndex==0) {
					tyreData = new Dictionary<string, NSNumber>(S1HeatData[dataIndex]);
				} else {
					tyreData = new Dictionary<string, NSNumber>(S2HeatData[dataIndex]);
				}

				datapoint.XValue = new NSNumber(dataIndex*10);
				datapoint.YValue = tyreData[AVERAGE_TYRE_HEAT];

				dp = datapoint;

			} else {

				// Tooltip chart - find out which series on the main chart the crosshair is on.
				if (string.Equals("S1 - Intermediate Tyres", ToolTipSeriesTitle)) {
					tyreData = new Dictionary<string, NSNumber>(S1HeatData[MainSelectedIndex.IntValue]);
				} else {
					tyreData = new Dictionary<string, NSNumber>(S2HeatData[MainSelectedIndex.IntValue]);
				}

				switch (dataSeriesIndex) {
				case 0:
					datapoint.XValue = new NSString("Temperature");
					datapoint.YValue = tyreData[FRONT_LEFT_TYRE_HEAT];
					break;

				case 1:
					datapoint.XValue = new NSString("Temperature");
					datapoint.YValue = tyreData[FRONT_RIGHT_TYRE_HEAT];
					break;

				case 2:
					datapoint.XValue = new NSString("Temperature");
					datapoint.YValue = tyreData[REAR_LEFT_TYRE_HEAT];
					break;

				case 3:
					datapoint.XValue = new NSString("Temperature");
					datapoint.YValue = tyreData[REAR_RIGHT_TYRE_HEAT];
					break;

				default:
					break;
				}

				dp = datapoint;
			}

			return dp;
		}
	}
}

