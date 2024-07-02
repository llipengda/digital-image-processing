using System;
using System.Threading;

namespace DigitalImageProcessing.UI;

public class DebounceHelper
{
    private Timer? _timer;
    private Action? _action;
    private TimeSpan _timeout;

    private void DebouncedAction()
    {
        _timer?.Dispose();
        _timer = new Timer(
            _ => { _action?.Invoke(); },
            null,
            _timeout,
            TimeSpan.FromMilliseconds(-1)
        );
    }

    public Action Debounce(Action action, TimeSpan timeout)
    {
        _action = action;
        _timeout = timeout;
        return DebouncedAction;
    }
}