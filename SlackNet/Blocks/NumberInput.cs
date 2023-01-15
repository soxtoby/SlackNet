using System.Globalization;
using Newtonsoft.Json;

namespace SlackNet.Blocks;

public class NumberInput : TextInput
{
    public NumberInput() : base("number_input") { }

    /// <summary>
    /// Decimal numbers are allowed if this is set to true.
    /// </summary>
    public bool IsDecimalAllowed { get; set; }

    /// <summary>
    /// The minimum value, cannot be greater than <see cref="MaxValue"/>.
    /// </summary>
    public string MinValue { get; set; }

    /// <summary>
    /// The maximum value, cannot be less than <see cref="MinValue"/>.
    /// </summary>
    public string MaxValue { get; set; }

    /// <summary>
    /// The initial value in the input when it is loaded, as a double. 
    /// </summary>
    [JsonIgnore]
    public double? InitialDouble
    {
        get => double.TryParse(InitialValue, out var value) ? value : null;
        set => InitialValue = value?.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// The initial value in the input when it is loaded, as an integer. 
    /// </summary>
    [JsonIgnore]
    public int? InitialInteger
    {
        get => int.TryParse(InitialValue, out var value) ? value : null;
        set => InitialValue = value?.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// The minimum value as a double, cannot be greater than <see cref="MaxValue"/>.
    /// </summary>
    [JsonIgnore]
    public double? MinDouble
    {
        get => double.TryParse(MinValue, out var value) ? value : null;
        set => MinValue = value?.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// The minimum value as an integer, cannot be greater than <see cref="MaxValue"/>.
    /// </summary>
    [JsonIgnore]
    public int? MinInteger
    {
        get => int.TryParse(MinValue, out var value) ? value : null;
        set => MinValue = value?.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// The maximum value as a double, cannot be less than <see cref="MinValue"/>.
    /// </summary>
    [JsonIgnore]
    public double? MaxDouble
    {
        get => double.TryParse(MaxValue, out var value) ? value : null;
        set => MaxValue = value?.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// The maximum value as an integer, cannot be less than <see cref="MinValue"/>.
    /// </summary>
    [JsonIgnore]
    public int? MaxInteger
    {
        get => int.TryParse(MaxValue, out var value) ? value : null;
        set => MaxValue = value?.ToString(CultureInfo.InvariantCulture);
    }
}

[SlackType("number_input")]
public class NumberInputAction : BlockAction
{
    public string Value { get; set; }
    public double? DoubleValue => double.TryParse(Value, out var value) ? value : null;
    public int? IntegerValue => int.TryParse(Value, out var value) ? value : null;
}

[SlackType("number_input")]
public class NumberInputValue : ElementValue
{
    public string Value { get; set; }
    public double? DoubleValue => double.TryParse(Value, out var value) ? value : null;
    public int? IntegerValue => int.TryParse(Value, out var value) ? value : null;
}