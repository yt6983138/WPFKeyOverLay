using System.Diagnostics;

namespace WPFKeyOverLay;
public class TickedExecuter
{
	private Stopwatch Stopwatch { get; set; } = new();

	public long HasExecutedCount { get; private set; }
	public long TickInterval { get; set; }
	public event EventHandler? OnTicked;
	public float MillisecondsInterval
	{
		get => this.TickInterval / 10000f;
		set => this.TickInterval = (long)(value * 10000);
	}
	public float Tps
	{
		get => 1000 / this.MillisecondsInterval;
		set => this.MillisecondsInterval = 1000 / value;
	}

	public TickedExecuter()
	{

	}

	public TickedExecuter Start()
	{
		if (this.TickInterval < 1)
			throw new InvalidOperationException("You must set Interval before use.");

		this.Stopwatch.Start();
		Task.Run(this.TicksChecker);

		return this;
	}
	public TickedExecuter Stop()
	{
		this.Stopwatch.Stop();
		this.HasExecutedCount = 0;

		return this;
	}

	private void TicksChecker()
	{
		SpinWait spinWait = new();
		while (this.Stopwatch.IsRunning)
		{
			spinWait.SpinOnce();
			long execCount = this.Stopwatch.ElapsedTicks / this.TickInterval;
			if (execCount > this.HasExecutedCount)
			{
				this.HasExecutedCount = execCount;
				OnTicked?.Invoke(this, EventArgs.Empty);
			}

		}
	}

}
