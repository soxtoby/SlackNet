using System.Collections.Generic;
using Newtonsoft.Json;
using SlackNet.Blocks;

namespace SlackNet.Events;

public class AssistantAppThread : MessageEvent
{
    public bool IsLocked { get; set; }
    
    [JsonProperty("assistant_app_thread")]
    public AssistantAppThreadInfo AssistantThread { get; set; }
}

public class AssistantAppThreadInfo
{
  public string Title { get; set; }
  public IList<Block> TitleBlocks { get; set; } = [];
  public AssistantThreadContext Context { get; set; }

  /// <summary>
  /// Undocumented. Please raise an issue at https://github.com/soxtoby/SlackNet/issues if you find anything here.
  /// </summary>
  public IList<object> Artifacts { get; set; } = [];
}