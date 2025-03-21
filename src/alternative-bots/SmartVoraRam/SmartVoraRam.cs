using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class SmartVoraRam : Bot
{   
	int turnDirect = 1;   // Arah putar bot
	bool hitted = false;  // Kondisi bot menabrak bot lain
	int botId = 0;        // ID bot yang ditabrak

    /* A bot that drives forward and backward, and fires a bullet */
    static void Main(string[] args)
    {
        new SmartVoraRam().Start();
    }

    SmartVoraRam() : base(BotInfo.FromFile("SmartVoraRam.json")) { }

    public override void Run()
    {
        /* Customize bot colors, read the documentation for more information */
        BodyColor = Color.Blue;
		TurretColor = Color.Red;
		RadarColor = Color.Yellow;

        while (IsRunning)
        {
			TurnRight(90 * turnDirect);  // Berputar 90 derajat
        }
    }

	// Deteksi dengan radar
    public override void OnScannedBot(ScannedBotEvent e)
    {
		// Memutar posisi bot ke arah bot lawan
		var dir = DirectionTo(e.X, e.Y);
		dir = NormalizeRelativeAngle(dir - Direction);
		TurnRate = 10;
		TurnLeft(dir);
		
		// Jika kecepatan bot di bawah 4.5 dan energy-nya di bawah 40, tabrak bot itu
		if ((e.Speed < 4.5) & (e.Energy < 40))
		{
			ToTarget(e.X, e.Y);  // Arahkan posisi ke bot lawan
			ToBot(e.X, e.Y, 50); // Datang ke bot lawan dengan tambahan gerakan 
								 // maju sebesar 50 (agar menabrak bot lawan)
		}
		else
		{
			ToTarget(e.X, e.Y);  // Arahkan posisi ke bot lawan
			// Jika energy bot lebih dari 10, tembak dengan firepower 1.5
			if (Energy > 10)
			{
				Fire(1.5);
			}
			else
			{
				Fire(1);
			}
			// Pindah posisi setelah menembak
			TurnLeft(45 * turnDirect);
			TargetSpeed = 8;
			Forward(50);
		}
    }
	
	// Kondisi ketika menabrak bot
    public override void OnHitBot(HitBotEvent e)
    {
		hitted = true;
		botId = e.VictimId;
		
		// Spam tembak
		ToTarget(e.X, e.Y);
		while (hitted)
		{
			// Jika energy bot di atas 15, tembak dengan firepower 4
			if (Energy > 15)
			{
				Fire(4);
			}
			else
			{
				Fire(1.5);
			}
			ToBot(e.X, e.Y, 100);  // Maju terus ke arah bot (agar tertabrak)
			
			// Jika sudah tidak menabrak bot, berhenti menembak
			if (!e.IsRammed)
			{
				hitted = false;
				break;
			}
		}
    }

	// Kondisi ketika bot menabrak dinding
    public override void OnHitWall(HitWallEvent e)
    {
		Rescan();
    }
	
	// Ketika peluru mengenai bot lawan, arahkan posisi bot ke posisi peluru, ganti arah rotasi
	public override void OnBulletHit(BulletHitBotEvent e)
	{
		ToTarget(e.Bullet.X, e.Bullet.Y);
		turnDirect *= -1;
	}
	
	// Ketika terkena peluru dari lawan, arahkan radar ke posisi meriam (untuk memeriksa apakah terdapat bot lawan)
	public override void OnHitByBullet(HitByBulletEvent e)
	{
		ToTarget(e.Bullet.X, e.Bullet.Y);
	}
	
	// Ketika satu ronde berakhir, ubah kondisi bot menabrak bot lain, 
	// agar tidak terjebak di while loop, apabila bot sedang ram bot lain
	public override void OnRoundEnded(RoundEndedEvent e)
	{
		hitted = false;
		Rescan();
	}
	
	// Ketika ada bot yang mati, periksa apakah itu bot yang sedang ditabrak.
	// Jika iya, ubah kondisi menabrak bot lain menjadi false, agar tidak terjebak di while loop
	public override void OnBotDeath(BotDeathEvent e)
	{
		if (botId == e.VictimId)
		{
			hitted = false;
		}
	}
	
	// Fungsi untuk mengubah posisi arah bot ke bot lawan.
	private void ToTarget(double x, double y)
	{
		var bearing = DirectionTo(x, y);
		bearing = NormalizeRelativeAngle(bearing - Direction);
		TurnRate = 10;
		TurnLeft(bearing);
	}
	
	// Fungsi untuk mendekat ke bot lawan dengan tambahan jarak tertentu
	private void ToBot(double x, double y, double far)
	{
		var dist = DistanceTo(x, y);
		TargetSpeed = 8;
		Forward(dist + far);
	}
}