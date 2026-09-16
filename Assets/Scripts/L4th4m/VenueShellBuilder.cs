using UnityEngine;

namespace L4th4m
{
    /// <summary>
    /// Builds recognisable venue architecture around the existing 40 x 40 gameplay grid.
    /// The inner playfield remains untouched so the current game rules can stay stable.
    /// </summary>
    public class VenueShellBuilder : MonoBehaviour
    {
        public enum Venue
        {
            Wembley,
            Colosseum,
            MadisonSquareGarden
        }

        [SerializeField] private Venue venue = Venue.Wembley;
        [SerializeField] private float gridSize = 40f;
        [SerializeField] private Material primaryMaterial;
        [SerializeField] private Material secondaryMaterial;
        [SerializeField] private Material emissiveMaterial;
        [SerializeField] private bool buildOnStart = true;

        private Transform generatedRoot;

        private void Start()
        {
            if (buildOnStart)
                Rebuild();
        }

        [ContextMenu("Rebuild Venue")]
        public void Rebuild()
        {
            ClearGenerated();
            generatedRoot = new GameObject("Generated Venue Shell").transform;
            generatedRoot.SetParent(transform, false);

            switch (venue)
            {
                case Venue.Wembley:
                    BuildWembley();
                    break;
                case Venue.Colosseum:
                    BuildColosseum();
                    break;
                case Venue.MadisonSquareGarden:
                    BuildMadisonSquareGarden();
                    break;
            }
        }

        [ContextMenu("Clear Generated Venue")]
        public void ClearGenerated()
        {
            Transform existing = transform.Find("Generated Venue Shell");
            if (existing == null)
                return;

            if (Application.isPlaying)
                Destroy(existing.gameObject);
            else
                DestroyImmediate(existing.gameObject);
        }

        private void BuildWembley()
        {
            BuildTieredBowl(4, 4.5f, 2.3f, 2.5f);

            // Player tunnel / arena entrance.
            CreateBlock("Wembley Tunnel Left", new Vector3(-6f, 2.5f, -(gridSize * 0.5f + 7f)), new Vector3(4f, 5f, 10f), primaryMaterial);
            CreateBlock("Wembley Tunnel Right", new Vector3(6f, 2.5f, -(gridSize * 0.5f + 7f)), new Vector3(4f, 5f, 10f), primaryMaterial);
            CreateBlock("Wembley Tunnel Roof", new Vector3(0f, 5.5f, -(gridSize * 0.5f + 7f)), new Vector3(16f, 1.5f, 10f), primaryMaterial);

            // Large end scoreboards.
            CreateBlock("Wembley Scoreboard North", new Vector3(0f, 12f, gridSize * 0.5f + 14f), new Vector3(16f, 7f, 1f), emissiveMaterial);
            CreateBlock("Wembley Scoreboard South", new Vector3(0f, 12f, -(gridSize * 0.5f + 14f)), new Vector3(16f, 7f, 1f), emissiveMaterial);

            // Four floodlight towers.
            float p = gridSize * 0.5f + 18f;
            CreateFloodlightTower(new Vector3(-p, 0f, -p));
            CreateFloodlightTower(new Vector3(p, 0f, -p));
            CreateFloodlightTower(new Vector3(-p, 0f, p));
            CreateFloodlightTower(new Vector3(p, 0f, p));

            // Simplified Wembley arch silhouette above the stadium.
            BuildArch("Wembley Arch", 34f, 26f, 16, 1.2f, new Vector3(0f, 0f, 2f));
        }

        private void BuildColosseum()
        {
            BuildTieredBowl(3, 5f, 2.1f, 2.8f);

            float radius = gridSize * 0.5f + 11f;
            const int columns = 28;
            for (int i = 0; i < columns; i++)
            {
                float a = i * Mathf.PI * 2f / columns;
                Vector3 pos = new Vector3(Mathf.Cos(a) * radius, 3.5f, Mathf.Sin(a) * radius);
                Transform column = CreateCylinder("Colosseum Column", pos, new Vector3(1.5f, 3.5f, 1.5f), primaryMaterial);
                column.rotation = Quaternion.Euler(0f, -a * Mathf.Rad2Deg, 0f);
            }

            // Broken upper-ring masses give the silhouette depth without occupying the grid.
            for (int i = 0; i < 12; i++)
            {
                if (i == 2 || i == 3 || i == 8)
                    continue;

                float a = i * Mathf.PI * 2f / 12f;
                Vector3 pos = new Vector3(Mathf.Cos(a) * (radius + 2.5f), 10f, Mathf.Sin(a) * (radius + 2.5f));
                CreateBlock("Colosseum Upper Ring", pos, new Vector3(8f, 5f, 3f), secondaryMaterial).rotation = Quaternion.Euler(0f, 90f - a * Mathf.Rad2Deg, 0f);
            }
        }

