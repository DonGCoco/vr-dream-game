using UnityEngine;
using System.IO;

public class ExportTopViewMap : MonoBehaviour
{
    public Camera topViewCamera;
    public int width = 2048;
    public int height = 2048;
    public string fileName = "TopViewMap.png";

    [ContextMenu("Export PNG")]
    public void ExportPNG()
    {
        if (topViewCamera == null)
        {
            Debug.LogError("TopViewCamera 没有拖进去。");
            return;
        }

        RenderTexture rt = new RenderTexture(width, height, 24);
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        topViewCamera.targetTexture = rt;
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        topViewCamera.Render();

        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        topViewCamera.targetTexture = null;
        RenderTexture.active = currentRT;

        byte[] bytes = tex.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllBytes(path, bytes);

        DestroyImmediate(rt);
        DestroyImmediate(tex);

        Debug.Log("已导出到: " + path);
    }
}