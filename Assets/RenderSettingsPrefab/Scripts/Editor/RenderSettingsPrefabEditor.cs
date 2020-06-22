using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

/// <summary>
/// RenderSettingsPrefab インスペクタ拡張
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(RenderSettingsPrefab))]
public class RenderSettingsPrefabEditor : Editor
{
    static readonly string[] amibientSource = new string[]
    {
        "Skybox",
        "Gradient",
        "Color",
    };
    static readonly int[] amibientSourceValues = new int[]
    {
        (int)AmbientMode.Skybox,
        (int)AmbientMode.Trilight,
        (int)AmbientMode.Flat,
    };
    static readonly string[] defaultReflectionResolution = new string[]
    {
        "16",
        "32",
        "64",
        "128",
        "256",
        "512",
        "1024",
        "2048",
    };
    static readonly int[] defaultReflectionResolutionValues = new int[]
    {
        16,
        32,
        64,
        128,
        256,
        512,
        1024,
        2048,
    };

    bool holdoutEnvironment;
    bool holdoutMixedLighting;
    bool holdoutOtherSettings;

    /// <summary>
    /// インスペクタ描画
    /// </summary>
    /// <param name="go"></param>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUI.BeginChangeCheck();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(MonoScript), false);
        EditorGUI.EndDisabledGroup();

