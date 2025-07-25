using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Assets
{
    internal class WAV
    {
        public float[] LeftChannel { get; private set; }
        public int ChannelCount { get; private set; }
        public int SampleCount { get; private set; }
        public int Frequency { get; private set; }
        public WAV(byte[] wav)
        {
            ChannelCount = wav[22];
            Frequency = wav[24] | wav[25] << 8;
            int num = 44;
            int num2 = (wav.Length - num) / 2;
            SampleCount = num2;
            LeftChannel = new float[num2];
            for (int i = 0; i < num2; i++)
            {
                short num3 = (short)(wav[num] | wav[num + 1] << 8);
                LeftChannel[i] = num3 / 32768f;
                num += 2;
            }
        }
    }
}
