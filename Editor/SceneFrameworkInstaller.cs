using UnityEditor;
using UnityEngine;
using System.IO;

public static class SceneFrameworkInstaller
{
    private const string SourcePath = "Assets/Samples/Scene Framework/1.0.0/Essential Scene Assets";
    private const string TargetPath = "Assets/SceneFramework";

    [MenuItem("Tools/Hybrid Scene Framework/Move Imported Samples to Assets")]
    public static void MoveSamplesToAssets()
    {
        if (!Directory.Exists(SourcePath))
        {
            EditorUtility.DisplayDialog("Scene Framework Installer", 
                "샘플이 설치되어 있지 않습니다.\n먼저 Package Manager에서 샘플을 Import 해주세요.", 
                "확인");
            return;
        }

        if (Directory.Exists(TargetPath))
        {
            bool overwrite = EditorUtility.DisplayDialog("경고: 기존 폴더 존재",
                "이미 'Assets/SceneFramework' 폴더가 존재합니다.\n덮어쓸까요?",
                "덮어쓰기", "취소");

            if (!overwrite) return;

            FileUtil.DeleteFileOrDirectory(TargetPath);
        }

        FileUtil.CopyFileOrDirectory(SourcePath, TargetPath);
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Scene Framework Installer", 
            "✅ 에셋이 Assets/SceneFramework 경로로 복사되었습니다!", 
            "좋아요!");
    }
}