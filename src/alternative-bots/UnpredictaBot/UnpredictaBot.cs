using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class UnpredictaBot : Bot {   
	int turnDirection = 1; // Arah putar radar
	int moveDirection = 1;
	int oscillationCounter = 0; // Menghitung osilasi
	bool justFired = false;

	const double OPTIMAL_DISTANCE = 200;
	const double CLOSE_RANGE = 200; // Jarak dekat
	const double MEDIUM_RANGE = 400; // Jarak menengah
	const double WALL_MARGIN = 50;

    static void Main(string[] args) {
        new UnpredictaBot().Start();
    }

    UnpredictaBot() : base(BotInfo.FromFile("UnpredictaBot.json")) { }

    public override void Run() {
        // Warna body, gun, dan radar
        BodyColor = Color.Gray;
		TurretColor = Color.Yellow;
		RadarColor = Color.Green;
		
		AdjustGunForBodyTurn = true;    // Gun bergerak secara independen terhadap body
		AdjustRadarForGunTurn = true;   // Radar bergerak secara independen terhadap Gun
		AdjustRadarForBodyTurn = true;  // Radar bergerak secara independen terhadap body

        while (IsRunning) {
			avoidWall();
			
			TurnRadarLeft(90*turnDirection);
			oscillationCounter++;
			if (oscillationCounter == 4) {
				oscillationCounter = 0;
				turnDirection *= -1;
			}
        }
    }

	// Ketika dekat dengan dinding -> arahkan ke tengah arena -> maju
    private void avoidWall() {
        if (X < WALL_MARGIN || X > ArenaWidth - WALL_MARGIN ||
            Y < WALL_MARGIN || Y > ArenaHeight - WALL_MARGIN) {
            double angleToCenter = DirectionTo(ArenaWidth / 2, ArenaHeight / 2);
            double turnAngle = NormalizeRelativeAngle(angleToCenter - Direction);

            TurnRight(turnAngle);
            Forward(100);
            moveDirection *= -1;
        } 
        else if (!justFired) {
            TurnLeft(45);
            Forward(80 * moveDirection);
        }
        
        justFired = false;
    }

    // bot lawan terdeteksi -> arahkan ke bot tersebut -> ukur kecepatannya -> tembak
	public override void OnScannedBot(ScannedBotEvent e) {
        var gunBear = GunBearingTo(e.X, e.Y);
        TurnGunLeft(gunBear);
        
        double firePower;
        if (Math.Abs(e.Speed) < 2) firePower = 4.0;
        else if (Math.Abs(e.Speed) < 5) firePower = 2.0;
        else firePower = 1.0;
        
        if (Energy < 20) firePower = 1.0;

        Fire(firePower);
        
        TurnRadarRight(30 * turnDirection); //lebih baik dihapus kah??
        turnDirection *= -1; //lebih baik dihapus kah??
        
        justFired = true;

        if (gunBear == 0) Rescan();
    }

    // bot menabrak bot lawan -> Arahkan peluru ke bot lawan -> tembak -> pindah posisi
    public override void OnHitBot(HitBotEvent e) {
        var gunBear = GunBearingTo(e.X, e.Y);
        TurnGunLeft(gunBear);
        
        Stop(true);
        if (Energy < 2.5) Fire(0.2);
        else Fire (2.5);
        // Fire(Math.Min(2.5, Energy-0.1));
        
        // Tegak lurus
        double perpendicular = NormalizeRelativeAngle(90 - DirectionTo(e.X, e.Y));
        TurnLeft(perpendicular);
        MaxSpeed = 5;
        Forward(100);
        
        Resume();
        justFired = true;
    }

    // bot menabrak dinding -> putar bot -> maju 100 langkah
    public override void OnHitWall(HitWallEvent e) {
        double turnAngle = 90 + (Energy % 70); // random berdasarkan energy
        TurnLeft(turnAngle);
        Forward(100);
        moveDirection *= -1;
    }

   	// terkena peluru dari bot lain -> putar ke arah peluru tersebut, tembak -> pindah posisi
    public override void OnHitByBullet(HitByBulletEvent e) {
        var gunBear = GunBearingTo(e.Bullet.X, e.Bullet.Y);
        TurnGunLeft(gunBear);
        Fire(1.5);
        
        // menghindar tegak lurus terhadap arah peluru
        double dodgeAngle = NormalizeRelativeAngle(90 - (Direction - e.Bullet.Direction));
        TurnRight(dodgeAngle);
        MaxSpeed = 5;
        Forward(100);
        
        justFired = true;
    }
    
    // peluru menabrak dinding -> Putar radar 60 derajat
    public override void OnBulletHitWall(BulletHitWallEvent e) {
        TurnRadarRight(60);
    }
    
    // peluru berhasil mengenai bot lain -> Arahkan kembali ke arah peluru, tembak lagi
    public override void OnBulletHit(BulletHitBotEvent e) {
        var gunBear = GunBearingTo(e.Bullet.X, e.Bullet.Y);
        TurnGunLeft(gunBear);
        Fire(Math.Min(e.Bullet.Power + 1, 3.0));
        
        justFired = true;
    }

	// ada yang mati -> Rescan
    public override void OnBotDeath(BotDeathEvent e) {
        Rescan();
    }
    
    // setelah menembak -> langsung pindah tempat
    public override void OnBulletFired(BulletFiredEvent e) {
        TurnLeft(35 + (int)(Energy % 30)); // random berdasarkan energy
        Forward(70);
        
        justFired = true;
    }
    
    // private void ToTarget(double x, double y) {
    //     var direction = DirectionTo(x, y);
    //     var gunBear = NormalizeRelativeAngle(direction - GunDirection);
    //     TurnGunLeft(gunBear);
    // }
}