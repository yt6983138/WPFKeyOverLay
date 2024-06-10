namespace WPFKeyOverLay;
public class KpsHandler
{
	private int index;
	private int[] kps;

	private Timer timer;

	public float Kps
	{
		get
		{
			float kps = this.kps.Sum(x => x) / (float)this.UpdatesPerSecond;

			if (kps > this.PeakKps)
				this.PeakKps = kps;

			return kps;
		}
	}
	public float PeakKps { get; set; }
	public int UpdatesPerSecond { get; private set; }

	public KpsHandler(int updatesPerSecond = 10)
	{
		this.UpdatesPerSecond = updatesPerSecond;
		this.kps = new int[this.UpdatesPerSecond];
		this.timer = new(this.Tick, null, 0, 1000 / this.UpdatesPerSecond);
	}

	private void Tick(object? sender)
	{
		this.kps[this.index] = 0;
		if (++this.index >= this.UpdatesPerSecond)
		{
			this.index = 0;
		}
	}

	public void Update(int keyCount)
	{
		for (int i = 0; i < this.UpdatesPerSecond; i++)
		{
			this.kps[i] += keyCount;
		}
	}

	public void Clear()
	{
		Array.Clear(this.kps);
	}
}

