using System;
using System.Text;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class PlayerWeaponStatisticsView
    {
        public PlayerWeaponStatisticsView()
        {
            // Space
        }

        public PlayerWeaponStatisticsView(int meleeTotalSplats, int machineGunTotalSplats, int shotgunTotalSplats, int sniperTotalSplats, int splattergunTotalSplats, int cannonTotalSplats, int launcherTotalSplats, int meleeTotalShotsFired, int meleeTotalShotsHit, int meleeTotalDamageDone, int machineGunTotalShotsFired, int machineGunTotalShotsHit, int machineGunTotalDamageDone, int shotgunTotalShotsFired, int shotgunTotalShotsHit, int shotgunTotalDamageDone, int sniperTotalShotsFired, int sniperTotalShotsHit, int sniperTotalDamageDone, int splattergunTotalShotsFired, int splattergunTotalShotsHit, int splattergunTotalDamageDone, int cannonTotalShotsFired, int cannonTotalShotsHit, int cannonTotalDamageDone, int launcherTotalShotsFired, int launcherTotalShotsHit, int launcherTotalDamageDone)
        {
            CannonTotalDamageDone = cannonTotalDamageDone;
            CannonTotalShotsFired = cannonTotalShotsFired;
            CannonTotalShotsHit = cannonTotalShotsHit;
            CannonTotalSplats = cannonTotalSplats;
            LauncherTotalDamageDone = launcherTotalDamageDone;
            LauncherTotalShotsFired = launcherTotalShotsFired;
            LauncherTotalShotsHit = launcherTotalShotsHit;
            LauncherTotalSplats = launcherTotalSplats;
            MachineGunTotalDamageDone = machineGunTotalDamageDone;
            MachineGunTotalShotsFired = machineGunTotalShotsFired;
            MachineGunTotalShotsHit = machineGunTotalShotsHit;
            MachineGunTotalSplats = machineGunTotalSplats;
            MeleeTotalDamageDone = meleeTotalDamageDone;
            MeleeTotalShotsFired = meleeTotalShotsFired;
            MeleeTotalShotsHit = meleeTotalShotsHit;
            MeleeTotalSplats = meleeTotalSplats;
            ShotgunTotalDamageDone = shotgunTotalDamageDone;
            ShotgunTotalShotsFired = shotgunTotalShotsFired;
            ShotgunTotalShotsHit = shotgunTotalShotsHit;
            ShotgunTotalSplats = shotgunTotalSplats;
            SniperTotalDamageDone = sniperTotalDamageDone;
            SniperTotalShotsFired = sniperTotalShotsFired;
            SniperTotalShotsHit = sniperTotalShotsHit;
            SniperTotalSplats = sniperTotalSplats;
            SplattergunTotalDamageDone = splattergunTotalDamageDone;
            SplattergunTotalShotsFired = splattergunTotalShotsFired;
            SplattergunTotalShotsHit = splattergunTotalShotsHit;
            SplattergunTotalSplats = splattergunTotalSplats;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            _ = builder.Append("[PlayerWeaponStatisticsView: ");
            _ = builder.Append("[CannonTotalDamageDone: ");
            _ = builder.Append(CannonTotalDamageDone);
            _ = builder.Append("][CannonTotalShotsFired: ");
            _ = builder.Append(CannonTotalShotsFired);
            _ = builder.Append("][CannonTotalShotsHit: ");
            _ = builder.Append(CannonTotalShotsHit);
            _ = builder.Append("][CannonTotalSplats: ");
            _ = builder.Append(CannonTotalSplats);
            _ = builder.Append("][LauncherTotalDamageDone: ");
            _ = builder.Append(LauncherTotalDamageDone);
            _ = builder.Append("][LauncherTotalShotsFired: ");
            _ = builder.Append(LauncherTotalShotsFired);
            _ = builder.Append("][LauncherTotalShotsHit: ");
            _ = builder.Append(LauncherTotalShotsHit);
            _ = builder.Append("][LauncherTotalSplats: ");
            _ = builder.Append(LauncherTotalSplats);
            _ = builder.Append("][MachineGunTotalDamageDone: ");
            _ = builder.Append(MachineGunTotalDamageDone);
            _ = builder.Append("][MachineGunTotalShotsFired: ");
            _ = builder.Append(MachineGunTotalShotsFired);
            _ = builder.Append("][MachineGunTotalShotsHit: ");
            _ = builder.Append(MachineGunTotalShotsHit);
            _ = builder.Append("][MachineGunTotalSplats: ");
            _ = builder.Append(MachineGunTotalSplats);
            _ = builder.Append("][MeleeTotalDamageDone: ");
            _ = builder.Append(MeleeTotalDamageDone);
            _ = builder.Append("][MeleeTotalShotsFired: ");
            _ = builder.Append(MeleeTotalShotsFired);
            _ = builder.Append("][MeleeTotalShotsHit: ");
            _ = builder.Append(MeleeTotalShotsHit);
            _ = builder.Append("][MeleeTotalSplats: ");
            _ = builder.Append(MeleeTotalSplats);
            _ = builder.Append("][ShotgunTotalDamageDone: ");
            _ = builder.Append(ShotgunTotalDamageDone);
            _ = builder.Append("][ShotgunTotalShotsFired: ");
            _ = builder.Append(ShotgunTotalShotsFired);
            _ = builder.Append("][ShotgunTotalShotsHit: ");
            _ = builder.Append(ShotgunTotalShotsHit);
            _ = builder.Append("][ShotgunTotalSplats: ");
            _ = builder.Append(ShotgunTotalSplats);
            _ = builder.Append("][SniperTotalDamageDone: ");
            _ = builder.Append(SniperTotalDamageDone);
            _ = builder.Append("][SniperTotalShotsFired: ");
            _ = builder.Append(SniperTotalShotsFired);
            _ = builder.Append("][SniperTotalShotsHit: ");
            _ = builder.Append(SniperTotalShotsHit);
            _ = builder.Append("][SniperTotalSplats: ");
            _ = builder.Append(SniperTotalSplats);
            _ = builder.Append("][SplattergunTotalDamageDone: ");
            _ = builder.Append(SplattergunTotalDamageDone);
            _ = builder.Append("][SplattergunTotalShotsFired: ");
            _ = builder.Append(SplattergunTotalShotsFired);
            _ = builder.Append("][SplattergunTotalShotsHit: ");
            _ = builder.Append(SplattergunTotalShotsHit);
            _ = builder.Append("][SplattergunTotalSplats: ");
            _ = builder.Append(SplattergunTotalSplats);
            _ = builder.Append("]]");
            return builder.ToString();
        }

        public int CannonTotalDamageDone { get; set; }
        public int CannonTotalShotsFired { get; set; }
        public int CannonTotalShotsHit { get; set; }
        public int CannonTotalSplats { get; set; }
        public int LauncherTotalDamageDone { get; set; }
        public int LauncherTotalShotsFired { get; set; }
        public int LauncherTotalShotsHit { get; set; }
        public int LauncherTotalSplats { get; set; }
        public int MachineGunTotalDamageDone { get; set; }
        public int MachineGunTotalShotsFired { get; set; }
        public int MachineGunTotalShotsHit { get; set; }
        public int MachineGunTotalSplats { get; set; }
        public int MeleeTotalDamageDone { get; set; }
        public int MeleeTotalShotsFired { get; set; }
        public int MeleeTotalShotsHit { get; set; }
        public int MeleeTotalSplats { get; set; }
        public int ShotgunTotalDamageDone { get; set; }
        public int ShotgunTotalShotsFired { get; set; }
        public int ShotgunTotalShotsHit { get; set; }
        public int ShotgunTotalSplats { get; set; }
        public int SniperTotalDamageDone { get; set; }
        public int SniperTotalShotsFired { get; set; }
        public int SniperTotalShotsHit { get; set; }
        public int SniperTotalSplats { get; set; }
        public int SplattergunTotalDamageDone { get; set; }
        public int SplattergunTotalShotsFired { get; set; }
        public int SplattergunTotalShotsHit { get; set; }
        public int SplattergunTotalSplats { get; set; }
    }
}
