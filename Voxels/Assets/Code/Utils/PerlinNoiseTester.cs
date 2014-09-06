using UnityEngine;
using System.Collections;
using System.IO;

public class PerlinNoiseTester {
    public void CreateTest() {
        int width = 256;
        int height = 256;

        Texture2D tex = new Texture2D(width, height);
        Color[] colors = new Color[65536];

        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                float rnd = Noise.GetNoise(x, y, 0, 50, 2, 0);
                int index = y * height + x;
                colors[index] = new Color(rnd, rnd, rnd);
            }
        }

        tex.SetPixels(colors);

        SaveTextureToFile (tex, "Textures/test.png");
	}
    
    private void SaveTextureToFile(Texture2D tex, string filepath) {
        string fullpath = Application.dataPath + "/" + filepath;
        FileStream file = File.Open(fullpath, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);

        byte[] bytes = tex.EncodeToPNG();
        writer.Write(bytes);
    }
}
