using System.IO;

namespace UberStrok.Patcher
{
    internal class UIPatch : Patch
    {
        public override void Apply(UberStrike uberStrike)
        {
            var arr = new string[] { "sharedassets0.assets", "sharedassets1.assets", "sharedassets2.assets" };
            for (int i = 0; i < 3; i++)
            {
                var replace = Path.Combine(uberStrike.ManagedPath.Replace("\\Managed", ""), arr[i]);
                switch (i)
                {
                    case 0:
                        File.WriteAllBytes(replace, Resource.sharedassets0);
                        break;
                    case 1:
                        File.WriteAllBytes(replace, Resource.sharedassets1);
                        break;
                    case 2:
                        File.WriteAllBytes(replace, Resource.sharedassets2);
                        break;
                }
            }
        }
    }
}
