// This code is related to an answer I provided in the Unity forums at:
// http://forum.unity3d.com/threads/circular-fade-in-out-shader.344816/

using UnityEngine;

namespace Utils.Screen_Wipe.Scripts {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Screen Transition")]
    public class ScreenTransitionImageEffect : MonoBehaviour
    {
        /// Provides a shader property that is set in the inspector
        /// and a material instantiated from the shader
        public Shader shader;

        [Range(0,1.0f)]
        public float maskValue;
        public Color maskColor = Color.black;
        public Texture2D maskTexture;
        public bool maskInvert;

        private Material mMaterial;
        private bool m_maskInvert;

        Material Material
        {
            get
            {
                if (mMaterial == null)
                {
                    mMaterial = new Material(shader);
                    mMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return mMaterial;
            }
        }

        void Start()
        {
            enabled = false;
        }

        void OnDisable()
        {
            if (mMaterial)
            {
                DestroyImmediate(mMaterial);
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!enabled)
            {
                Graphics.Blit(source, destination);
                return;
            }

            Material.SetColor("_MaskColor", maskColor);
            Material.SetFloat("_MaskValue", maskValue);
            Material.SetTexture("_MainTex", source);
            Material.SetTexture("_MaskTex", maskTexture);

            if (Material.IsKeywordEnabled("INVERT_MASK") != maskInvert)
            {
                if (maskInvert)
                    Material.EnableKeyword("INVERT_MASK");
                else
                    Material.DisableKeyword("INVERT_MASK");
            }

            Graphics.Blit(source, destination, Material);
        }
    }
}
