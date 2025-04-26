using DaftAppleGames.Editor;
using UnityEditor;
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Editor
{
    public class TpCharacterControllerPackageInitializerEditorWindow : PackageInitializerEditorWindow
    {
        protected override string ToolTitle => "TPC Controller Installer";

        protected override string WelcomeLogText =>
            "Welcome to the TPC Controller installer!";

        protected override void CreateCustomGUI()
        {
        }

        [MenuItem("Daft Apple Games/Packages/TPC Controller")]
        public static void ShowWindow()
        {
            TpCharacterControllerPackageInitializerEditorWindow packageInitWindow = GetWindow<TpCharacterControllerPackageInitializerEditorWindow>();
            packageInitWindow.titleContent = new GUIContent("TPC Controller Installer");
        }
    }
}