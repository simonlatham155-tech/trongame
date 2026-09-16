using UnityEngine;

namespace L4th4m
{
    /// <summary>
    /// Wembley-specific presentation shell. Keeps the 40x40 gameplay grid clear while
    /// creating a recognisable stadium bowl, player tunnel, arch, lighting and screens.
    /// Generated geometry is intentionally outside the competitive playfield.
    /// </summary>
    public class WembleyArenaBuilder : MonoBehaviour
    {
        [Header("Gameplay footprint")]
        [SerializeField] private float gridSize = 40f;
        [SerializeField] private float safetyGap = 3f;

        [Header("Stadium")]
        [SerializeField] private int standTiers = 5;
        [SerializeField] private float tierDepth = 4.2f;
        [SerializeField] private float tierRise = 2.2f;
        [SerializeField] private float cornerOpening = 7f;

        [Header("Wembley arch")]
        [SerializeField] private float archWidth = 72f;
        [SerializeField] private float archHeight = 38f;
        [SerializeField] private float archThickness = 1.25f;
        [SerializeField] private int archSegments = 30;

        [Header("Materials")]
        [SerializeField] private Material structureMaterial;
        [SerializeField] private Material standMaterial;
        [SerializeField] private Material emissiveMaterial;
        [SerializeField] private Material tunnelMaterial;

        [Header("Build")]
        [SerializeField] private bool buildOnStart = true;

        private Transform generatedRoot;

        private void Start()
        {
            if (buildOnStart)
                Rebuild();
        }

        [ContextMenu("Rebuild Wembley")]
        public void Rebuild()
        {
            ClearGenerated();
            generatedRoot = new GameObject("Generated Wembley").transform;
            generatedRoot.SetParent(transform, false);

            BuildSeatingBowl();
            BuildPlayerTunnel();
            BuildArch();
            BuildFloodlights();
            BuildScoreboards();
            BuildUpperRim();
        }

        [ContextMenu("Clear Wembley")]
        public void ClearGenerated()
        {
            Transform old = transform.Find("Generated Wembley");
            if (old == null) return;

            if (Application.isPlaying) Destroy(old.gameObject);
            else DestroyImmediate(old.gameObject);
        }

        private void BuildSeatingBowl()
        {
            float half = gridSize * 0.5f;

            for (int tier = 0; tier < standTiers; tier++)
            {
                float d = half + safetyGap + tier * tierDepth + tierDepth * 0.5f;
                float y = 0.7f + tier * tierRise;
                float span = gridSize + 2f * (safetyGap + tier * tierDepth + tierDepth);

                // Split each side into sections. This creates entrances/corner gaps instead
                // of four continuous walls around the player.
                float sideSection = (span - cornerOpening * 2f) * 0.5f;
                float sectionOffset = cornerOpening * 0.5f + sideSection * 0.5f;

                CreateBlock($"North Stand L T{tier}", new Vector3(-sectionOffset, y, d), new Vector3(sideSection, tierRise, tierDepth), standMaterial);
                CreateBlock($"North Stand R T{tier}", new Vector3(sectionOffset, y, d), new Vector3(sideSection, tierRise, tierDepth), standMaterial);
                CreateBlock($"South Stand L T{tier}", new Vector3(-sectionOffset, y, -d), new Vector3(sideSection, tierRise, tierDepth), standMaterial);
                CreateBlock($"South Stand R T{tier}", new Vector3(sectionOffset, y, -d), new Vector3(sideSection, tierRise, tierDepth), standMaterial);

                CreateBlock($"East Stand N T{tier}", new Vector3(d, y, sectionOffset), new Vector3(tierDepth, tierRise, sideSection), standMaterial);
                CreateBlock($"East Stand S T{tier}", new Vector3(d, y, -sectionOffset), new Vector3(tierDepth, tierRise, sideSection), standMaterial);
                CreateBlock($"West Stand N T{tier}", new Vector3(-d, y, sectionOffset), new Vector3(tierDepth, tierRise, sideSection), standMaterial);
                CreateBlock($"West Stand S T{tier}", new Vector3(-d, y, -sectionOffset), new Vector3(tierDepth, tierRise, sideSection), standMaterial);
            }
        }

