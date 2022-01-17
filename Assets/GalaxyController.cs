using Assets.Graphics;
using Assets.PKG;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GalaxyController : MonoBehaviour
{
    private PKG _pkg;
    private GI _gi;
    // Start is called before the first frame update
    void Start()
    {
        //this._pkg = new PKG();
        //this._pkg.LoadResources();
        this._gi = new GI("2bg.gi");
        Texture2D txt = this._gi.ReadBytes();
        //Debug.Log(txt.width);
        //Debug.Log(txt.height);
        //byte[] bytes = txt.EncodeToPNG();
        //System.IO.File.WriteAllBytes(Application.persistentDataPath + "\\Image" + ".png", bytes);
        var fullPath = Path.Combine(Application.persistentDataPath, "haiframe.bin");
        using (BinaryReader reader = new BinaryReader(System.IO.File.Open(fullPath, FileMode.Open)))
        {
            Texture2D texture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
            byte[] data = reader.ReadBytes(256*256);
            Color32[] colors = new Color32[256];
            Color32[] result = new Color32[256 * 256];
            for (int i = 0; i < 256; i++)
            {
                byte r = reader.ReadByte();
                byte g = reader.ReadByte();
                byte b = reader.ReadByte();
                byte a = reader.ReadByte();
                colors[i] = new Color32(r, g, b, a);
            }
            for (int x = 0; x<256 ; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    var clr = (int)data[x + y * 256];
                    result[x + y * 256] = colors[clr];
                }
            }
            texture.SetPixels32(result);
            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "\\hai" + ".png", bytes);
        }
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(txt, new Rect(0,0,txt.width,txt.height ),new Vector2(0.5f, 0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
