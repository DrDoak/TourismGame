
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
class TransparencySortGraphicsHelper
{
    static TransparencySortGraphicsHelper()
    {
        OnLoad();
    }

    [RuntimeInitializeOnLoadMethod]
    static void OnLoad()
    {
        GraphicsSettings.transparencySortMode = TransparencySortMode.Perspective;
    }
}