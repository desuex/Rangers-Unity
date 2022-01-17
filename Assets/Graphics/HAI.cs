using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Graphics
{
    struct HAIHeader
    {
        public uint signature;  //!< Signature
        public uint width;  //!< Animation width
        public uint height;  //!< Animation height
        public uint rowBytes;  //!< Bytes in one line
        public uint count;  //!< Number of frames in animation
        public uint frameSize;  //!< Size of one frame
        public uint unknown1;
        public uint unknown2;
        public uint unknown3;
        public uint unknown4;
        public uint unknown5;
        public uint unknown6;
        public uint palSize;  //!< Size of pallete
    };
    class HAI
    {
        protected string filename;
        protected HAIHeader header;
        public HAI(string filename)
        {
            this.filename = filename;
        }

        internal Sprite[] LoadBytes()
        {
            var fullPath = Path.Combine(Application.persistentDataPath, this.filename);
            if (File.Exists(fullPath))
            {
                using (BinaryReader reader = new BinaryReader(System.IO.File.Open(fullPath, FileMode.Open)))
                {
                    var header = new HAIHeader();
                    header.signature = reader.ReadUInt32();
                    header.width = reader.ReadUInt32();
                    header.height = reader.ReadUInt32();
                    header.rowBytes = reader.ReadUInt32();
                    header.count = reader.ReadUInt32();
                    header.frameSize = reader.ReadUInt32();
                    header.unknown1 = reader.ReadUInt32();
                    header.unknown2 = reader.ReadUInt32();
                    header.unknown3 = reader.ReadUInt32();
                    header.unknown4 = reader.ReadUInt32();
                    header.unknown5 = reader.ReadUInt32();
                    header.unknown6 = reader.ReadUInt32();
                    header.palSize = reader.ReadUInt32();
                    this.header = header;
                    Sprite[] sprites = new Sprite[header.count];
                    for (int i = 0; i < header.count; i++)
                    {
                        sprites[i] = this.loadHAIFrame(reader, i);
                    }

                    return sprites;
                }
            }
            throw new FileNotFoundException("HAI file not found: " + fullPath);
            
        }

        private Sprite loadHAIFrame(BinaryReader reader, int i)
        {
            var offset = sizeof(uint) * 13 + i * (this.header.height * this.header.rowBytes + this.header.palSize);
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            Texture2D texture = new Texture2D((int)this.header.width, (int)this.header.height, TextureFormat.ARGB32, false);
            byte[] data = reader.ReadBytes((int)this.header.width * (int)this.header.height);
            Color32[] colors = new Color32[this.header.palSize/4];
            Color32[] result = new Color32[(int)this.header.width * (int)this.header.height];
            for (int c = 0; c < (this.header.palSize / 4); c++)
            {
                byte r = reader.ReadByte();
                byte g = reader.ReadByte();
                byte b = reader.ReadByte();
                byte a = reader.ReadByte();
                colors[c] = new Color32(r, g, b, a);
            }
            for (int x = 0; x < this.header.width; x++)
            {
                for (int y = 0; y < this.header.height; y++)
                {
                    var clr = (int)data[x + y * this.header.rowBytes];
                    result[x + y * this.header.rowBytes] = colors[clr];
                }
            }
            texture.SetPixels32(result);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            //Texture2D texture = new Texture2D((int)this.header.width, (int)this.header.height, TextureFormat.R8, false);
            //var offset = sizeof(uint)*13 + i * (this.header.height * this.header.rowBytes + this.header.palSize);
            //reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            //texture = this.blitARGBI(texture, 0, 0, this.header.width, this.header.height, reader);
            //throw new NotImplementedException();
        }

        private Texture2D blitARGBI(Texture2D texture, int v1, int v2, uint width, uint height, BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
