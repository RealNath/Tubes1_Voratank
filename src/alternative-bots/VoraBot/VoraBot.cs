using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class VoraBot : Bot
{
    private double lastTargetX, lastTargetY; //Posisi terakhir musuh yang terdeteksi
    private int movementDirection = 1; //Arah pergerakan bot (1 = maju, -1 = mundur)
    private int radarDirection = 1; //Arah perputaran radar (1 = searah jarum jam, -1 = berlawanan)
    static void Main()
    {
        new VoraBot().Start();
    }

    public VoraBot() : base(BotInfo.FromFile("VoraBot.json")) { }

    public override void Run()
    {
        //Warna elemen-elemen pada bot
        BodyColor = Color.Red;
        TurretColor = Color.Yellow;
        RadarColor = Color.Green;
        BulletColor = Color.White;
        ScanColor = Color.Cyan;

        // Mengatur agar radar dan senjata bergerak secara independen dari body bot
        AdjustGunForBodyTurn = true;
        AdjustRadarForGunTurn = true;
        AdjustRadarForBodyTurn = true;

        while (IsRunning)
        {
            // Radar terus berputar untuk mencari musuh
            TurnRadarRight(360 * radarDirection);

            // Bot bergerak dalam pola memutar untuk menghindari musuh
            TurnLeft(45);
            Forward(60);
        }
    }
    public override void OnScannedBot(ScannedBotEvent e)
    {
        //Simpan posisi terakhir musuh
        lastTargetX = e.X;
        lastTargetY = e.Y;

        //Arahkan senjata ke target
        ToTarget(lastTargetX, lastTargetY);
        
        //Hentikan pergerakan sementara untuk menembak lebih akurat
        Stop(true);

        //Hitung kekuatan tembakan berdasarkan jarak musuh
        double firePower = Math.Min(3, 500 / DistanceTo(lastTargetX, lastTargetY));
        Fire(firePower);

        //Ubah arah radar agar tetap mengikuti musuh
        radarDirection *= -1;

        //Lanjutkan pergerakan bot
        Resume();
    }
    public override void OnHitBot(HitBotEvent e)
    {
        //Arahkan senjata ke bot lawan
        ToTarget(e.X, e.Y);
        
        //Berhenti sejenak untuk menembak
        Stop(true);
        Fire(3);
        
        //Mundur sedikit untuk menghindari tabrakan terus-menerus
        Back(50);
        
        //Lanjutkan pergerakan bot
        Resume();
    }
    public override void OnHitWall(HitWallEvent e)
    {
        //Mundur untuk menjauh dari dinding
        Back(60);
        
        //Putar 90 derajat agar bisa keluar dari posisi terjepit
        TurnRight(90);
        Forward(100);
    }
    public override void OnHitByBullet(HitByBulletEvent e)
    {
        //Arahkan senjata ke posisi musuh yang menembak
        ToTarget(e.Bullet.X, e.Bullet.Y);
        
        //Hitung firepower berdasarkan jarak ke bot musuh
        double distance = DistanceTo(e.Bullet.X, e.Bullet.Y);
        double firePower = Math.Min(3, Math.Max(0.1, 500 / distance));
        
        //Tembak balik dengan kekuatan yang telah dihitung
        Fire(firePower);
        
        //Gunakan gerakan zig-zag untuk menghindari serangan lanjutan
        ZigZagMovement();
    }
    public override void OnBulletHitWall(BulletHitWallEvent e)
    {
        TurnRadarLeft(90);
        Rescan();
    }
    public override void OnBulletHit(BulletHitBotEvent e)
    {
        //Arahkan senjata ke posisi musuh yang terkena tembakan
        ToTarget(e.Bullet.X, e.Bullet.Y);
        
        //Hitung firepower berdasarkan jarak ke musuh
        double distance = DistanceTo(e.Bullet.X, e.Bullet.Y);
        double firePower = Math.Min(3, Math.Max(0.1, 500 / distance));
        
        //Tembak musuh dengan kekuatan yang telah dihitung
        Fire(firePower);
    }
    public override void OnBulletFired(BulletFiredEvent e)
    {
        // Lakukan gerakan menghindar setelah menembak
        TurnLeft(20);
        Forward(50);
    }
    public override void OnBotDeath(BotDeathEvent e)
    {
        Rescan();
    }
    private void ToTarget(double x, double y)
    {
        var direct = DirectionTo(x, y);
        var gunBear = NormalizeBearing(direct - GunDirection);
        TurnGunLeft(gunBear);
    }
    private double NormalizeBearing(double angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }
    // Metode untuk melakukan gerakan zig-zag agar sulit ditembak
    private void ZigZagMovement()
    {
        for (int i = 0; i < 3; i++)
        {
            TurnRight(40*movementDirection);
            Forward(40);
            TurnLeft(40*movementDirection);
            Forward(40);
        }
        // Ubah arah gerakan setelah zig-zag selesai
        movementDirection *= -1;
    }
}
