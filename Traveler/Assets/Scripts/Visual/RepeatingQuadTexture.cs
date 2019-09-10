using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class RepeatingQuadTexture : MonoBehaviour
{
    public Sprite RepeatingSprite;
    public Texture NormalTexture;
    public Vector2 SizeOfRepeatTexture = new Vector2(1f, 1f);
    private Vector2Int SizeOfOriginal = new Vector2Int(128, 128);

    private Sprite m_repeatingSprite;
    private Vector2 m_repeatTextureSize;
    private Vector2Int m_originalSize;
    private Vector3 m_oldScale;
    private Renderer rend;

    private Material m_material;
    private 
    // Start is called before the first frame update
    void Start()
    {
        m_originalSize = SizeOfOriginal;
        m_repeatTextureSize = SizeOfRepeatTexture;
        m_repeatingSprite = RepeatingSprite;
        m_oldScale = transform.lossyScale;
        redrawQuad();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_repeatingSprite != RepeatingSprite ||
            m_originalSize != SizeOfOriginal ||
            m_repeatTextureSize != SizeOfRepeatTexture ||
            m_oldScale != transform.lossyScale)
        {
            redrawQuad();
        }
    }


    private void redrawQuad()
    {
        if (m_material != null)
            DestroyImmediate(m_material);

        if (rend == null)
        {
            rend = GetComponent<Renderer>() as Renderer;
        }
        
        m_material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m_material.SetTexture("_BaseMap",RepeatingSprite.texture);
        m_material.SetColor("_BaseColor", Color.white);
        m_material.SetVector("_BaseMap_ST", new Vector4(transform.lossyScale.x/ SizeOfRepeatTexture.x, transform.lossyScale.y / SizeOfRepeatTexture.y, 0f, 0f));

        m_material.SetTexture("_BumpMap", NormalTexture);
        rend.materials = new Material[]{ m_material };
        m_originalSize = SizeOfOriginal;
        m_repeatTextureSize = SizeOfRepeatTexture;
        m_repeatingSprite = RepeatingSprite;
        m_oldScale = transform.lossyScale;

    }
}
