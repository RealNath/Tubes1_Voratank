using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;
public class VoraBot : Bot
{
    int turnDirect = 1; 
    static void Main()
    {
        new VoraBot().Start();
    }
    public VoraBot() : base(BotInfo.FromFile("VoraBot.json")) { }
    public override void Run()
    {
        BodyColor = Color.Red;
        TurretColor = Color.Yellow;
        RadarColor = Color.Green;
        BulletColor = Color.White;
        ScanColor = Color.Cyan;

        AdjustGunForBodyTurn = true;
        AdjustRadarForGunTurn = true;
        AdjustRadarForBodyTurn = true;

        while (IsRunning)
        {
            TurnRadarRight(270 * turnDirect);
            TurnLeft(45);
            Forward(80);
        }
    }
    public override void OnScannedBot(ScannedBotEvent e)
    {
        ToTarget(e.X, e.Y);
        Stop(true);

        double firePower = Math.Min(3, 500 / DistanceTo(e.X, e.Y));
        Fire(firePower);
        TurnRadarLeft(30 * turnDirect);
        turnDirect *= -1;
        Resume();
    }
    public override void OnHitBot(HitBotEvent e)
    {
        ToTarget(e.X, e.Y);
        Stop(true);
        Fire(2.5);
        TurnLeft(NormalizeBearing(90 - DirectionTo(e.X, e.Y)));
        Forward(90);
        Resume();
    }
    public override void OnHitWall(HitWallEvent e)
    {
        Back(50); 
        TurnRight(60); 
        Forward(80); 
        TurnLeft(60); 
    }
    public override void OnHitByBullet(HitByBulletEvent e)
    {
        ToTarget(e.Bullet.X, e.Bullet.Y);
        Fire(1);
        TurnRight(NormalizeBearing(90 - (Direction - e.Bullet.Direction)));
        Forward(60);
    }
    public override void OnBulletHitWall(BulletHitWallEvent e)
    {
        TurnRadarLeft(90);
        Rescan();
    }
    public override void OnBulletHit(BulletHitBotEvent e)
    {
        ToTarget(e.Bullet.X, e.Bullet.Y);

        double distance = DistanceTo(e.Bullet.X, e.Bullet.Y);
        double firePower = Math.Min(3, Math.Max(0.1, 500 / distance));

        Fire(firePower);
    }
    public override void OnBulletFired(BulletFiredEvent e)
    {
        TurnLeft(30);
        Forward(70);
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
        public override void OnBotDeath(BotDeathEvent e)
    {
        Rescan();
    }
}
