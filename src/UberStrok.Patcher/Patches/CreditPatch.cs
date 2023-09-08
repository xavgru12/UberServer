namespace UberStrok.Patcher
{
    internal class CreditPatch : Patch
    {
        public override void Apply(UberStrike uberStrike)
        {
            var ApplicationDataManager_Type = uberStrike.AssemblyCSharp.Find("ApplicationDataManager", true);
            var ApplicationDataManager_CCtor = ApplicationDataManager_Type.FindMethod("OpenBuyCredits");
            var ilBody = ApplicationDataManager_CCtor.Body;
            if (ilBody.Instructions.Count == 15)
            {
                for (int x = 0; x <= 14; x++)
                {
                    ilBody.Instructions.RemoveAt(0);
                }
            }

            var ShopPageGUI_Type = uberStrike.AssemblyCSharp.Find("ShopPageGUI", true);
            var ShopPageGUI_Start = ShopPageGUI_Type.FindMethod("Start");
            ilBody = ShopPageGUI_Start.Body;
            if (ilBody.Instructions.Count == 298)
            {
                for (int x = 45; x < 53; x++)
                {
                    ilBody.Instructions.RemoveAt(45);
                }
            }
        }
    }
}
