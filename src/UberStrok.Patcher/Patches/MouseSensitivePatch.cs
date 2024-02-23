using dnlib.DotNet.Emit;

namespace UberStrok.Patcher
{
    internal class MouseSensitivePatch : Patch
    {
        public override void Apply(UberStrike uberStrike)
        {
            var ApplicationOptions_Type = uberStrike.AssemblyCSharp.Find("ApplicationOptions", true);
            var ApplicationOptions_CCtor = ApplicationOptions_Type.FindMethod("Initialize");
            var ilBody = ApplicationOptions_CCtor.Body;
            ilBody.Instructions.Insert(122, OpCodes.Ldc_R4.ToInstruction(0.1f));
            ilBody.Instructions.RemoveAt(123);
            ilBody.Instructions.Insert(130, OpCodes.Ldc_R4.ToInstruction(0.1f));
            ilBody.Instructions.RemoveAt(131);
            var OptionsPanelGUI_Type = uberStrike.AssemblyCSharp.Find("OptionsPanelGUI", true);
            var OptionsPanel_DoControlsGroup = OptionsPanelGUI_Type.FindMethod("DoControlsGroup");
            ilBody = OptionsPanel_DoControlsGroup.Body;
            ilBody.Instructions.Insert(84, OpCodes.Ldc_R4.ToInstruction(0.1f));
            ilBody.Instructions.RemoveAt(85);
        }
    }
}
