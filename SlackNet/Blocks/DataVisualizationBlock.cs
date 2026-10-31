using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// Displays data in a line, bar, area, or pie chart.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/blocks/data-visualization-block/">Slack documentation</a> for more information.</remarks>
[SlackType("data_visualization")]
public class DataVisualizationBlock() : Block("data_visualization")
{
    /// <summary>
    /// A short label displayed above the chart.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The chart-specific payload.
    /// </summary>
    public DataVisualizationChart Chart { get; set; }
}

/// <summary>
/// A chart displayed by a <see cref="DataVisualizationBlock"/>.
/// </summary>
public abstract class DataVisualizationChart(string type) : SlackObject
{
    /// <summary>
    /// The type of chart.
    /// </summary>
    public string Type { get; } = type;
}

/// <summary>
/// A pie chart.
/// </summary>
[SlackType("pie")]
public class PieChart() : DataVisualizationChart("pie")
{
    /// <summary>
    /// Labeled slices that make up the pie.
    /// </summary>
    public IList<ChartSegment> Segments { get; set; } = [];
}

/// <summary>
/// A chart whose values are plotted against axes.
/// </summary>
public abstract class AxisChart(string type) : DataVisualizationChart(type)
{
    /// <summary>
    /// The data series to plot.
    /// </summary>
    public IList<DataSeries> Series { get; set; } = [];

    /// <summary>
    /// X-axis categories and axis titles.
    /// </summary>
    public AxisConfig AxisConfig { get; set; }
}

/// <summary>
/// A bar chart.
/// </summary>
[SlackType("bar")]
public class BarChart() : AxisChart("bar");

/// <summary>
/// An area chart.
/// </summary>
[SlackType("area")]
public class AreaChart() : AxisChart("area");

/// <summary>
/// A line chart.
/// </summary>
[SlackType("line")]
public class LineChart() : AxisChart("line");

/// <summary>
/// A labeled slice in a <see cref="PieChart"/>.
/// </summary>
public class ChartSegment : SlackObject
{
    /// <summary>
    /// Display name for this slice, shown in the legend and on hover.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Numeric weight of this slice.
    /// </summary>
    public double Value { get; set; }
}

/// <summary>
/// A named set of ordered data points.
/// </summary>
public class DataSeries : SlackObject
{
    /// <summary>
    /// Human-readable identifier displayed in the chart legend.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Ordered data points, with one entry for each <see cref="AxisConfig.Categories"/> value.
    /// </summary>
    public IList<DataPoint> Data { get; set; } = [];
}

/// <summary>
/// A value belonging to an x-axis category.
/// </summary>
public class DataPoint : SlackObject
{
    /// <summary>
    /// The x-axis category this point belongs to.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Numeric y-axis value.
    /// </summary>
    public double Value { get; set; }
}

/// <summary>
/// X-axis categories and optional axis titles.
/// </summary>
public class AxisConfig : SlackObject
{
    /// <summary>
    /// Category labels for the x-axis, in display order.
    /// </summary>
    public IList<string> Categories { get; set; } = [];

    /// <summary>
    /// Descriptive title displayed below the x-axis.
    /// </summary>
    public string XLabel { get; set; }

    /// <summary>
    /// Descriptive title displayed beside the y-axis.
    /// </summary>
    public string YLabel { get; set; }
}
