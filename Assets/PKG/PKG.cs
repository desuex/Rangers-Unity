using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.PKG
{
    class PKG
    {
        const string fileName = "common.pkg";
        public void LoadResources()
        {
            var fullPath = Path.Combine(Application.persistentDataPath, fileName);
            if (File.Exists(fullPath))
            {
                using (BinaryReader reader = new BinaryReader(System.IO.File.Open(fullPath, FileMode.Open)))
                {

                    var magic = reader.ReadInt32();
                    Debug.Log("Magic is: " + magic.ToString("X"));
                }


            }else
            {
                Debug.Log(fullPath);
            }
        }
    }
}
