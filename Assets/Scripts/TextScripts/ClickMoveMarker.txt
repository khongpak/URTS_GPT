using UnityEngine;

namespace URTS_GPT.MovementSystem
{
    public sealed class ClickMoveMarker : MonoBehaviour
    {
        [SerializeField] private Color color = new(1f, 0.85f, 0.2f, 1f);
        [SerializeField] private float scale = 0.25f;
        [SerializeField] private float lifeTime = 1.5f;

        private static ClickMoveMarker prefab;

        public static void Spawn(Vector3 pos)
        {
            if (prefab == null)
            {
                var go = new GameObject("ClickMoveMarker");
                prefab = go.AddComponent<ClickMoveMarker>();
            }
            var instance = Instantiate(prefab, pos, Quaternion.identity);
            instance.Initialize();
        }

        private void Initialize()
        {
            if (GetComponent<MeshRenderer>() == null)
            {
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(transform, false);
                Destroy(sphere.GetComponent<Collider>());
            }

            transform.localScale = Vector3.one * Mathf.Max(0.01f, scale);
            transform.position += Vector3.up * 0.05f;

            var mr = GetComponentInChildren<MeshRenderer>();
            if (mr != null)
            {
                Material mat;
                if (mr.sharedMaterial != null)
                {
                    mat = new Material(mr.sharedMaterial);
                }
                else
                {
                    var shader = Shader.Find("Universal Render Pipeline/Lit");
                    mat = new Material(shader);
                }

                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
                if (mat.HasProperty("_Color"))     mat.SetColor("_Color", color);
                mr.material = mat;
            }

            Destroy(gameObject, Mathf.Clamp(lifeTime, 0.1f, 5f));
        }
    }
}