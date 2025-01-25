using System;
using System.Collections.Generic;
using System.Timers;

namespace BlackHole.Model
{
	public class BlackHoleTimerAggregation : ITimer
	{
		private readonly System.Timers.Timer _timer;

		public bool Enabled
		{
			get => _timer.Enabled;
			set => _timer.Enabled = true;
		}

		public double Interval
		{
			get => _timer.Interval;
			set => _timer.Interval = value;
		}

		public event EventHandler? Elapsed;

		public BlackHoleTimerAggregation()
		{
			_timer = new System.Timers.Timer();
			_timer.Elapsed += (sender, e) =>
			{
				Elapsed?.Invoke(sender, e);
			};
		}

		public void Start()
		{
			_timer.Start();
		}

		public void Stop()
		{
			_timer.Stop();
		}
	}
}