        DrawLightingEditor();
        DrawBakeButton();

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    /// <summary>
    /// LightingEditor 描画。
    /// </summary>
    void DrawLightingEditor()
    {
        var instance = (RenderSettingsPrefab)target;
        if (!instance.baked)
        {
            return;
        }

        EditorGUI.BeginDisabledGroup(true);
        if (holdoutEnvironment = EditorGUILayout.Foldout(holdoutEnvironment, "Environment"))
        {
            EditorGUI.indentLevel++;
            instance.skybox = EditorGUILayout.ObjectField("Skybox Material", instance.skybox, typeof(Material), true) as Material;
            instance.sun = EditorGUILayout.ObjectField("Sun Source", instance.sun, typeof(Light), true)as Light;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Environment Lighting");
            EditorGUI.indentLevel++;
            switch (instance.ambientMode = (AmbientMode)EditorGUILayout.IntPopup("Source", (int)instance.ambientMode, amibientSource, amibientSourceValues))
            {
                case AmbientMode.Trilight:
                    instance.ambientSkyColor = EditorGUILayout.ColorField("Sky Color", instance.ambientSkyColor);
                    instance.ambientEquatorColor = EditorGUILayout.ColorField("Equator Color", instance.ambientEquatorColor);
                    instance.ambientGroundColor = EditorGUILayout.ColorField("Ground Color", instance.ambientGroundColor);
                    break;
                case AmbientMode.Flat:
                    instance.ambientSkyColor = EditorGUILayout.ColorField("Ambient Color", instance.ambientSkyColor);
                    break;
                case AmbientMode.Skybox:
                    if (instance.skybox == null)
                    {
                        instance.ambientSkyColor = EditorGUILayout.ColorField("Ambient Color", instance.ambientSkyColor);
                    }
                    else
                    {
                        instance.ambientIntensity = EditorGUILayout.Slider("Intensity Multiplier", instance.ambientIntensity, 0.0F, 8.0F);
                    }
                    break;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Environment Reflections");
            EditorGUI.indentLevel++;
            switch (instance.defaultReflectionMode = (DefaultReflectionMode)EditorGUILayout.EnumPopup("Source", instance.defaultReflectionMode))
            {
                case DefaultReflectionMode.Skybox:
                    instance.defaultReflectionResolution = EditorGUILayout.IntPopup("Resolution", instance.defaultReflectionResolution, defaultReflectionResolution, defaultReflectionResolutionValues);
                    break;
                case DefaultReflectionMode.Custom:
                    instance.customReflection = EditorGUILayout.ObjectField("Cubemap", instance.customReflection, typeof(Cubemap), true) as Cubemap;
                    break;
            }
            instance.reflectionIntensity = EditorGUILayout.Slider("Intensity Multiplier", instance.reflectionIntensity, 0.0F, 1.0F);
            instance.reflectionBounces = EditorGUILayout.IntSlider("Bounces", instance.reflectionBounces, 1, 5);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
        if (holdoutMixedLighting = EditorGUILayout.Foldout(holdoutMixedLighting, "Mixed Lighting"))
        {
            EditorGUI.indentLevel++;
            instance.subtractiveShadowColor = EditorGUILayout.ColorField("Realtime Shadow Color", instance.subtractiveShadowColor);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
        if (holdoutOtherSettings = EditorGUILayout.Foldout(holdoutOtherSettings, "Other Settings"))
        {
            EditorGUI.indentLevel++;
            if (instance.fog = EditorGUILayout.Toggle("Fog", instance.fog))
            {
                EditorGUI.indentLevel++;
                instance.fogColor = EditorGUILayout.ColorField("Color", instance.fogColor);
                switch (instance.fogMode = (FogMode)EditorGUILayout.EnumPopup("Mode", instance.fogMode))
                {
                    case FogMode.Linear:
                        instance.fogStartDistance = EditorGUILayout.FloatField("Start", instance.fogStartDistance);
                        instance.fogEndDistance = EditorGUILayout.FloatField("End", instance.fogEndDistance);
                        break;
                    default:
                        instance.fogDensity = EditorGUILayout.FloatField("Density", instance.fogDensity);
                        break;
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            instance.haloStrength = EditorGUILayout.Slider("Halo Strength", instance.haloStrength, 0.0f, 1.0f);
            instance.flareFadeSpeed = EditorGUILayout.FloatField("Flare Fade Speed", instance.flareFadeSpeed);
            instance.flareStrength = EditorGUILayout.Slider("Flare Strength", instance.flareStrength, 0.0f, 1.0f);

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
        EditorGUI.EndDisabledGroup();
    }

    /// <summary>
    /// ベイクボタン描画。
    /// </summary>
    void DrawBakeButton()
    {
        var instance = (RenderSettingsPrefab)target;
        
        GUILayout.BeginHorizontal();
        GUILayout.Space(EditorGUIUtility.labelWidth);
        EditorGUI.BeginDisabledGroup(!instance.baked);
        if (GUILayout.Button("Clear"))
        {
            DoClear();
        }
        EditorGUI.EndDisabledGroup();
        if (GUILayout.Button("Bake"))
        {
            DoBake();
        }

        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// クリアする
    /// </summary>
    void DoClear()
    {
        var instance = (RenderSettingsPrefab)target;
        var go = instance.gameObject;

        instance.baked = default(bool);
        instance.unityVersion = default(string);
        instance.haloStrength = default(float);
        instance.defaultReflectionResolution = default(int);
        instance.defaultReflectionMode = default(DefaultReflectionMode);
        instance.reflectionBounces = default(int);
        instance.reflectionIntensity = default(float);
        instance.sun = default(Light);
        instance.skybox = default(Material);
        instance.ambientIntensity = default(float);
        instance.subtractiveShadowColor = default(Color);
        instance.ambientIntensity = default(float);
        instance.ambientGroundColor = default(Color);
        instance.ambientEquatorColor = default(Color);
        instance.ambientSkyColor = default(Color);
        instance.ambientMode = default(AmbientMode);
        instance.fogDensity = default(float);
        instance.fogColor = default(Color);
        instance.fogMode = default(FogMode);
        instance.fogStartDistance = default(float);
        instance.fog = default(bool);
        instance.customReflection = default(Cubemap);
        instance.flareStrength = default(float);
        instance.flareFadeSpeed = default(float);
 
        DoApply(go);
    }

    /// <summary>
    /// ベイクする。
    /// </summary>
    void DoBake()
    {
        var instance = (RenderSettingsPrefab)target;
        var go = instance.gameObject;

        instance.baked = true;
        instance.unityVersion = Application.unityVersion;
        instance.haloStrength = RenderSettings.haloStrength;
        instance.defaultReflectionResolution = RenderSettings.defaultReflectionResolution;
        instance.defaultReflectionMode = RenderSettings.defaultReflectionMode;
        instance.reflectionBounces = RenderSettings.reflectionBounces;
        instance.reflectionIntensity = RenderSettings.reflectionIntensity;
        instance.sun = RenderSettings.sun;
        instance.skybox = RenderSettings.skybox;
        instance.ambientIntensity = RenderSettings.ambientIntensity;
        instance.subtractiveShadowColor = RenderSettings.subtractiveShadowColor;
        instance.ambientIntensity = RenderSettings.ambientIntensity;
        instance.ambientGroundColor = RenderSettings.ambientGroundColor;
        instance.ambientEquatorColor = RenderSettings.ambientEquatorColor;
        instance.ambientSkyColor = RenderSettings.ambientSkyColor;
        instance.ambientMode = RenderSettings.ambientMode;
        instance.fogDensity = RenderSettings.fogDensity;
        instance.fogColor = RenderSettings.fogColor;
        instance.fogMode = RenderSettings.fogMode;
        instance.fogStartDistance = RenderSettings.fogStartDistance;
        instance.fog = RenderSettings.fog;
        instance.customReflection = RenderSettings.customReflection;
        instance.flareStrength = RenderSettings.flareStrength;
        instance.flareFadeSpeed = RenderSettings.flareFadeSpeed;

        DoApply(go);
    }

    /// <summary>
    /// アプライする。
    /// </summary>
    /// <param name="go"></param>
    void DoApply(GameObject go)
    {
        var prefab = PrefabUtility.GetPrefabObject(go);
        if (prefab == null)
        {
            return;
        }
        var path = AssetDatabase.GetAssetPath(prefab);
        if (!string.IsNullOrEmpty(path))
        {
            return;
        }

        var root = PrefabUtility.FindPrefabRoot(go);
        var parent = PrefabUtility.GetPrefabParent(root);

        PrefabUtility.ReplacePrefab(root, parent, ReplacePrefabOptions.ConnectToPrefab);
        AssetDatabase.SaveAssets();
    }
}