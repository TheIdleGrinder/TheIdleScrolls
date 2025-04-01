namespace TheIdleScrolls_Web.CoreWrapper
{
    public class ExpiringMessage
    {
        System.Timers.Timer _timer;
        string _message;
        bool _expired = false;

        public bool Expired => _expired;
        public string Message => _message;

        public ExpiringMessage(string message, double lifetime)
        {
            _message = message;
            _timer = new((int)(1000 * lifetime));
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = false;
            _timer.Enabled = true;
            _timer.Start();
        }

        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _expired = true;
        }

        public void SetExpired()
        {
            _expired = true;
        }
    }
}
