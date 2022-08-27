using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditorInternal;
using UnityEngine;

namespace Kogane.Internal
{
    [InitializeOnLoad]
    internal static class PackageEmbedder
    {
        static PackageEmbedder()
        {
            Editor.finishedDefaultHeaderGUI -= OnGUI;
            Editor.finishedDefaultHeaderGUI += OnGUI;
        }

        private static void OnGUI( Editor editor )
        {
            var packageName = GetPackageName( editor.target );

            if ( string.IsNullOrWhiteSpace( packageName ) ) return;

            var oldEnabled = GUI.enabled;
            GUI.enabled = true;

            try
            {
                if ( !GUILayout.Button( "Embed", EditorStyles.miniButton ) ) return;

                Client.Embed( packageName );
            }
            finally
            {
                GUI.enabled = oldEnabled;
            }
        }

        private static string GetPackageName( Object target )
        {
            if ( target == null ) return string.Empty;
            if ( !EditorUtility.IsPersistent( target ) ) return string.Empty;

            var assetPath = AssetDatabase.GetAssetPath( target );

            if ( string.IsNullOrWhiteSpace( assetPath ) ) return string.Empty;

            var directoryName = Path.GetDirectoryName( assetPath );

            if ( string.IsNullOrWhiteSpace( directoryName ) ) return string.Empty;

            if ( target is PackageManifestImporter )
            {
                return Path.GetFileName( directoryName );
            }

            return directoryName == "Packages"
                    ? Path.GetFileName( assetPath )
                    : string.Empty
                ;
        }
    }

    // /// <summary>
    // /// Project ウィンドウを右クリックして
    // /// 選択したパッケージを埋め込みパッケージに変更できるエディタ拡張
    // /// </summary>
    // internal static class PackageEmbedder
    // {
    //     //==============================================================================
    //     // 定数
    //     //==============================================================================
    //     private const string MENU_ITEM_NAME     = "Assets/Embed Package";
    //     private const int    MENU_ITEM_PRIORITY = 1200;
    //
    //     //==============================================================================
    //     // 関数(static)
    //     //==============================================================================
    //     /// <summary>
    //     /// 選択中のアセットが埋め込めるパッケージか確認する時に呼び出されます
    //     /// </summary>
    //     [MenuItem( MENU_ITEM_NAME, true )]
    //     private static bool EmbedPackageValidation()
    //     {
    //         var assetPath = GetSelectedAssetPath();
    //
    //         if ( string.IsNullOrWhiteSpace( assetPath ) ) return false;
    //
    //         var directoryName = Path.GetDirectoryName( assetPath );
    //
    //         return directoryName == "Packages";
    //     }
    //
    //     /// <summary>
    //     /// 選択中のパッケージを埋め込みます
    //     /// </summary>
    //     [MenuItem( MENU_ITEM_NAME, false, MENU_ITEM_PRIORITY )]
    //     private static void EmbedPackage()
    //     {
    //         var packageName = Path.GetFileName( GetSelectedAssetPath() );
    //
    //         if ( string.IsNullOrWhiteSpace( packageName ) ) return;
    //
    //         Client.Embed( packageName );
    //
    //         AssetDatabase.Refresh();
    //     }
    //
    //     /// <summary>
    //     /// 選択中のアセットのパスを返します
    //     /// </summary>
    //     private static string GetSelectedAssetPath()
    //     {
    //         // Selection.activeObject だとフォルダの情報が取得できないようなので
    //         // Selection.assetGUIDs を使用しています
    //         var assetGUIDs = Selection.assetGUIDs;
    //
    //         if ( assetGUIDs == null || assetGUIDs.Length == 0 ) return string.Empty;
    //
    //         var assetGuid = assetGUIDs[ 0 ];
    //         var assetPath = AssetDatabase.GUIDToAssetPath( assetGuid );
    //
    //         return assetPath;
    //     }
    // }
}