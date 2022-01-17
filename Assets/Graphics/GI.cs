using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Graphics
{
    //! Header of frame in *.gi file
    struct GIFrameHeader
    {
        public uint signature;  //!< Signature
        public uint version;  //!< Version of GI file
        public uint startX;  //!< Left corner
        public uint startY;  //!< Top corner
        public uint finishX;  //!< Right corner
        public uint finishY;  //!< Bottom corner
        public uint rBitmask;  //!< Mask of r color component
        public uint gBitmask;  //!< Mask of g color component
        public uint bBitmask;  //!< Mask of b color component
        public uint aBitmask;  //!< Mask of a color component
        public uint type;  //!< Frame type
        /*!<
         * Variants:
         *  -# 0 - One layer, 16 or 32 bit, depends on mask.
         *  -# 1 - One layer, 16 bit RGB optimized.
         *  -# 2 - Three layers:
         *   -# 16 bit RGB optimized - body
         *   -# 16 bit RGB optimized - outline
         *   -# 6 bit Alpha optimized
         *  -# 3 - Two layers:
         *   -# Indexed RGB colors
         *   -# Indexed Alpha
         *  -# 4 - One layer, indexed RGBA colors
         *  -# 5 - Delta frame of GAI animation.
         */
        public uint layerCount; //!< Number of layers in frame
        public uint unknown1;
        public uint unknown2;
        public uint unknown3;
        public uint unknown4;

    };

    //! Header of layer in *.gi files
    struct GILayerHeader
    {
        public uint seek;  //!< Layer offset in file
        public uint size;  //!< Layer size
        public uint startX;  //!< Layer left corner
        public uint startY;  //!< Layer top corner
        public uint finishX;  //!< Layer rigth corner
        public uint finishY;  //!< Layer bottom corner
        public uint unknown1;
        public uint unknown2;
    };
    class GI
    {
        protected string filename;
        protected GIFrameHeader header;
        public GI(string filename)
        {
            this.filename = filename;
        }

        public Texture2D ReadBytes()
        {
            var fullPath = Path.Combine(Application.persistentDataPath, this.filename);
            if (File.Exists(fullPath))
            {
                using (BinaryReader reader = new BinaryReader(System.IO.File.Open(fullPath, FileMode.Open)))
                {
                    var header = new GIFrameHeader();
                    header.signature = reader.ReadUInt32();
                    header.version = reader.ReadUInt32();
                    header.startX = reader.ReadUInt32();
                    header.startY = reader.ReadUInt32();
                    header.finishX = reader.ReadUInt32();
                    header.finishY = reader.ReadUInt32();
                    header.rBitmask = reader.ReadUInt32();
                    header.gBitmask = reader.ReadUInt32();
                    header.bBitmask = reader.ReadUInt32();
                    header.aBitmask = reader.ReadUInt32();
                    header.type = reader.ReadUInt32();
                    header.layerCount = reader.ReadUInt32();
                    header.unknown1 = reader.ReadUInt32();
                    header.unknown2 = reader.ReadUInt32();
                    header.unknown3 = reader.ReadUInt32();
                    header.unknown4 = reader.ReadUInt32();
                    if (header.signature != 26983)
                    {
                        throw new Exception("Bad GI Format " + this.filename);
                    }
                    this.header = header;
                    var offset = 0;
                    var layers = new GILayerHeader[header.layerCount];
                    for (int i = 0; i < header.layerCount; i++)
                    {
                        layers[i].seek = reader.ReadUInt32();
                        layers[i].size = reader.ReadUInt32();
                        layers[i].startX = reader.ReadUInt32();
                        layers[i].startY = reader.ReadUInt32();
                        layers[i].finishX = reader.ReadUInt32();
                        layers[i].finishY = reader.ReadUInt32();
                        layers[i].unknown1 = reader.ReadUInt32();
                        layers[i].unknown2 = reader.ReadUInt32();

                        layers[i].startX -= header.startX;
                        layers[i].startY -= header.startY;
                        layers[i].finishX -= header.startX;
                        layers[i].finishY -= header.startY;
                    }


                    header.finishX -= header.startX;
                    header.finishY -= header.startY;
                    header.startX = 0;
                    header.startY = 0;

                    var resultFrame = this.loadGIImageData( layers, reader, offset);
                    var output = JsonUtility.ToJson(resultFrame.dimension, true);
                    Debug.Log(output);
                    return resultFrame;
                    
                    //
                    
                    //Debug.Log(this.header);
                }


            }
            else
            {
                Debug.Log("File does not exist: "+fullPath);
            }
            return null;
        }

        private Texture2D loadGIImageData(GILayerHeader[] layers, BinaryReader reader, int offset)
        {
            switch (this.header.type)
            {
                case 0:
                    return this.loadFrameType0(layers, reader, offset);
                case 2:
                    return this.loadFrameType2(layers, reader, offset);
                default:
                    return this.loadFrameType0(layers, reader, offset);
            }
        }

        private Texture2D loadFrameType2(GILayerHeader[] layers, BinaryReader reader, int offset)
        {
            Texture2D texture = new Texture2D((int)this.header.finishX, (int)this.header.finishY, TextureFormat.ARGB32, false);
            if (layers[0].size > 0)
            {
                reader.BaseStream.Seek((offset + layers[0].seek), SeekOrigin.Begin);
                texture = this.drawR5G6B5(texture, layers[0].startX, layers[0].startY, reader);
            }

            if (layers[1].size > 0)
            {
                reader.BaseStream.Seek((offset + layers[1].seek), SeekOrigin.Begin);
                texture = this.drawR5G6B5(texture, layers[1].startX, layers[1].startY, reader);
            }

            if (layers[2].size >0)
            {
                reader.BaseStream.Seek((offset + layers[2].seek), SeekOrigin.Begin);
                texture = this.drawA6(texture, layers[2].startX, layers[2].startY, reader);
            }
            return texture;
        }

        private Texture2D drawA6(Texture2D texture, uint startX, uint startY, BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        private Texture2D drawR5G6B5(Texture2D texture, uint startX, uint startY, object dev)
        {
            throw new NotImplementedException();
        }

        private Texture2D loadFrameType0(GILayerHeader[] layers, BinaryReader reader, int offset)
        {
            Texture2D texture;
            if ((this.header.aBitmask == 0xFF000000) && (this.header.rBitmask == 0xFF0000) && (this.header.gBitmask == 0xFF00) && (this.header.bBitmask == 0xFF))
            {
                //Format_ARGB32
                Debug.Log("ARGB32");
                texture = new Texture2D((int)this.header.finishX, (int)this.header.finishY, TextureFormat.ARGB32, false);
                if (layers[0].size != 0)
                {
                    Debug.Log("ARGB32 valid size");
                    //reader.BaseStream.Seek(offset + layers[0].seek, SeekOrigin.Begin);
                    this.blitARGB(texture, layers[0].startX, layers[0].startY, (layers[0].finishX - layers[0].startX), (layers[0].finishY - layers[0].startY), reader);
                }
                
            }
            else if ((this.header.rBitmask == 0xF800) && (this.header.gBitmask == 0x7E0) && (this.header.bBitmask == 0x1F))
            {
                //Format_RGB16
                Debug.Log("RGB565");
                Debug.Log("width: " +(int)this.header.finishX);
                Debug.Log("height: " + (int)this.header.finishY);

                texture = new Texture2D((int)this.header.finishX, (int)this.header.finishY, TextureFormat.RGB565, false);
                this.blitR5G6B5(texture, layers[0].startX, layers[0].startY, (layers[0].finishX - layers[0].startX), (layers[0].finishY - layers[0].startY), reader);
            } else
            {
                Debug.Log("weird texture");
                Debug.Log(this.header.aBitmask);
                Debug.Log(0xFF000000);
                Debug.Log(this.header.aBitmask == 0xFF000000);
                Debug.Log(this.header.rBitmask == 0xFF0000);
                Debug.Log(this.header.gBitmask == 0xFF00);
                Debug.Log(this.header.bBitmask == 0xFF);
                texture = new Texture2D((int)this.header.finishX, (int)this.header.finishY, TextureFormat.RGB565, false);
            }
            return texture;
        }

        private Texture2D blitR5G6B5(Texture2D texture, long x, long y, long w, long h, BinaryReader reader)
        {
            var data = reader.ReadBytes((int)(2 * w * h));
            Debug.Log("Data size: " + data.Length);
            Debug.Log("Txt size: " + texture.GetRawTextureData().Length);
            texture.LoadRawTextureData(data);
            texture.Apply();
            return texture;
        }

        private Texture2D blitARGB(Texture2D texture, long x, long y, long w, long h, BinaryReader reader)
        {
            var data = reader.ReadBytes((int)(4 * w * h));
            Debug.Log("Data size: " + data.Length);
            Debug.Log("Txt size: " + texture.GetRawTextureData().Length);
            texture.LoadRawTextureData(data);
            texture.Apply();
            return texture;
            
        }
    }
}
