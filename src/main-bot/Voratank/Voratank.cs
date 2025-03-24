using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Voratank : Bot
{   
	int turnDirect = 1; // Arah putar radar

    /* A bot that drives forward and backward, and fires a bullet */
    static void Main(string[] args)
    {
        new Voratank().Start();
    }

    Voratank() : base(BotInfo.FromFile("Voratank.json")) { }

    public override void Run()
    {
        // Warna body, gun, dan radar
        BodyColor = Color.Gray;
		TurretColor = Color.Blue;
		RadarColor = Color.Yellow;
		BulletColor = Color.Gray;
		ScanColor = Color.Brown;
		TracksColor = Color.DarkGray;
		GunColor = Color.Cyan;
		
		AdjustGunForBodyTurn = true;    // Gun bergerak secara independen terhadap body
		AdjustRadarForGunTurn = true;   // Radar bergerak secara independen terhadap Gun
		AdjustRadarForBodyTurn = true;  // Radar bergerak secara independen terhadap body

        while (IsRunning)
        {			
			if (TurnNumber == 1)
			{
				Forward(100);
			}

			TurnRadarRight(270*turnDirect);

			TurnLeft(45);
			Forward(80);
        }
    }

	// Ketika bot lawan terdeteksi -> arahkan ke bot tersebut -> ukur kecepatannya -> Tembak
    public override void OnScannedBot(ScannedBotEvent e)
    {					
        ToTarget(e.X, e.Y);
		Stop(true);
		if (Math.Abs(e.Speed) < 5)
		{
			Fire(2);
			TurnRadarLeft(30*turnDirect);
		}
		else
		{
			Fire(1);
			TurnRadarLeft(30*turnDirect);
		}
		turnDirect *= -1;
		Resume();
    }

	// Ketika menabrak bot lawan -> Arahkan peluru ke bot lawan -> tembak -> pindah posisi
    public override void OnHitBot(HitBotEvent e) 
    {
		ToTarget(e.X, e.Y);

		Stop(true);
		Fire(2.5);

		TurnLeft(NormalizeRelativeAngle(90 - DirectionTo(e.X, e.Y)));
		MaxSpeed = 5;
		Forward(90);
		
		Resume();
    }

	// Ketika body bot menabrak dinding -> putar bot 80 derajat -> maju 60 langkah
    public override void OnHitWall(HitWallEvent e)
    {
        TurnLeft(80);
		Forward(60);
    }

	// Ketika terkena peluru dari bot lain -> putar ke arah peluru tersebut, tembak -> pindah posisi
	public override void OnHitByBullet(HitByBulletEvent e)
	{
		ToTarget(e.Bullet.X, e.Bullet.Y);
		Fire(1);
		TurnRight(NormalizeRelativeAngle(90 - (Direction - e.Bullet.Direction)));
		Forward(60);
	}
	
	// Ketika peluru menabrak dinding -> Putar radar 45 derajat
	public override void OnBulletHitWall(BulletHitWallEvent e)
	{
		TurnRadarLeft(45);
	}
	
	// ketika peluru berhasil mengenai bot lain -> Arahkan kembali ke arah peluru, tembak lagi
	public override void OnBulletHit(BulletHitBotEvent e)
	{
		ToTarget(e.Bullet.X, e.Bullet.Y);
		Fire(2);
	}

	// Ada yang mati -> Rescan
	public override void OnBotDeath(BotDeathEvent e)
	{
		Rescan();
	}
	
	// setelah menembak -> langsung pindah tempat
	public override void OnBulletFired(BulletFiredEvent e)
	{
		TurnLeft(30);
		Forward(70);
	}
	
	// Fungsi untuk mengarahkan Gun ke arah bot lain
	private void ToTarget(double x, double y)
	{
		var direct = DirectionTo(x, y);
		var gunBear = NormalizeRelativeAngle(direct - GunDirection);

		TurnGunLeft(gunBear); 
	}
}