        private void BuildPlayerTunnel()
        {
            float z = -(gridSize * 0.5f + safetyGap + 6f);
            CreateBlock("Player Tunnel Left", new Vector3(-5f, 2.4f, z), new Vector3(3f, 4.8f, 12f), tunnelMaterial);
            CreateBlock("Player Tunnel Right", new Vector3(5f, 2.4f, z), new Vector3(3f, 4.8f, 12f), tunnelMaterial);
            CreateBlock("Player Tunnel Roof", new Vector3(0f, 5.1f, z), new Vector3(13f, 1.1f, 12f), tunnelMaterial);

            // Emissive strips guide the opening-scene character toward the grid.
            CreateBlock("Tunnel Strip L", new Vector3(-3.15f, 0.12f, z), new Vector3(0.18f, 0.12f, 11f), emissiveMaterial);
            CreateBlock("Tunnel Strip R", new Vector3(3.15f, 0.12f, z), new Vector3(0.18f, 0.12f, 11f), emissiveMaterial);
        }

        private void BuildArch()
        {
            // Arch lies across X/Y with slight rear offset so it frames the arena reveal.
            for (int i = 0; i < archSegments; i++)
            {
                float t0 = i / (float)archSegments;
                float t1 = (i + 1) / (float)archSegments;
                float a0 = Mathf.PI * t0;
                float a1 = Mathf.PI * t1;

                Vector3 p0 = new Vector3(Mathf.Cos(a0) * archWidth * 0.5f, Mathf.Sin(a0) * archHeight + 2f, 8f);
                Vector3 p1 = new Vector3(Mathf.Cos(a1) * archWidth * 0.5f, Mathf.Sin(a1) * archHeight + 2f, 8f);
                Vector3 centre = (p0 + p1) * 0.5f;
                Vector3 direction = p1 - p0;

                Transform segment = CreateBlock($"Wembley Arch {i:00}", centre,
                    new Vector3(archThickness, archThickness, direction.magnitude), emissiveMaterial);
                segment.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }
        }

        private void BuildFloodlights()
        {
            float p = gridSize * 0.5f + safetyGap + standTiers * tierDepth + 3f;
            CreateLightGantries(new Vector3(-p, 0f, -p));
            CreateLightGantries(new Vector3(p, 0f, -p));
            CreateLightGantries(new Vector3(-p, 0f, p));
            CreateLightGantries(new Vector3(p, 0f, p));
        }

        private void CreateLightGantries(Vector3 basePosition)
        {
            CreateBlock("Floodlight Mast", basePosition + Vector3.up * 11f, new Vector3(0.7f, 22f, 0.7f), structureMaterial);
            CreateBlock("Floodlight Bank", basePosition + Vector3.up * 22f, new Vector3(8f, 3.5f, 1f), emissiveMaterial);
        }

        private void BuildScoreboards()
        {
            float z = gridSize * 0.5f + safetyGap + standTiers * tierDepth + 1.5f;
            CreateBlock("North Main Screen", new Vector3(0f, 14f, z), new Vector3(19f, 7.5f, 0.75f), emissiveMaterial);
            CreateBlock("South Main Screen", new Vector3(0f, 14f, -z), new Vector3(19f, 7.5f, 0.75f), emissiveMaterial);
        }

        private void BuildUpperRim()
        {
            float edge = gridSize * 0.5f + safetyGap + standTiers * tierDepth + 1f;
            float y = standTiers * tierRise + 3.2f;
            float span = edge * 2f;

            CreateBlock("Upper Rim North", new Vector3(0f, y, edge), new Vector3(span, 0.6f, 0.8f), structureMaterial);
            CreateBlock("Upper Rim South", new Vector3(0f, y, -edge), new Vector3(span, 0.6f, 0.8f), structureMaterial);
            CreateBlock("Upper Rim East", new Vector3(edge, y, 0f), new Vector3(0.8f, 0.6f, span), structureMaterial);
            CreateBlock("Upper Rim West", new Vector3(-edge, y, 0f), new Vector3(0.8f, 0.6f, span), structureMaterial);
        }

        private Transform CreateBlock(string objectName, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = objectName;
            go.transform.SetParent(generatedRoot, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;

            if (material != null)
            {
                Renderer renderer = go.GetComponent<Renderer>();
                if (renderer != null) renderer.sharedMaterial = material;
            }

            return go.transform;
        }
    }
}
