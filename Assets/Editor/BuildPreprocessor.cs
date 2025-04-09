using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildPreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        ExportCrunchedTextures.Export();
    }
}