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
    /// A component that animates SpriteRenderers using a given GifFile
    /// </summary>
    [RegisterTypeInIl2Cpp]
    public class GifAnimator : MonoBehaviour
    {
        public Gif Gif;
        public float Speed = 1;
        public bool Stoped;

        private Material material;
        private MeshRenderer meshRenderer;
        private float timer;
        public Material Material
        {
            get
            {
                return material;
            }
            set
            {
                if (material == value) return;
                material?.Destroy();
                meshRenderer.material = value;
                material = meshRenderer.material;
            }
        }

        public void Awake()
        {
            meshRenderer = gameObject.GetOrAddComponent<MeshRenderer>(); 

            MeshFilter meshFilter = GetComponent<MeshFilter>();
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
            if (Gif != null && !Stoped)
            {
                timer += Time.deltaTime * Speed;
                material.mainTexture = Gif.GetFrame(timer);
                if (Gif.maxTime <= timer)
                {
                    timer = 0;
                    if (!Gif.Loop)
                    {
                        Stop();
                    }
                }
            }
        }
    }
}
