using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace SlackNet.WebApi;

/// <summary>
/// Helper class to make building <see cref="FormUrlEncodedContent" />s easier.
/// </summary>
class SlackFormContent : IEnumerable
{
    private readonly List<KeyValuePair<string, string>> _fields = new();

    public void Add(string key, string value) => _fields.Add(new KeyValuePair<string, string>(key, value));

    public IEnumerator GetEnumerator() => _fields.GetEnumerator();

    public static implicit operator FormUrlEncodedContent(SlackFormContent content) => new(content._fields.Where(f => f.Value != null));
}