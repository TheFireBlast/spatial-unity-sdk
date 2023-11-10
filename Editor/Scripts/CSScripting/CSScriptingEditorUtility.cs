using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Build.Player;
using System.Security.Cryptography;
using System.Text;

namespace SpatialSys.UnitySDK.Editor
{
    public static class CSScriptingEditorUtility
    {
        public static readonly string OUTPUT_ASSET_PATH = Path.Combine(EditorUtility.AUTOGENERATED_ASSETS_DIRECTORY, CSScriptingUtility.DEFAULT_CSHARP_ASSEMBLY_NAME + ".dll.txt");

        private const string COMPILE_DESTINATION_DIR = "Temp/CSScriptingCompiledDlls";

        public static void EnforceCustomAssemblyName(AssemblyDefinitionAsset assemblyDefinition, string sku)
        {
            string asmDefAssetPath = AssetDatabase.GetAssetPath(assemblyDefinition);
            string asmDefOriginal = File.ReadAllText(asmDefAssetPath);
            string assemblyName = GetAssemblyNameForSKU(sku);
            string asmDefModified = Regex.Replace(asmDefOriginal, "\"name\":\\s*\".*?\",", $"\"name\": \"{assemblyName}\",");
            File.WriteAllText(asmDefAssetPath, asmDefModified);
            AssetDatabase.ImportAsset(asmDefAssetPath);
        }

        public static bool ValidateCustomAssemblyName(AssemblyDefinitionAsset assemblyDefinition, string sku)
        {
            return GetAssemblyName(assemblyDefinition) == GetAssemblyNameForSKU(sku);
        }

        private static string Sha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static string GetAssemblyNameForSKU(string sku)
        {
            switch (sku == null ? "" : Sha256Hash(sku))
            {
                case "71303bc05617d4e9aa08596f62b1aa3ebb74ffc0000d81d2d0e76df15ab2c088":
                    return "Placeholder1";
                case "d028dbd5e554c43f2c33d58603af76c6e70b5c3b99fbcdce157e343718047310":
                    return "Placeholder3";
                case "1b6ab678554cbf1db00532dcb14d5965f3cb97e66f2d31ba4ee7c8774940176a":
                    return "Placeholder4";
                case "49a73d957e76f92f67eebfa3ee0aa08dc812a0ef412b606bbc790c92a30d4768":
                    return "Placeholder5";
                case "5dc94cecece2c7be476b4b9d98855c9ee35347f7d0cbe8bd7b262568c84b2a39":
                    return "Placeholder6";
                case "4d4ddbcfc4d4ffe33487828b250d76b8f88031289314e299dbbf0fde49a3e6c6":
                    return "Placeholder7";
                case "319000e8795a6e199ae559cbbe9ae2953f2ae6b3d61358fd83cec5f6da3044af":
                    return "Placeholder8";
                case "fdd6834435e4f96fa3621d87e3232d475e8eee641c041989030bae7d1f736bda":
                    return "Placeholder9";
                default:
                    return CSScriptingUtility.DEFAULT_CSHARP_ASSEMBLY_NAME;
            }
        }

        public static bool CompileAssembly(AssemblyDefinitionAsset assemblyDefinition, string sku)
        {
            string assemblyName = GetAssemblyNameForSKU(sku);

            if (!ValidateCustomAssemblyName(assemblyDefinition, sku))
            {
                Debug.LogError($"Failed to compile c# assembly: Assembly name must be {assemblyName}; Did you forget to call EnforceCustomAssemblyName");
                return false;
            }

            // Compile
            try
            {
                BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
                BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
                ScriptCompilationSettings scriptCompilationSettings = new() {
                    target = buildTarget,
                    group = buildTargetGroup,
                    options = ScriptCompilationOptions.None,
                };

                string outputDir = Path.Combine(COMPILE_DESTINATION_DIR, buildTarget.ToString());
                if (!Directory.Exists(outputDir))
                    Directory.CreateDirectory(outputDir);

                PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, outputDir);

                // Copy dll to generated folder
                string dllPath = Path.Combine(outputDir, assemblyName + ".dll");
                if (File.Exists(dllPath))
                {
                    string dllAssetPathOutputDir = Path.GetDirectoryName(OUTPUT_ASSET_PATH);
                    if (!Directory.Exists(dllAssetPathOutputDir))
                        Directory.CreateDirectory(dllAssetPathOutputDir);

                    File.Copy(dllPath, OUTPUT_ASSET_PATH, true);
                    AssetDatabase.ImportAsset(OUTPUT_ASSET_PATH);
                    AssetDatabase.Refresh();
                    return true;
                }
                else
                {
                    Debug.LogError($"Failed to compile C# assembly; Output dll not found at {dllPath}\n{assemblyDefinition}");
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to compile c# assembly: {e}\n{assemblyDefinition}");
                return false;
            }
        }

        public static string GetAssemblyName(AssemblyDefinitionAsset assemblyDefinition)
        {
            string txt = File.ReadAllText(AssetDatabase.GetAssetPath(assemblyDefinition));
            Match match = Regex.Match(txt, "\"name\":\\s*\"(.*?)\"");
            if (match.Success)
                return match.Groups[1].Value;

            return null;
        }
    }
}