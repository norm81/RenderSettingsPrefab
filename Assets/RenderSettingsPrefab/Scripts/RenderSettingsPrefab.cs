using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// RenderSettingsのプレハブ保存データ。
/// </summary>
public class RenderSettingsPrefab : MonoBehaviour
{
#region RenderSettings
    /// <summary>
    /// Size of the Light halos.
    /// </summary>
    public float haloStrength;

    /// <summary>
    /// Cubemap resolution for default reflection.
    /// </summary>
    public int defaultReflectionResolution;

    /// <summary>
    /// Default reflection mode.
    /// </summary>
    public DefaultReflectionMode defaultReflectionMode;

    /// <summary>
    /// The number of times a reflection includes other reflections.
    /// </summary>
    public int reflectionBounces;

    /// <summary>
    /// How much the skybox / custom cubemap reflection affects the scene.
    /// </summary>
    public float reflectionIntensity;

    /// <summary>
    /// The light used by the procedural skybox.
    /// </summary>
    public Light sun;

    /// <summary>
    /// The global skybox to use.
    /// </summary>
    public Material skybox;

    /// <summary>
    /// The color used for the sun shadows in the Subtractive lightmode.
    /// </summary>
    public Color subtractiveShadowColor;

    /// <summary>
    /// Flat ambient lighting color.
    /// </summary>
    public float ambientIntensity;

    /// <summary>
    /// Ambient lighting coming from below.
    /// </summary>
    public Color ambientGroundColor;

    /// <summary>
    /// Ambient lighting coming from the sides.
    /// </summary>
    public Color ambientEquatorColor;

    /// <summary>
    /// Ambient lighting coming from above.
    /// </summary>
    public Color ambientSkyColor;

    /// <summary>
    /// Ambient lighting mode.
    /// </summary>
    public AmbientMode ambientMode;

    /// <summary>
    /// The density of the exponential fog.
    /// </summary>
    public float fogDensity;

    /// <summary>
    /// The color of the fog.
    /// </summary>
    public Color fogColor;

    /// <summary>
    /// Fog mode to use.
    /// </summary>
    public FogMode fogMode;

    /// <summary>
    /// The ending distance of linear fog.
    /// </summary>
    public float fogEndDistance;

    /// <summary>
    /// The starting distance of linear fog.
    /// </summary>
    public float fogStartDistance;

    /// <summary>
    /// Is fog enabled?
    /// </summary>
    public bool fog;

    /// <summary>
    /// Custom specular reflection cubemap.
    /// </summary>
    public Cubemap customReflection;

    /// <summary>
    /// The intensity of all flares in the scene.
    /// </summary>
    public float flareStrength;

    /// <summary>
    /// The fade speed of all flares in the scene.
    /// </summary>
    public float flareFadeSpeed;
#endregion

    [HideInInspector]
    public bool baked;
    [HideInInspector]
    public string unityVersion;
    bool initialized;

#region MonoBehaviour
    void OnEnable()
    {
        if (!baked)
        {
            return;
        }
        SetupRender();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!baked || initialized)
        {
            return;
        }
        SetupRender();
    }
#endif
#endregion

    public void SetupRender()
    {
        RenderSettings.haloStrength = haloStrength;
        RenderSettings.defaultReflectionResolution = defaultReflectionResolution;
        RenderSettings.defaultReflectionMode = defaultReflectionMode;
        RenderSettings.reflectionBounces = reflectionBounces;
        RenderSettings.reflectionIntensity = reflectionIntensity;
        RenderSettings.sun = sun;
        RenderSettings.skybox = skybox;
        RenderSettings.ambientIntensity = ambientIntensity;
        RenderSettings.subtractiveShadowColor = subtractiveShadowColor;
        RenderSettings.ambientIntensity = ambientIntensity;
        RenderSettings.ambientGroundColor = ambientGroundColor;
        RenderSettings.ambientEquatorColor = ambientEquatorColor;
        RenderSettings.ambientSkyColor = ambientSkyColor;
        RenderSettings.ambientMode = ambientMode;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogStartDistance = fogStartDistance;
        RenderSettings.fog = fog;
        RenderSettings.customReflection = customReflection;
        RenderSettings.flareStrength = flareStrength;
        RenderSettings.flareFadeSpeed = flareFadeSpeed;

        initialized = true;
    }
}