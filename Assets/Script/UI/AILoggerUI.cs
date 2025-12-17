using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AILoggerUI : MonoBehaviour
{
    [Header("Log Output")]
    public TextMeshProUGUI logText;   // or TMP_Text if you prefer TextMeshPro

    [Header("Channel Toggles")]
    public Transform channelToggleContainer; // parent object with VerticalLayoutGroup
    public Toggle channelTogglePrefab;       // a simple Toggle prefab with a Text child

    private readonly Dictionary<string, Toggle> _channelToggles = new Dictionary<string, Toggle>();
    private AILogger _logger;

    private void Start()
    {
        _logger = AILogger.Instance;

        if (_logger == null)
        {
            Debug.LogWarning("AILoggerUI: No AILogger instance found in the scene.");
            enabled = false;
            return;
        }

        // Subscribe to logger events
        _logger.OnLogAdded += OnLogAdded;
        _logger.OnChannelsChanged += OnChannelsChanged;

        // Initial population
        BuildChannelToggles();
        RebuildLogText();
    }

    private void OnDestroy()
    {
        if (_logger != null)
        {
            _logger.OnLogAdded -= OnLogAdded;
            _logger.OnChannelsChanged -= OnChannelsChanged;
        }
    }

    private void OnChannelsChanged()
    {
        BuildChannelToggles();
        RebuildLogText();
    }

    private void OnLogAdded(AILogger.LogEntry entry)
    {
        // Append only if the channel is enabled
        if (_logger.IsChannelEnabled(entry.channel))
        {
            AppendLogLine(entry);
        }
    }

    private void BuildChannelToggles()
    {
        // Clear old toggles
        foreach (Transform child in channelToggleContainer)
        {
            Destroy(child.gameObject);
        }
        _channelToggles.Clear();

        // Create a toggle for each channel
        foreach (string channel in _logger.GetChannels())
        {
            var toggle = Instantiate(channelTogglePrefab, channelToggleContainer);
            toggle.isOn = _logger.IsChannelEnabled(channel);

            // Set label
            var label = toggle.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = channel;
            }

            string capturedChannel = channel; // capture for closure
            toggle.onValueChanged.AddListener(isOn =>
            {
                _logger.SetChannelEnabled(capturedChannel, isOn);
                RebuildLogText();
            });

            _channelToggles[channel] = toggle;
        }
    }

    private void RebuildLogText()
    {
        if (logText == null) return;

        StringBuilder sb = new StringBuilder();
        foreach (var entry in _logger.entries)
        {
            if (_logger.IsChannelEnabled(entry.channel))
            {
                sb.AppendLine(FormatEntry(entry));
            }
        }

        logText.text = sb.ToString();
    }

    private void AppendLogLine(AILogger.LogEntry entry)
    {
        if (logText == null) return;

        logText.text += FormatEntry(entry) + "\n";
    }

    private string FormatEntry(AILogger.LogEntry entry)
    {
        // Example: [12.3s][Pathfinding] Recalculated path with 18 nodes
        return $"[{entry.time:0.0}s][{entry.channel}] {entry.message}";
    }
}
