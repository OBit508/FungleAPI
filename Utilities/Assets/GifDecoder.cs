using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FungleAPI.Utilities.Assets
{
    internal class GifDecoder
    {
        public List<Texture2D> Frames = new List<Texture2D>();
        public List<float> FrameDelays = new List<float>();
        private byte[] globalColorTable;
        private int width, height;
        private Color32[] prevFramePixels;
        public void LoadGif(byte[] gif)
        {
            Frames.Clear();
            FrameDelays.Clear();
            using (BinaryReader reader = new BinaryReader(new MemoryStream(gif)))
            {
                string signature = new string(reader.ReadChars(3));
                string version = new string(reader.ReadChars(3));
                if (signature != "GIF") return;
                width = reader.ReadUInt16();
                height = reader.ReadUInt16();
                byte packed = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                if ((packed & 0x80) != 0)
                {
                    globalColorTable = reader.ReadBytes(3 * 2 << (packed & 0x07));
                }
                prevFramePixels = new Color32[width * height];
                for (int i = 0; i < prevFramePixels.Length; i++)
                {
                    prevFramePixels[i] = new Color32(0, 0, 0, 0);
                }
                int transparentIndex = -1;
                int disposalMethod = 0;
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    switch (reader.ReadByte())
                    {
                        case 0x21:
                            if (reader.ReadByte() == 0xF9)
                            {
                                reader.ReadByte(); // block size (sempre 4)
                                byte packedGCE = reader.ReadByte();

                                disposalMethod = packedGCE >> 2 & 0x07; // <- extrair disposal method
                                bool transparencyFlag = (packedGCE & 0x01) != 0;

                                int delay = reader.ReadUInt16();
                                transparentIndex = reader.ReadByte();
                                reader.ReadByte(); // terminator

                                FrameDelays.Add(delay / 100f);
                            }
                            else
                            {
                                SkipDataBlocks(reader);
                            }
                            break;

                        case 0x2C:
                            int imgX = reader.ReadUInt16();
                            int imgY = reader.ReadUInt16();
                            int imgW = reader.ReadUInt16();
                            int imgH = reader.ReadUInt16();
                            byte imgPacked = reader.ReadByte();
                            byte[] colorTable = (imgPacked & 0x80) != 0 ? reader.ReadBytes(3 * 2 << (imgPacked & 0x07)) : globalColorTable;
                            int lzwMinCodeSize = reader.ReadByte();
                            byte[] imageData = ReadDataBlocks(reader);
                            byte[] decodedPixels = LzwDecode(imageData, lzwMinCodeSize, imgW * imgH);
                            Color32[] framePixels = new Color32[width * height];
                            Array.Copy(prevFramePixels, framePixels, prevFramePixels.Length);

                            // 2. Aplica disposal antes de desenhar
                            if (disposalMethod == 2) // Restore to background
                            {
                                for (int y = 0; y < imgH; y++)
                                {
                                    for (int x = 0; x < imgW; x++)
                                    {
                                        int globalIndex = (height - 1 - (imgY + y)) * width + imgX + x;
                                        if (globalIndex >= 0 && globalIndex < framePixels.Length)
                                            framePixels[globalIndex] = new Color32(0, 0, 0, 0); // limpa
                                    }
                                }
                            }
                            // (disposalMethod == 3) exigiria salvar um backup de prevFramePixels de antes, se quiser suportar)

                            // 3. Agora desenha os pixels do frame atual
                            for (int y = 0; y < imgH; y++)
                            {
                                for (int x = 0; x < imgW; x++)
                                {
                                    int pixelIndex = decodedPixels[y * imgW + x];
                                    if (pixelIndex == transparentIndex) continue;

                                    int globalIndex = (height - 1 - (imgY + y)) * width + imgX + x;
                                    if (globalIndex >= 0 && globalIndex < framePixels.Length)
                                    {
                                        framePixels[globalIndex] = new Color32(
                                            colorTable[pixelIndex * 3],
                                            colorTable[pixelIndex * 3 + 1],
                                            colorTable[pixelIndex * 3 + 2],
                                            255
                                        );
                                    }
                                }
                            }

                            // Atualiza o buffer do próximo frame
                            prevFramePixels = framePixels;
                            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
                            tex.SetPixels32(framePixels);
                            tex.Apply();
                            Frames.Add(tex);
                            break;
                    }
                }
            }
        }
        private static void SkipDataBlocks(BinaryReader reader)
        {
            byte size;
            while ((size = reader.ReadByte()) != 0)
            {
                reader.ReadBytes(size);
            }
        }
        private static byte[] ReadDataBlocks(BinaryReader reader)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte size;
                while ((size = reader.ReadByte()) != 0)
                {
                    ms.Write(reader.ReadBytes(size), 0, size);
                }
                return ms.ToArray();
            }
        }
        private static byte[] LzwDecode(byte[] data, int minCodeSize, int expectedSize)
        {
            int clearCode = 1 << minCodeSize;
            int endCode = clearCode + 1;
            int codeSize = minCodeSize + 1;
            int dictSize = clearCode + 2;
            List<List<byte>> dictionary = new List<List<byte>>(4096);
            for (int i = 0; i < clearCode; i++)
            {
                dictionary.Add(new List<byte> { (byte)i });
            }
            dictionary.Add(null);
            dictionary.Add(null);
            List<byte> output = new List<byte>(expectedSize);
            int dataPos = 0, bitPos = 0;
            Func<int> ReadCode = () =>
            {
                int rawCode = 0;
                int bitsRead = 0;
                while (bitsRead < codeSize)
                {
                    if (dataPos >= data.Length) return -1;
                    rawCode |= data[dataPos] >> bitPos << bitsRead;
                    int bitsFromThisByte = Math.Min(8 - bitPos, codeSize - bitsRead);
                    bitPos += bitsFromThisByte;
                    bitsRead += bitsFromThisByte;
                    if (bitPos >= 8) { bitPos = 0; dataPos++; }
                }
                return rawCode & (1 << codeSize) - 1;
            };
            int prevCode = -1;
            while (true)
            {
                int code = ReadCode();
                if (code == -1 || code == endCode) break;
                if (code == clearCode)
                {
                    dictionary.RemoveRange(clearCode + 2, dictionary.Count - (clearCode + 2));
                    codeSize = minCodeSize + 1;
                    dictSize = clearCode + 2;
                    prevCode = -1;
                    continue;
                }

                List<byte> entry = null;
                if (code < dictionary.Count)
                {
                    entry = dictionary[code];
                }
                else if (code == dictSize)
                {
                    entry = new List<byte>(dictionary[prevCode]);
                    entry.Add(dictionary[prevCode][0]);
                }
                output.AddRange(entry);
                if (prevCode != -1)
                {
                    List<byte> newEntry = new List<byte>(dictionary[prevCode]);
                    newEntry.Add(entry[0]);
                    dictionary.Add(newEntry);
                    dictSize++;
                    if (dictSize == 1 << codeSize && codeSize < 12)
                        codeSize++;
                }
                prevCode = code;
            }
            return output.ToArray();
        }
    }
}
