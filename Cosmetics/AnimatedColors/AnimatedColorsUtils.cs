using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Cosmetics.AnimatedColors
{
    internal static class AnimatedColorsUtils
    {
        public static Color Rainbow => HSB(Ping(0f, 1f, 0.25f), 0.9f, 1f);
        public static Color RainbowShadow => Shadow(Rainbow, 0.35f);
        public static Color Galaxy
        {
            get
            {
                float cycle = Mathf.Repeat(Time.time, 3f);
                float flashStrength = 0f;
                if (cycle < 0.1f)
                {
                    float x = cycle / 0.1f;
                    flashStrength = Mathf.SmoothStep(0f, 0.35f, x);
                }
                else if (cycle < 0.2f)
                {
                    float x = (cycle - 0.1f) / 0.1f;
                    flashStrength = Mathf.SmoothStep(0.35f, 0f, x);
                }
                return Color.Lerp(Color.Lerp(new Color(0.32f, 0.12f, 0.55f), new Color(0.08f, 0.18f, 0.42f), Mathf.PingPong(Time.time * 0.15f, 1f)), Color.white, flashStrength);
            }
        }
        public static Color GalaxyShadow => Shadow(Galaxy, 0.45f);
        private static float fireNextChange;
        private static Color fireCurrent;
        private static Color fireTarget;
        public static Color Fire
        {
            get
            {
                if (Time.time >= fireNextChange)
                {
                    fireNextChange = Time.time + UnityEngine.Random.Range(0.25f, 0.5f);
                    fireTarget = RandomFireColor();
                }
                fireCurrent = Color.Lerp(fireCurrent == default ? fireTarget : fireCurrent, fireTarget, Time.deltaTime * 2.5f);
                return fireCurrent;
            }
        }
        public static Color FireShadow => Shadow(Fire, 0.5f);
        private static Color RandomFireColor()
        {
            return new HSBColor(UnityEngine.Random.Range(0.03f, 0.12f), UnityEngine.Random.Range(0.85f, 1f), UnityEngine.Random.Range(0.5f, 0.9f)).ToColor();
        }
        private static Color Shadow(Color c, float strength)
        {
            return new Color(Mathf.Clamp01(c.r * (1f - strength)), Mathf.Clamp01(c.g * (1f - strength)), Mathf.Clamp01(c.b * (1f - strength)), c.a);
        }
        private static Color HSB(float h, float s, float b)
        {
            return new HSBColor(h, s, b).ToColor();
        }
        private static float Ping(float min, float max, float speed)
        {
            return min + Mathf.PingPong(Time.time * speed, max - min);
        }
    }
}
