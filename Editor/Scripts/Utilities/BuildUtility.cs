using UnityEngine.Build.Pipeline;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.SceneManagement;

using System.Collections.Generic;
using System.IO;
using RSG;

namespace SpatialSys.UnitySDK.Editor
{
    public class BuildUtility
    {
        public static string BUILD_DIR = "Exports";
        public static string PACKAGE_EXPORT_PATH = Path.Combine(BUILD_DIR, "spaces.unitypackage");

        public static IPromise BuildAndUploadForSandbox()
        {
            // TODO: Ensure WebGL bundle is installed.
            const BuildTarget TARGET = BuildTarget.WebGL;
            string bundleDir = Path.Combine(BUILD_DIR, TARGET.ToString());
            if (Directory.Exists(bundleDir))
                Directory.Delete(bundleDir, recursive: true);
            Directory.CreateDirectory(bundleDir);

            // Compress bundles with LZ4 (ChunkBasedCompression) since web doesn't support LZMA (why isn't this automatic based on build target??)
            CompatibilityAssetBundleManifest bundleManifest = CompatibilityBuildPipeline.BuildAssetBundles(
                bundleDir,
                BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression,
                TARGET
            );

            if (bundleManifest == null)
                return Promise.Rejected(new System.Exception("Failed to build asset bundle for sandbox. Check the console for details."));

            string[] bundleNames = bundleManifest.GetAllAssetBundles();
            if (bundleNames == null || bundleNames.Length == 0)
                return Promise.Rejected(new System.Exception("There are no asset bundles in this project"));

            var bundleNamesSet = new HashSet<string>(bundleManifest.GetAllAssetBundles());

            // Only upload the bundle for the currently opened scene. If there are multiple open scenes, it will choose the first one encountered.
            string bundleNameToUpload = GetAssetBundleNameForOpenedScene();
            if (string.IsNullOrEmpty(bundleNameToUpload))
                return Promise.Rejected(new System.Exception("None of the open scenes are tagged as an asset bundle"));

            // This shouldn't really happen since we build all bundles, but check anyway.
            if (!bundleNamesSet.Contains(bundleNameToUpload))
                return Promise.Rejected(new System.Exception("Asset bundle for the target scene was not built"));

            // TODO: Make cancellable
            UnityEditor.EditorUtility.DisplayProgressBar("Uploading sandbox asset bundle", "Please wait...", 0.5f);

            return SpatialAPI.UploadTestEnvironment()
                .Then(resp => {
                    string bundlePath = Path.Combine(bundleDir, bundleNameToUpload);
                    if (!File.Exists(bundlePath))
                        throw new System.Exception("Built asset bundle was not found on disk");

                    byte[] data = File.ReadAllBytes(bundlePath);
                    SpatialAPI.UploadFile(resp.uploadUrl, data)
                        .Then(resp => EditorUtility.OpenSandboxInBrowser())
                        .Catch(exc => UnityEditor.EditorUtility.DisplayDialog("Asset bundle upload network error", exc.Message, "OK"))
                        .Finally(() => UnityEditor.EditorUtility.ClearProgressBar());
                })
                .Catch(exc => {
                    UnityEditor.EditorUtility.ClearProgressBar();
                    UnityEditor.EditorUtility.DisplayDialog("Upload failed", exc.Message, "OK");
                });
        }

        public static IPromise PackageForPublishing()
        {
            string[] scenePaths = BuildUtility.GetAllAssetBundleScenePaths();

            // Export all scenes and dependencies as a package
            AssetDatabase.ExportPackage(
                scenePaths,
                PACKAGE_EXPORT_PATH,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
            );

            // TODO: Upload package to SAPI

            return Promise.Resolved();
        }

        private static string[] GetAllAssetBundleScenePaths()
        {
            List<string> scenePaths = new List<string>();
            string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < assetBundleNames.Length; i++)
            {
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleNames[i]);
                if (assetPaths.Length > 0 && assetPaths[0].EndsWith(".unity"))
                    scenePaths.Add(assetPaths[0]);
            }

            return scenePaths.ToArray();
        }

        public static string GetAssetBundleNameForOpenedScene()
        {
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                Scene scene = EditorSceneManager.GetSceneAt(i);

                if (scene.IsValid() && scene.isLoaded)
                {
                    AssetImporter importer = AssetImporter.GetAtPath(scene.path);
                    if (importer != null && !string.IsNullOrEmpty(importer.assetBundleName))
                        return importer.assetBundleName;
                }
            }

            return null;
        }
    }
}