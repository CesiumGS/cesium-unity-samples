using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class CesiumMagicLeapDefine
{
    private const string CESIUM_ML_DEFINE = "CESIUM_MAGIC_LEAP";

    [InitializeOnLoadMethod]
    private static void CheckAndEnableMagicLeapDefine()
    {
        // We don't get a script define for the ML SDK package being included, so we check if its assembly exists
        // If it does, we set our own define
        System.Reflection.Assembly mlAssembly;
        try
        {
            mlAssembly = System.Reflection.Assembly.Load("MagicLeap.SDK");
        }
        catch (System.IO.FileNotFoundException e)
        {
            mlAssembly = null;
        }

        if (mlAssembly == null)
        {
#if CESIUM_MAGIC_LEAP
            // Remove the define if it already exists, to clean up after ourselves
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out string[] defines);
            defines = defines.Where(d => d != CESIUM_ML_DEFINE).ToArray();
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
#endif
            return;
        }

#if !CESIUM_MAGIC_LEAP
        PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out string[] defines);
        HashSet<string> definesSet = new HashSet<string>(defines) { CESIUM_ML_DEFINE };
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, definesSet.ToArray());
#endif
    }
}
