using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class CutAndPaste
{
    private const string RecordID = "Cut And Paste";

    [MenuItem("Assets/Cut", false)]
    private static void Cut()
    {
        var paths = new StringBuilder();
        foreach (var item in Selection.objects)
        {
            paths.AppendLine(AssetDatabase.GetAssetPath(item));
        }
        EditorPrefs.SetString(RecordID, paths.ToString());
    }

    [MenuItem("Assets/Paste", false)]
    private static void Paste()
    {
        if (Selection.objects.Length == 1)
        {
            var directory = AssetDatabase.GetAssetPath(Selection.objects[0]);
            var absolutePath = Path.Combine(Application.dataPath, "..", directory);
            if (File.Exists(absolutePath))
            {
                var len = new FileInfo(absolutePath).Name.Length;
                directory = directory.Remove(directory.Length - len, len);
            }
            var paths = EditorPrefs.GetString(RecordID);
            if (!string.IsNullOrEmpty(paths))
            {
                using var reader = new StringReader(paths);
                string oldPath;
                while (!string.IsNullOrEmpty(oldPath = reader.ReadLine()))
                {
                    var fileInfo = new FileInfo(oldPath);
                    AssetDatabase.MoveAsset(oldPath, Path.Combine(directory, fileInfo.Name));
                }
            }
            EditorPrefs.SetString(RecordID, string.Empty);
        }
    }
}
