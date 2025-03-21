using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;
public class AltVoraBot : Bot
{
    private bool trackingTarget = false;
    private double lastTargetX, lastTargetY;
    private int movementDirection = 1;
    static void Main()
    {
        new AltVoraBot().Start();
    }
    public AltVoraBot() : base(BotInfo.FromFile("AltVoraBot.json")) { }
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
            if (!trackingTarget)
            {
                TurnRadarRight(360);
            }
            else
            {
                TrackAndAttack();
            }
        }
    }
    private void TrackAndAttack()
    {
        ToTarget(lastTargetX, lastTargetY);
        Stop(true);
        Fire(3);
        ZigZagMovement();
        Resume();
    }
    public override void OnScannedBot(ScannedBotEvent e)
    {
        trackingTarget = true;
        lastTargetX = e.X;
        lastTargetY = e.Y;

        ToTarget(lastTargetX, lastTargetY);
        Stop(true);

        double firePower = Math.Min(3, 500 / DistanceTo(lastTargetX, lastTargetY));
        Fire(firePower);

        if (DistanceTo(lastTargetX, lastTargetY) > 100)
        {
            SetTurnRight(BearingTo(lastTargetX, lastTargetY));
            SetForward(80);
        }
        else
        {
            ZigZagMovement();
        }
        Resume();
    }
    public override void OnHitBot(HitBotEvent e)
    {
        ToTarget(e.X, e.Y);
        Stop(true);
        Fire(3);
        Back(50);
        Resume();
    }   
    public override void OnHitWall(HitWallEvent e)
    {
        Back(60);
        TurnRight(90);
        Forward(100);
    } 
    public override void OnHitByBullet(HitByBulletEvent e)
    {
        ToTarget(e.Bullet.X, e.Bullet.Y);
        Fire(2);
        trackingTarget = false;
        ZigZagMovement();
    }
    public override void OnBulletHitWall(BulletHitWallEvent e)
    {
        TurnRadarLeft(90);
        Rescan();
    }
    public override void OnBulletHit(BulletHitBotEvent e)
    {
        ToTarget(e.Bullet.X, e.Bullet.Y);
        Fire(3);
    }
    public override void OnBulletFired(BulletFiredEvent e)
    {
        TurnLeft(20);
        Forward(50);
    }
    public override void OnBotDeath(BotDeathEvent e)
    {
        trackingTarget = false;
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
    private void ZigZagMovement()
    {
        for (int i = 0; i < 3; i++)
        {
            SetTurnRight(40 * movementDirection);
            SetForward(60);
            SetTurnLeft(40 * movementDirection);
            SetForward(60);
        }
        movementDirection *= -1;
    }
}