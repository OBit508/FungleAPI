using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Cosmetics.AnimatedColors
{
    [Serializable]
    internal struct HSBColor
    {
        public HSBColor(float h, float s, float b, float a)
        {
            this.h = h;
            this.s = s;
            this.b = b;
            this.a = a;
        }
        public HSBColor(float h, float s, float b)
        {
            this.h = h;
            this.s = s;
            this.b = b;
            this.a = 1f;
        }
        public HSBColor(Color col)
        {
            HSBColor hsbcolor = HSBColor.FromColor(col);
            this.h = hsbcolor.h;
            this.s = hsbcolor.s;
            this.b = hsbcolor.b;
            this.a = hsbcolor.a;
        }
        public static HSBColor FromColor(Color color)
        {
            HSBColor hsbcolor = new HSBColor(0f, 0f, 0f, color.a);
            float r = color.r;
            float g = color.g;
            float num = color.b;
            float num2 = Mathf.Max(r, Mathf.Max(g, num));
            HSBColor result;
            if (num2 <= 0f)
            {
                result = hsbcolor;
            }
            else
            {
                float num3 = Mathf.Min(r, Mathf.Min(g, num));
                float num4 = num2 - num3;
                if (num2 > num3)
                {
                    if (g == num2)
                    {
                        hsbcolor.h = (num - r) / num4 * 60f + 120f;
                    }
                    else if (num == num2)
                    {
                        hsbcolor.h = (r - g) / num4 * 60f + 240f;
                    }
                    else if (num > g)
                    {
                        hsbcolor.h = (g - num) / num4 * 60f + 360f;
                    }
                    else
                    {
                        hsbcolor.h = (g - num) / num4 * 60f;
                    }
                    if (hsbcolor.h < 0f)
                    {
                        hsbcolor.h += 360f;
                    }
                }
                else
                {
                    hsbcolor.h = 0f;
                }
                hsbcolor.h *= 0.0027777778f;
                hsbcolor.s = num4 / num2 * 1f;
                hsbcolor.b = num2;
                result = hsbcolor;
            }
            return result;
        }
        public static Color ToColor(HSBColor hsbColor)
        {
            float value = hsbColor.b;
            float value2 = hsbColor.b;
            float value3 = hsbColor.b;
            if (hsbColor.s != 0f)
            {
                float num = hsbColor.b;
                float num2 = hsbColor.b * hsbColor.s;
                float num3 = hsbColor.b - num2;
                float num4 = hsbColor.h * 360f;
                if (num4 < 60f)
                {
                    value = num;
                    value2 = num4 * num2 / 60f + num3;
                    value3 = num3;
                }
                else if (num4 < 120f)
                {
                    value = -(num4 - 120f) * num2 / 60f + num3;
                    value2 = num;
                    value3 = num3;
                }
                else if (num4 < 180f)
                {
                    value = num3;
                    value2 = num;
                    value3 = (num4 - 120f) * num2 / 60f + num3;
                }
                else if (num4 < 240f)
                {
                    value = num3;
                    value2 = -(num4 - 240f) * num2 / 60f + num3;
                    value3 = num;
                }
                else if (num4 < 300f)
                {
                    value = (num4 - 240f) * num2 / 60f + num3;
                    value2 = num3;
                    value3 = num;
                }
                else if (num4 <= 360f)
                {
                    value = num;
                    value2 = num3;
                    value3 = -(num4 - 360f) * num2 / 60f + num3;
                }
                else
                {
                    value = 0f;
                    value2 = 0f;
                    value3 = 0f;
                }
            }
            return new Color(Mathf.Clamp01(value), Mathf.Clamp01(value2), Mathf.Clamp01(value3), hsbColor.a);
        }
        public Color ToColor()
        {
            return HSBColor.ToColor(this);
        }
        public override string ToString()
        {
            return string.Concat(new string[]
            {
            "H:",
            this.h.ToString(),
            " S:",
            this.s.ToString(),
            " B:",
            this.b.ToString()
            });
        }
        public static HSBColor Lerp(HSBColor a, HSBColor b, float t)
        {
            float num;
            float num2;
            if (a.b == 0f)
            {
                num = b.h;
                num2 = b.s;
            }
            else if (b.b == 0f)
            {
                num = a.h;
                num2 = a.s;
            }
            else
            {
                if (a.s == 0f)
                {
                    num = b.h;
                }
                else if (b.s == 0f)
                {
                    num = a.h;
                }
                else
                {
                    float num3;
                    for (num3 = Mathf.LerpAngle(a.h * 360f, b.h * 360f, t); num3 < 0f; num3 += 360f)
                    {
                    }
                    while (num3 > 360f)
                    {
                        num3 -= 360f;
                    }
                    num = num3 / 360f;
                }
                num2 = Mathf.Lerp(a.s, b.s, t);
            }
            return new HSBColor(num, num2, Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));
        }
        public float h;
        public float s;
        public float b;
        public float a;
    }

}
