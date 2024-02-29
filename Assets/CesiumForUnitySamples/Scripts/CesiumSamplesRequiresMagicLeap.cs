using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

#if CESIUM_MAGIC_LEAP
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport;
#endif
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;

[ExecuteInEditMode]
public class CesiumSamplesRequiresMagicLeap : MonoBehaviour
{
#if UNITY_EDITOR
    private const string ML_PACKAGE_REQUIRED_TEXT =
         "The Magic Leap SDK needs to be installed in the project for this sample to work. " +
         "Click below to read instructions from the Magic Leap documentation on how to add the SDK to the project.";
    private const string ML_PACKAGE_DOCS_LINK = "https://developer-docs.magicleap.cloud/docs/guides/unity/getting-started/configure-unity-settings/#import-magic-leap-unity-sdk";

    private const string ML_BUILD_SETTINGS_TEXT =
        "Build settings need to be changed to support the Magic Leap. Click Ok to perform these changes automatically.";
    private const string ML_MIN_VERSION_TEXT =
        "The Magic Leap sample requires Unity version 2022.2 or greater. Please reopen the project in a compatible version of Unity to continue.";

    private const string ML_FEATURE_SET_ID = "com.magicleap.openxr.featuregroup";

    private static bool _waitingForReturnToEditMode = false;
    private static CesiumSamplesRequiresMagicLeap _instance = null;

#if UNITY_2022_2_OR_NEWER
    static CesiumSamplesRequiresMagicLeap()
    {
        EditorApplication.playModeStateChanged += OnStateChanged;
    }
#endif // UNITY_2022_2_OR_NEWER

    private void Start()
    {
        // There should only ever be one of these at most in a scene.
        if(_instance != null && _instance != this)
        {
            DestroyImmediate(this);
            return;
        }

        _instance = this;

#if !UNITY_2022_2_OR_NEWER
        EditorUtility.DisplayDialog("Unity 2022.2 or Greater Required", ML_MIN_VERSION_TEXT, "Ok");
        return;
#endif // !UNITY_2022_2_OR_NEWER

#if UNITY_2022_2_OR_NEWER
        if (_waitingForReturnToEditMode)
        {
            return;
        }

        bool hasMagicLeap = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages().Any(p => p.name == "com.magicleap.unitysdk");
        if (!hasMagicLeap)
        {
            if (EditorUtility.DisplayDialog("Setup required", ML_PACKAGE_REQUIRED_TEXT, "Open documentation", "I'll add it manually"))
            {
                Application.OpenURL(ML_PACKAGE_DOCS_LINK);
            }

            // We don't have the SDK yet, so let's handle the rest when we do
            return;
        }

        if (!CheckIfSettingsCorrect())
        {
            if (EditorUtility.DisplayDialog("Build settings need changing", ML_BUILD_SETTINGS_TEXT, "Ok", "Cancel"))
            {
                ChangeBuildSettings();
            }
        }
#endif // UNITY_2022_2_OR_NEWER
    }

#if UNITY_2022_2_OR_NEWER
    private static void OnStateChanged(PlayModeStateChange obj)
    {
        // If we're waiting for the editor to return to edit mode and we're there, actually change the build settings
        if (_waitingForReturnToEditMode && obj == PlayModeStateChange.EnteredEditMode && _instance != null)
        {
            _instance.ChangeBuildSettings();
        }
    }

    private void ChangeBuildSettings()
    {
        // We can't make these changes from within play mode, so let's kick the user out of play mode
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            _waitingForReturnToEditMode = true;
            EditorApplication.ExitPlaymode();
            return;
        }

        // MagicLeap is an Android target
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        // ML runs on X86_64, not the ARM default for Android
        PlayerSettings.Android.targetArchitectures |= AndroidArchitecture.X86_64;

        // Make sure the OpenXRLoader is enabled in the project settings
        XRGeneralSettings xrSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
        if (!xrSettings.Manager.activeLoaders.Any(l => l is OpenXRLoader))
        {
            XRPackageMetadataStore.AssignLoader(xrSettings.Manager, "Unity.XR.OpenXR.OpenXRLoader", BuildTargetGroup.Android);
        }

        // Make sure the Magic Leap feature group is enabled too
        OpenXRFeatureSetManager.FeatureSet featureSet = OpenXRFeatureSetManager.GetFeatureSetWithId(BuildTargetGroup.Android, ML_FEATURE_SET_ID);
        featureSet.isEnabled = true;
        OpenXRFeatureSetManager.SetFeaturesFromEnabledFeatureSets(BuildTargetGroup.Android);

