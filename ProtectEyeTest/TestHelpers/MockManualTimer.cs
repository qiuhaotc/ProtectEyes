namespace ProtectEyeTest.TestHelpers;

public class MockManualTimer : ProtectEyes.Services.ITimer
{
    class Entry
    {
        public TimeSpan When { get; set; }
        public Action Callback { get; set; } = () => { };
        public bool Repeating { get; set; }
        public TimeSpan Interval { get; set; }
        public bool Active { get; set; } = true;
    }
    readonly List<Entry> _entries = new();
    TimeSpan _elapsed = TimeSpan.Zero;

    public void Start(TimeSpan interval, Action callback, bool repeating = false)
    {
        _entries.Add(new Entry { Interval = interval, When = _elapsed + interval, Callback = callback, Repeating = repeating });
    }

    public void Stop()
    {
        foreach (var e in _entries) e.Active = false;
    }

    public void Advance(TimeSpan delta)
    {
        var target = _elapsed + delta;
        bool fired;
        do
        {
            fired = false;
            foreach (var e in _entries.ToArray())
            {
                if (!e.Active) continue;
                if (e.When <= target)
                {
                    _elapsed = e.When;
                    e.Callback();
                    if (e.Repeating && e.Active)
                    {
                        e.When = _elapsed + e.Interval;
                    }
                    else
                    {
                        e.Active = false;
                    }
                    fired = true;
                }
            }
        } while (fired);
        _elapsed = target;
    }
}
