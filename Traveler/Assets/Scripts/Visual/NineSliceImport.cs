using UnityEngine;

// Nine slice scaling using a Mesh.
// Original code by Asher Vollmer
//      https://twitter.com/AsherVo
//      http://ashervollmer.tumblr.com
// Modifications by Thomas Viktil
//      https://twitter.com/mandarinx
//      http://ma.ndar.in/

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class NineSliceImport : MonoBehaviour
{

    public Sprite useSprite;
    public float width;
    public float height;
    public int slicePixels = 5;

    private float oldWidth;
    private float oldHeight;
    private Sprite oldSprite;
    private MeshFilter meshF;
    private Renderer rend;
    private Mesh mesh;
    private Vector3 position;

    public void LateUpdate()
    {
        if (useSprite == oldSprite &&
            width == oldWidth &&
            height == oldHeight)
        {
            return;
        }

        CreateMesh();

        oldSprite = useSprite;
        oldWidth = width;
        oldHeight = height;
    }

    public void CreateMesh()
    {
        if (meshF == null)
        {
            meshF = GetComponent<MeshFilter>() as MeshFilter;
        }
        if (rend == null)
        {
            rend = GetComponent<Renderer>() as Renderer;
        }
        if (meshF.sharedMesh)
        {
            DestroyImmediate(meshF.sharedMesh);
        }
        if (mesh)
        {
            DestroyImmediate(mesh);
        }

        if (useSprite == null)
        {
            return;
        }

        MaterialPropertyBlock pBlock = new MaterialPropertyBlock();
        pBlock.SetTexture("_MainTex", useSprite.texture);
        rend.SetPropertyBlock(pBlock);

        float ppu = useSprite.pixelsPerUnit;
        float cornerWidth = (float)(slicePixels) / ppu;
        float cornerHeight = (float)(slicePixels) / ppu;

        mesh = new Mesh();

        Vector3[] vertices = new Vector3[16];
        Vector3[] normals = new Vector3[16];
        Vector2[] uvs = new Vector2[16];

        float[] xPValues = new float[4];
        float[] yPValues = new float[4];

        float cornerWidthP = cornerWidth / width;
        float cornerHeightP = cornerHeight / height;

        xPValues[0] = 0.0f;
        yPValues[0] = 0.0f;
        xPValues[1] = cornerWidthP;
        yPValues[1] = cornerHeightP;
        xPValues[2] = 1.0f - cornerWidthP;
        yPValues[2] = 1.0f - cornerHeightP;
        xPValues[3] = 1.0f;
        yPValues[3] = 1.0f;

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                float xP = xPValues[x];
                float yP = yPValues[y];

                int index = OneDee(x, y);
                vertices[index] = new Vector3(
                    Mathf.Lerp(-1.0f, 1.0f, xP) * width * 0.5f,
                    Mathf.Lerp(-1.0f, 1.0f, yP) * height * 0.5f,
                    0.0f);
                uvs[index] = new Vector2((float)x / 3.0f, (float)y / 3.0f);
                normals[index] = -Vector3.forward;
            }
        }

        int[] tris = new int[9 * 2 * 3];
        int i = 0;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                tris[i] = OneDee(x + 1, y);
                i++;
                tris[i] = OneDee(x, y);
                i++;
                tris[i] = OneDee(x + 1, y + 1);
                i++;
                tris[i] = OneDee(x, y);
                i++;
                tris[i] = OneDee(x, y + 1);
                i++;
                tris[i] = OneDee(x + 1, y + 1);
                i++;
            }
        }

        mesh.MarkDynamic();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.RecalculateBounds();

        meshF.sharedMesh = mesh;
    }

    public static int OneDee(Vector2 coords)
    {
        return OneDee((int)coords.x, (int)coords.y);
    }

    public static int OneDee(int x, int y)
    {
        return (y * 4) + x;
    }

    public static Vector2 TwoDee(int index)
    {
        int x = index % 4;
        int y = index / 4;
        return new Vector2(x, y);
    }

    public void SetPosition(float x, float y, float z)
    {
        position.x = x;
        position.y = y;
        position.z = z;
        transform.position = position;
    }

    public void SetSize(float w, float h)
    {
        width = w;
        height = h;
    }
}