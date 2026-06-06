using FungleAPI.Assets;
using FungleAPI.Attributes;
using FungleAPI.Extensions;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngineInternal;
using static Rewired.Controller;

namespace FungleAPI.Components
{
    /// <summary>
    /// A component that Renders a GIF
    /// </summary>
    [RegisterTypeInIl2Cpp]
    public class GIFAnimator : MonoBehaviour
    {
        public GIF GIF;
        public float Speed = 1;
        public bool Stoped;
        private Color color = Color.white;
        private bool flipX;
        private bool flipY;
        private int sortingOrder;
        private int sortingLayerID;
        private string sortingLayerName = "Default";
        private SpriteMaskInteraction maskInteraction = SpriteMaskInteraction.None;
        private Material material;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private float timer;
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                if (material != null)
                    material.color = value;
            }
        }
        public bool FlipX
        {
            get => flipX;
            set
            {
                if (flipX == value) return;
                flipX = value;
                ApplyFlip();
            }
        }
        public bool FlipY
        {
            get => flipY;
            set
            {
                if (flipY == value) return;
                flipY = value;
                ApplyFlip();
            }
        }
        public int SortingOrder
        {
            get => sortingOrder;
            set
            {
                sortingOrder = value;
                if (meshRenderer != null)
                {
                    meshRenderer.sortingOrder = value;
                }
            }
        }
        public int SortingLayerID
        {
            get => sortingLayerID;
            set
            {
                sortingLayerID = value;
                if (meshRenderer != null)
                {
                    meshRenderer.sortingLayerID = value;
                }
            }
        }
        public string SortingLayerName
        {
            get => sortingLayerName;
            set
            {
                sortingLayerName = value;
                if (meshRenderer != null)
                {
                    meshRenderer.sortingLayerName = value;
                }
            }
        }
        public SpriteMaskInteraction MaskInteraction
        {
            get => maskInteraction;
            set
            {
                maskInteraction = value;
                ApplyMaskInteraction();
            }
        }
        public Material Material
        {
            get => material;
            set
            {
                if (material == value) return;
                material?.Destroy();
                meshRenderer.material = value;
                material = meshRenderer.material;
                material.color = color;
            }
        }
        public void Awake()
        {
            meshRenderer = gameObject.GetOrAddComponent<MeshRenderer>();

            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
                meshFilter.mesh = new Mesh();
                meshFilter.mesh.vertices = new Vector3[4]
                {
                    new Vector3(-0.5f, -0.5f, 0),
                    new Vector3( 0.5f, -0.5f, 0),
                    new Vector3( 0.5f,  0.5f, 0),
                    new Vector3(-0.5f,  0.5f, 0)
                };
                meshFilter.mesh.uv = new Vector2[4]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1)
                };
                meshFilter.mesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
                meshFilter.mesh.RecalculateNormals();
                meshFilter.mesh.RecalculateBounds();
            }

            meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
            material = meshRenderer.material;
            material.color = color;
            meshRenderer.sortingOrder = sortingOrder;
            meshRenderer.sortingLayerID = sortingLayerID;
        }
        private void ApplyMaskInteraction()
        {
            if (material == null) return;

            switch (maskInteraction)
            {
                case SpriteMaskInteraction.None:
                    material.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.Disabled);
                    material.SetInt("_Stencil", 0);
                    material.SetInt("_StencilOp", (int)UnityEngine.Rendering.StencilOp.Keep);
                    material.SetInt("_StencilReadMask", 255);
                    material.SetInt("_StencilWriteMask", 255);
                    break;
                case SpriteMaskInteraction.VisibleInsideMask:
                    material.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.Equal);
                    material.SetInt("_Stencil", 1);
                    material.SetInt("_StencilOp", (int)UnityEngine.Rendering.StencilOp.Keep);
                    material.SetInt("_StencilReadMask", 255);
                    material.SetInt("_StencilWriteMask", 255);
                    break;
                case SpriteMaskInteraction.VisibleOutsideMask:
                    material.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.NotEqual);
                    material.SetInt("_Stencil", 1);
                    material.SetInt("_StencilOp", (int)UnityEngine.Rendering.StencilOp.Keep);
                    material.SetInt("_StencilReadMask", 255);
                    material.SetInt("_StencilWriteMask", 255);
                    break;
            }
        }
        private void ApplyFlip()
        {
            if (meshFilter == null || meshFilter.mesh == null) return;

            float x = flipX ? -1f : 1f;
            float y = flipY ? -1f : 1f;

            meshFilter.mesh.vertices = new Vector3[4]
            {
                new Vector3(-0.5f * x, -0.5f * y, 0),
                new Vector3( 0.5f * x, -0.5f * y, 0),
                new Vector3( 0.5f * x,  0.5f * y, 0),
                new Vector3(-0.5f * x,  0.5f * y, 0)
            };
            meshFilter.mesh.RecalculateNormals();
            meshFilter.mesh.RecalculateBounds();
        }
        /// <summary>
        /// Play the animation from where it left off
        /// </summary>
        public void Play(float speed = 1)
        {
            Speed = speed;
            Stoped = false;
        }
        /// <summary>
        /// Pause the animation
        /// </summary>
        public void Pause()
        {
            Speed = 0;
        }
        /// <summary>
        /// Pause and reset the animation
        /// </summary>
        public void Stop()
        {
            Stoped = true;
            timer = 0;
        }
        public void Update()
        {
            if (GIF != null && !Stoped)
            {
                timer += Time.deltaTime * Speed;
                material.mainTexture = GIF.GetFrame(timer);
                if (GIF.maxTime <= timer)
                {
                    timer = 0;
                    if (!GIF.Loop)
                    {
                        Stop();
                    }
                }
            }
        }
    }
}