using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FbxToPNGEditor : EditorWindow
{
    private string folderPath = "Assets";
    private string outputPath = "Assets";

    [MenuItem("Tools/Fbx To PNG Converter")]
    public static void ShowWindow()
    {
        GetWindow<FbxToPNGEditor>("FBX to PNG Converter");
    }

    void OnGUI()
    {
        GUILayout.Label("Convertir des Prefabs en images PNG", EditorStyles.boldLabel);

        GUILayout.Space(10);
        if (GUILayout.Button("Sélectionner le dossier source"))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Choisir un dossier dans Assets", "Assets", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (selectedPath.StartsWith(Application.dataPath))
                {
                    folderPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
                else
                {
                    Debug.LogWarning("Le dossier doit être dans le dossier 'Assets'");
                }
            }
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Sélectionner le dossier de sortie"))
        {
            string selectedPath2 = EditorUtility.OpenFolderPanel("Choisir un dossier dans Assets", "Assets", "");
            if (!string.IsNullOrEmpty(selectedPath2))
            {
                if (selectedPath2.StartsWith(Application.dataPath))
                {
                    outputPath = "Assets" + selectedPath2.Substring(Application.dataPath.Length);
                }
                else
                {
                    Debug.LogWarning("Le dossier doit être dans le dossier 'Assets'");
                }
            }
        }

        EditorGUILayout.LabelField("Dossier source :", folderPath);
        EditorGUILayout.LabelField("Dossier de sortie :", outputPath);

        GUILayout.Space(10);
        if (GUILayout.Button("Lancer la conversion"))
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("Le dossier spécifié n'est pas valide !");
                return;
            }

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            ConvertPrefabsToPNG(folderPath, outputPath);
        }
    }

    private void ConvertPrefabsToPNG(string sourcePath, string savePath)
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { sourcePath });
        List<GameObject> prefabs = new();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab != null)
                prefabs.Add(prefab);
        }

        int total = prefabs.Count;
        Debug.Log($"Nombre de prefabs trouvés : {total}");

        for (int i = 0; i < total; i++)
        {
            GameObject prefab = prefabs[i];
            EditorUtility.DisplayProgressBar("Génération des aperçus", prefab.name, (float)i / total);

            Texture2D preview = AssetPreview.GetAssetPreview(prefab);

            // attendre que l'aperçu soit prêt
            int wait = 0;
            while (preview == null && wait < 50)
            {
                System.Threading.Thread.Sleep(100);
                preview = AssetPreview.GetAssetPreview(prefab);
                wait++;
            }

            if (preview != null)
            {
                preview = MakeBackgroundTransparent(preview);
                byte[] bytes = preview.EncodeToPNG();
                string filePath = Path.Combine(savePath, $"{prefab.name}.png");
                File.WriteAllBytes(filePath, bytes);
                Debug.Log($"Aperçu sauvegardé : {filePath}");
            }
            else
            {
                Debug.LogWarning($"Impossible de générer l'aperçu pour : {prefab.name}");
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        Debug.Log("Conversion terminée !");
    }

    static Texture2D MakeBackgroundTransparent(Texture2D original)
    {
        Color backgroundColor = new Color(82 / 255f, 82 / 255f, 82 / 255f, 1f);
        float tolerance = 0.05f;

        Texture2D transparentTexture = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
        for (int y = 0; y < original.height; y++)
        {
            for (int x = 0; x < original.width; x++)
            {
                Color pixel = original.GetPixel(x, y);

                if (IsColorSimilar(pixel, backgroundColor, tolerance))
                    pixel.a = 0;

                transparentTexture.SetPixel(x, y, pixel);
            }
        }

        transparentTexture.Apply();
        return transparentTexture;
    }

    static bool IsColorSimilar(Color c1, Color c2, float tolerance)
    {
        return Mathf.Abs(c1.r - c2.r) < tolerance &&
               Mathf.Abs(c1.g - c2.g) < tolerance &&
               Mathf.Abs(c1.b - c2.b) < tolerance;
    }
}