        private void BuildMadisonSquareGarden()
        {
            BuildTieredBowl(5, 3.5f, 2.4f, 2f);

            float edge = gridSize * 0.5f + 12f;
            CreateBlock("MSG North Wall", new Vector3(0f, 10f, edge), new Vector3(58f, 20f, 2f), primaryMaterial);
            CreateBlock("MSG South Wall", new Vector3(0f, 10f, -edge), new Vector3(58f, 20f, 2f), primaryMaterial);
            CreateBlock("MSG East Wall", new Vector3(edge, 10f, 0f), new Vector3(2f, 20f, 58f), primaryMaterial);
            CreateBlock("MSG West Wall", new Vector3(-edge, 10f, 0f), new Vector3(2f, 20f, 58f), primaryMaterial);

            // Overhead truss.
            CreateBlock("MSG Truss North", new Vector3(0f, 18f, 13f), new Vector3(34f, 0.7f, 0.7f), secondaryMaterial);
            CreateBlock("MSG Truss South", new Vector3(0f, 18f, -13f), new Vector3(34f, 0.7f, 0.7f), secondaryMaterial);
            CreateBlock("MSG Truss East", new Vector3(13f, 18f, 0f), new Vector3(0.7f, 0.7f, 34f), secondaryMaterial);
            CreateBlock("MSG Truss West", new Vector3(-13f, 18f, 0f), new Vector3(0.7f, 0.7f, 34f), secondaryMaterial);

            // Centre-hung display cube.
            CreateBlock("MSG Centre Display North", new Vector3(0f, 14f, 3.2f), new Vector3(10f, 5f, 0.8f), emissiveMaterial);
            CreateBlock("MSG Centre Display South", new Vector3(0f, 14f, -3.2f), new Vector3(10f, 5f, 0.8f), emissiveMaterial);
            CreateBlock("MSG Centre Display East", new Vector3(3.2f, 14f, 0f), new Vector3(0.8f, 5f, 10f), emissiveMaterial);
            CreateBlock("MSG Centre Display West", new Vector3(-3.2f, 14f, 0f), new Vector3(0.8f, 5f, 10f), emissiveMaterial);
        }

        private void BuildTieredBowl(int tiers, float tierDepth, float tierHeight, float gapFromGrid)
        {
            float half = gridSize * 0.5f;
            for (int tier = 0; tier < tiers; tier++)
            {
                float offset = half + gapFromGrid + tier * tierDepth;
                float y = 0.6f + tier * tierHeight;
                float run = gridSize + (gapFromGrid + tier * tierDepth) * 2f;

                CreateBlock("Stand North T" + tier, new Vector3(0f, y, offset), new Vector3(run, tierHeight, tierDepth), secondaryMaterial);
                CreateBlock("Stand South T" + tier, new Vector3(0f, y, -offset), new Vector3(run, tierHeight, tierDepth), secondaryMaterial);
                CreateBlock("Stand East T" + tier, new Vector3(offset, y, 0f), new Vector3(tierDepth, tierHeight, run), secondaryMaterial);
                CreateBlock("Stand West T" + tier, new Vector3(-offset, y, 0f), new Vector3(tierDepth, tierHeight, run), secondaryMaterial);
            }
        }

        private void CreateFloodlightTower(Vector3 basePosition)
        {
            CreateBlock("Floodlight Mast", basePosition + Vector3.up * 9f, new Vector3(0.8f, 18f, 0.8f), primaryMaterial);
            CreateBlock("Floodlight Bank", basePosition + Vector3.up * 18f, new Vector3(7f, 3f, 1f), emissiveMaterial);
        }

        private void BuildArch(string objectName, float width, float height, int segments, float thickness, Vector3 offset)
        {
            float radiusX = width * 0.5f;
            for (int i = 0; i < segments; i++)
            {
                float t0 = i / (float)segments;
                float t1 = (i + 1) / (float)segments;
                float a0 = Mathf.PI * t0;
                float a1 = Mathf.PI * t1;

                Vector3 p0 = new Vector3(Mathf.Cos(a0) * radiusX, Mathf.Sin(a0) * height, 0f) + offset;
                Vector3 p1 = new Vector3(Mathf.Cos(a1) * radiusX, Mathf.Sin(a1) * height, 0f) + offset;
                Vector3 centre = (p0 + p1) * 0.5f;
                Vector3 direction = p1 - p0;

                Transform section = CreateBlock(objectName + " " + i, centre, new Vector3(thickness, thickness, direction.magnitude), emissiveMaterial);
                section.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }
        }

        private Transform CreateBlock(string objectName, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = objectName;
            go.transform.SetParent(generatedRoot, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;
            ApplyMaterial(go, material);
            return go.transform;
        }

        private Transform CreateCylinder(string objectName, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = objectName;
            go.transform.SetParent(generatedRoot, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;
            ApplyMaterial(go, material);
            return go.transform;
        }

        private static void ApplyMaterial(GameObject go, Material material)
        {
            if (material == null)
                return;

            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = material;
        }
    }
}
