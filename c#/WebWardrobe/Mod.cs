using GDWeave;

namespace WebWardrobe;

public class Mod : IMod {

    public Mod(IModInterface modInterface) {
        modInterface.RegisterScriptMod(new HudMod());
        modInterface.RegisterScriptMod(new CosmeticMenuMod());
    }

    public void Dispose() {
        // Cleanup anything you do here
    }
}
