
using UnityEngine;

public class SceneGenerator : MonoBehaviour
{
    [Range(3,10)] public int pyramidSize = 5; //Controls Size of Pyramid
    [Range(10,200)] public int treeCount = 50; // How many trees there'll be
    [Range(5f,50f)] public float forestRadius = 30f; //How far the forest goes
    public float orbitSpeed = 10f, orbitRadius = 80f; // Sun distance

    GameObject sun;
    Light sunLight;

    void Start()
    {
        CreateGround();
        CreatePyramid();
        CreateForest();
        CreateSun();
    }

    void Update() // Each frame has the sun orbit around based on y position
    { //The following function calculates how the sun will rotate around world, the color and how bright itll 
        sun.transform.RotateAround(Vector3.zero, Vector3.right, orbitSpeed * Time.deltaTime);
        bool day = sun.transform.position.y > 0;
        sunLight.color = day ? new Color(1f, 0.95f, 0.8f) : new Color(0.6f, 0.7f, 1f);
        sunLight.intensity = day ? 2f : 0.4f;
        RenderSettings.ambientLight = day ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.05f, 0.05f, 0.1f);
        sun.GetComponent<Renderer>().material.color = day ? Color.yellow : Color.white;
    }

    GameObject Make(PrimitiveType t, string n, Vector3 pos, Vector3 scale, Color col, Transform parent = null)
    // Creates a primitive , parents it and returns it
    {
        var g = GameObject.CreatePrimitive(t);
        g.name = n; g.transform.position = pos; g.transform.localScale = scale;
        if (parent) g.transform.parent = parent;
        var m = g.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
        m.color = col;
        return g;
    }

    void CreateGround()
    // Flat plane for the ground
    {
        Make(PrimitiveType.Plane, "Ground", Vector3.zero, new Vector3(20,1,20), new Color(0.35f,0.55f,0.2f), new GameObject("Ground").transform);
    }

    void CreatePyramid()
    // Builds the pyramid level by level using loops
    {
        var root = new GameObject("Pyramid").transform;
        for (int lvl = 0; lvl < pyramidSize; lvl++)
        {
            var levelRoot = new GameObject("Level_" + lvl);
            levelRoot.transform.parent = root;
            int s = pyramidSize - lvl;
            float offset = (pyramidSize - s) / 2f - pyramidSize / 2f;
            Color c = Color.HSVToRGB((float)lvl / pyramidSize, 0.7f, 0.9f);
            for (int x = 0; x < s; x++)
                for (int z = 0; z < s; z++)
                    Make(PrimitiveType.Cube, "Block", new Vector3(x + offset, lvl + 0.5f, z + offset), Vector3.one, c, levelRoot.transform);
        }
    }

    void CreateForest()
    // Places trees across the place, avoids the pyramids 
    {
        var root = new GameObject("Forest").transform;
        for (int i = 0; i < treeCount; i++)
        {
            var p = Random.insideUnitCircle * forestRadius;
            var pos = new Vector3(p.x, 0, p.y);
            if (Vector3.Distance(pos, Vector3.zero) < pyramidSize) continue;
            var tree = new GameObject("Tree"); tree.transform.parent = root; tree.transform.position = pos;
            float h = Random.Range(1.5f, 3f);
            Make(PrimitiveType.Cylinder, "Trunk", pos + Vector3.up * h / 2f, new Vector3(0.3f, h / 2f, 0.3f), new Color(0.45f, 0.28f, 0.1f), tree.transform);
            Color fc = new Color(Random.Range(0.05f,0.2f), Random.Range(0.4f,0.7f), Random.Range(0.05f,0.2f));
            for (int f = 0; f < Random.Range(2,4); f++)
                Make(PrimitiveType.Sphere, "Foliage", pos + Vector3.up * (h + f * 0.7f), Vector3.one * Mathf.Lerp(1.8f, 0.8f, (float)f / 3), fc, tree.transform);
        }
    }

    void CreateSun()
    // This creates a orbiting sun sphere with an attached point light
    {
        var root = new GameObject("Celestial").transform;
        sun = Make(PrimitiveType.Sphere, "Sun", new Vector3(0, orbitRadius, 0), Vector3.one * 8f, Color.yellow, root);
        sunLight = new GameObject("SunLight").AddComponent<Light>();
        sunLight.transform.parent = sun.transform; sunLight.transform.localPosition = Vector3.zero;
        sunLight.type = LightType.Point; sunLight.range = orbitRadius * 3f; sunLight.intensity = 2f;
    }
}