        OpenXRSettings oxrSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);

        // Meta Quest support will cause a build error if it's enabled and an unsupported interaction feature
        // (like the MagicLeapControllerProfile) is configured. So let's disable it so we can build.
        MetaQuestFeature metaQuestFeature = oxrSettings.GetFeature<MetaQuestFeature>();
        if (metaQuestFeature != null && metaQuestFeature.enabled)
        {
            metaQuestFeature.enabled = false;
        }

#if CESIUM_MAGIC_LEAP
        // Make sure we can use the global dimmer feature
        MagicLeapRenderingExtensionsFeature renderFeature = oxrSettings.GetFeature<MagicLeapRenderingExtensionsFeature>();
        renderFeature.enabled = true;
        renderFeature.GlobalDimmerEnabled = true;

        // Make sure the Magic Leap Controller is enabled as an input source
        OpenXRFeature[] interactionFeatures = oxrSettings.GetFeatures<OpenXRInteractionFeature>();
        MagicLeapControllerProfile magicLeapProfile = interactionFeatures.FirstOrDefault(f => f.GetType() == typeof(MagicLeapControllerProfile)) as MagicLeapControllerProfile;
        if (magicLeapProfile != null)
        {
            magicLeapProfile.enabled = true;
        }

        MagicLeapFeature mlFeature = oxrSettings.GetFeature<MagicLeapFeature>();
        mlFeature.enabled = true;

        // We need to disable ML's handling of the far clip plane, so we can see distant terrain
        // This property is private so we use SerializedProperty to access it
        SerializedObject mlFeatureObject = new SerializedObject(mlFeature);
        SerializedProperty farClipProperty = mlFeatureObject.FindProperty("farClipPolicy");
        farClipProperty.intValue = (int)MagicLeapFeature.FarClipMode.None;
        mlFeatureObject.ApplyModifiedPropertiesWithoutUndo();

#else  // CESIUM_MAGIC_LEAP
        return;
#endif // CESIUM_MAGIC_LEAP

        // If we forced the user out of playmode to make these changes, put them back in
        if (_waitingForReturnToEditMode)
        {
            _waitingForReturnToEditMode = false;
            EditorApplication.EnterPlaymode();
        }
    }

    private bool CheckIfSettingsCorrect()
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            return false;
        }

        var targetArchitectures = PlayerSettings.Android.targetArchitectures;
        if (!PlayerSettings.Android.targetArchitectures.HasFlag(AndroidArchitecture.X86_64))
        {
            return false;
        }

        XRGeneralSettingsPerBuildTarget buildTargetSettings = null;
        EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out buildTargetSettings);
        XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Android);
        OpenXRLoader loader = settings.Manager.activeLoaders.FirstOrDefault(l => l.GetType() == typeof(OpenXRLoader)) as OpenXRLoader;
        if (loader == null)
        {
            return false;
        }

        OpenXRFeatureSetManager.FeatureSet featureSet = OpenXRFeatureSetManager.GetFeatureSetWithId(BuildTargetGroup.Android, ML_FEATURE_SET_ID);
        if (featureSet == null || !featureSet.isInstalled || !featureSet.isEnabled)
        {
            return false;
        }

        OpenXRSettings oxrSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);

        MetaQuestFeature metaQuestFeature = oxrSettings.GetFeature<MetaQuestFeature>();
        if (metaQuestFeature != null && metaQuestFeature.enabled)
        {
            return false;
        }

#if CESIUM_MAGIC_LEAP
        MagicLeapRenderingExtensionsFeature renderFeature = oxrSettings.GetFeature<MagicLeapRenderingExtensionsFeature>();
        if (renderFeature == null || !renderFeature.enabled || !renderFeature.GlobalDimmerEnabled)
        {
            return false;
        }

        OpenXRFeature[] interactionFeatures = oxrSettings.GetFeatures<OpenXRInteractionFeature>();
        if (!interactionFeatures.Any(f => f.GetType() == typeof(MagicLeapControllerProfile) && f.enabled))
        {
            return false;
        }

        MagicLeapFeature mlFeature = oxrSettings.GetFeature<MagicLeapFeature>();
        if (mlFeature == null || !mlFeature.enabled || mlFeature.FarClipPolicy != MagicLeapFeature.FarClipMode.None)
        {
            return false;
        }
#else  // CESIUM_MAGIC_LEAP
        return false;
#endif // CESIUM_MAGIC_LEAP

        return true;
    }
#endif // UNITY_2022_2_OR_NEWER
#endif // UNITY_EDITOR
}
