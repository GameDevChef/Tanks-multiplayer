using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SOutility : Editor {

    [MenuItem("Assets/AudioSO")]
    public static void Create()
    {
        AudioSO audioSO = CreateInstance<AudioSO>();
        AssetDatabase.CreateAsset(audioSO, "Assets/Resources/AudioSO.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
