﻿using System;
using System.Linq;
using System.Text;
using ASRR.Revit.Core.Exporter.GLTF.Model;

namespace ASRR.Revit.Core.Exporter.GLTF.Export
{
    public static class GlbJsonInfo
    {
        public static byte[] Get(string json)
        {
            GlbJson glbJson = new GlbJson();

            if (json.Length % 4 != 0)
            {
                int missingNumbers = 4 - (json.Length % 4);
                json = json.PadRight(json.Length + missingNumbers);
            }

            glbJson.ChunkData = Encoding.ASCII.GetBytes(json);
            glbJson.Length = BitConverter.GetBytes(Convert.ToUInt32(glbJson.ChunkData.Length));

            byte[] result = new byte[] { };
            result = result.Concat(glbJson.Length).ToArray();
            result = result.Concat(glbJson.ChunkType()).ToArray();
            result = result.Concat(glbJson.ChunkData).ToArray();

            return result;
        }
    }
}
