using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridGenerator : MonoBehaviour
{
    int Size = 64 * 2;
    public int cellSize = 1; 

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;

    private void Awake()
    {
        GenerateMesh();

        SetupHeightMap();
    }

    private void GenerateMesh()
    {
        
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Procedural Terrain Grid"; 

        
        int vertexCount = (Size + 1) * (Size + 1);
        vertices = new Vector3[vertexCount];
        uv = new Vector2[vertexCount];

        for (int i = 0, y = 0; y <= Size; y++)
        {
            for (int x = 0; x <= Size; x++, i++)
            {
                vertices[i] = new Vector3((x * cellSize) - ((Size/2) * cellSize), 0, (y * cellSize) - ((Size / 2) * cellSize));
                uv[i] = new Vector2((float)x / Size, (float)y / Size); 
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;

        
        triangles = new int[Size * Size * 6];
        int triIndex = 0;
        int vertIndex = 0;

        for (int y = 0; y < Size; y++, vertIndex++)
        {
            for (int x = 0; x < Size; x++, triIndex += 6, vertIndex++)
            {
                
                triangles[triIndex] = vertIndex;
                triangles[triIndex + 1] = vertIndex + Size + 1;
                triangles[triIndex + 2] = vertIndex + 1;

                
                triangles[triIndex + 3] = vertIndex + 1;
                triangles[triIndex + 4] = vertIndex + Size + 1;
                triangles[triIndex + 5] = vertIndex + Size + 2;
            }
        }
        mesh.triangles = triangles;

        
        mesh.RecalculateNormals();
       

        
        gameObject.AddComponent<MeshCollider>();
    }


    void SetupHeightMap ()
    {

        

        float[,] map = GenerateDiamondSquareHeightmap(Size + 1,  0f, 20f, 20f);

        Debug.Log(map.Length);

        Debug.Log(mesh.vertices.Length);

        ApplyHeightmap(map);

        ApplyTexture(map);


    }




    
    public void ApplyHeightmap(float[,] heightmap)
    {
        if (heightmap.GetLength(0) != Size + 1 || heightmap.GetLength(1) != Size + 1)
        {
            Debug.LogError("Heightmap size doesn't match grid, you naughty thing!");
            return;
        }

        for (int i = 0, y = 0; y <= Size; y++)
        {
            for (int x = 0; x <= Size; x++, i++)
            {
                vertices[i].y = heightmap[x, y]; 
            }
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals(); 
        MeshCollider mc = GetComponent<MeshCollider>();
        if (mc != null)
        {
            mc.sharedMesh = null; 
            mc.sharedMesh = mesh; 
        }
    }



    //HEIGHT MAP FUNCTION DIAMOND AND SQUARE


    public float[,] GenerateDiamondSquareHeightmap(int size, float minHeight = 0f, float maxHeight = 10f, float roughness = 0.8f, int? seed = null)
    {
        if ((size - 1) % 2 != 0 || size < 3)
            throw new ArgumentException("Size must be 2^n + 1 and at least 3");

        float[,] heightmap = new float[size, size];
        System.Random rand = seed.HasValue ? new System.Random(seed.Value) : new System.Random();

        
        heightmap[0, 0] = RandomHeight(rand, minHeight, maxHeight);
        heightmap[0, size - 1] = RandomHeight(rand, minHeight, maxHeight);
        heightmap[size - 1, 0] = RandomHeight(rand, minHeight, maxHeight);
        heightmap[size - 1, size - 1] = RandomHeight(rand, minHeight, maxHeight);

        
        for (int step = size - 1; step > 1; step /= 2)
        {
            float currentRoughness = roughness * step / (size - 1); 

            //diamond step
            for (int x = step / 2; x < size; x += step)
            {
                for (int y = step / 2; y < size; y += step)
                {
                    float avg = (heightmap[x - step / 2, y - step / 2] +  // top left
                                 heightmap[x + step / 2, y - step / 2] +  //  top right
                                 heightmap[x - step / 2, y + step / 2] +  // bottom left
                                 heightmap[x + step / 2, y + step / 2]) / 4; // nottom right
                    heightmap[x, y] = avg + RandomOffset(rand, -currentRoughness, currentRoughness);
                }
            }

            
            for (int x = 0; x < size; x += step / 2)
            {
                for (int y = (x + step / 2) % step; y < size; y += step)
                {
                    int count = 0;
                    float sum = 0f;

                    
                    if (x >= step / 2) { sum += heightmap[x - step / 2, y]; count++; } 
                    if (x + step / 2 < size) { sum += heightmap[x + step / 2, y]; count++; } 
                    if (y >= step / 2) { sum += heightmap[x, y - step / 2]; count++; } 
                    if (y + step / 2 < size) { sum += heightmap[x, y + step / 2]; count++; } 

                    float avg = sum / count;
                    heightmap[x, y] = avg + RandomOffset(rand, -currentRoughness, currentRoughness);
                }
            }
        }

        return heightmap;
    }

    private float RandomHeight(System.Random rand, float min, float max)
    {
        return (float)rand.NextDouble() * (max - min) + min; 
    }

    private float RandomOffset(System.Random rand, float min, float max)
    {
        return (float)rand.NextDouble() * (max - min) + min; 
    }


    public Texture2D GenerateHeightmapTexture(float[,] heightmap, float minHeight = 0f, float maxHeight = 20f)
    {
        int width = heightmap.GetLength(0);
        int height = heightmap.GetLength(1);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false); 
        Color[] colors = new Color[width * height];

        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float h = Mathf.Clamp(heightmap[x, y], minHeight, maxHeight); 
                float t = Mathf.InverseLerp(minHeight, maxHeight, h); 

                Color color;
                if (t < 0.3f) 
                    color = Color.Lerp(Color.black, Color.darkGray, t / 0.3f);
                else if (t < 0.6f) 
                    color = Color.Lerp(Color.darkGray, Color.green, (t - 0.3f) / 0.3f);
                else 
                    color = Color.Lerp(Color.green, Color.red, (t - 0.6f) / 0.4f);

                colors[y * width + x] = color; 
            }
        }

        texture.SetPixels(colors);
        texture.Apply(); 
        return texture;
    }

    public Texture2D GenerateSlopeBlendedTexture(float[,] heightmap, float minHeight = 0f, float maxHeight = 20f)
    {
        int width = heightmap.GetLength(0);
        int height = heightmap.GetLength(1);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false); 
        Color[] colors = new Color[width * height];

        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float h = Mathf.Clamp(heightmap[x, y], minHeight, maxHeight); 
                float slope = 0f;

                
                if (x > 0 && x < width - 1)
                {
                    float dx = Mathf.Abs(heightmap[x + 1, y] - heightmap[x - 1, y]) / 2f; 
                    slope = Mathf.Max(slope, dx);
                }
                if (y > 0 && y < height - 1)
                {
                    float dy = Mathf.Abs(heightmap[x, y + 1] - heightmap[x, y - 1]) / 2f; 
                    slope = Mathf.Max(slope, dy);
                }

                
                float slopeFactor = Mathf.Clamp01(slope / (maxHeight - minHeight) * 2f);

                
                Color color;

                color = Color.Lerp(Color.darkGreen, Color.brown, slopeFactor / 0.1f);

                colors[y * width + x] = color; 
            }
        }

        texture.SetPixels(colors); 
        texture.Apply(); 
        return texture;
    }

    void ApplyTexture(float[,] height_map)
    {
        Material terrain_material = GetComponent<MeshRenderer>().material;

        terrain_material.mainTexture = GenerateSlopeBlendedTexture(height_map, 0.0f, 20.0f);

    }


}