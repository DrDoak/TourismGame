using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Stairs : MonoBehaviour
{
    public Direction StairDirection;
    public float StairDepth = 0.5f;
    public Vector2 SizeOfSprite = new Vector2(1f, 1f);
    public Sprite SpriteTop;
    public Sprite SpriteFront;
    public Sprite SpriteLeft;
    public Sprite SpriteRight;
    public GameObject StairPiecePrefab;

    List<GameObject> StairPieces;
    private float m_last_refresh = 0f;
    private const float STAIR_REFRESH_COUNT = 2f;
    private Vector3 m_last_scale;
    private Direction m_last_stair_direction;
    private float m_last_stair_depth;
    // Start is called before the first frame update
    void Start()
    {
        m_last_scale = transform.localScale;
        m_last_stair_depth = StairDepth;
    }

    // Update is called once per frame
    void Update()
    {
        if ( StairDepth > 0.05f && ((m_last_stair_direction != StairDirection) ||
            (StairDepth != m_last_stair_depth ||
            m_last_scale != transform.localScale) &&
            Time.timeSinceLevelLoad - m_last_refresh > STAIR_REFRESH_COUNT))
        {
            m_last_refresh = Time.timeSinceLevelLoad;
            m_last_stair_depth = StairDepth;
            m_last_scale = transform.localScale;
            m_last_stair_direction = StairDirection;
            clearStairs();
            ReinitializeStairs();
        }
    }

    private void ReinitializeStairs()
    {
        float finalPos = transform.localScale.x;
        if (StairDirection == Direction.UP || StairDirection == Direction.DOWN){
            finalPos = transform.localScale.z;
        }

        
        float stairHeight = (transform.localScale.y) / (finalPos / StairDepth);
        Vector3 stairLocation = new Vector3(-0.5f + (StairDepth / transform.localScale.x)/2f, -(transform.localScale.y/2f) + stairHeight / 2f,0f);
        Vector3 stairOffset = new Vector3(StairDepth/ transform.localScale.x, stairHeight / 2f, 0f);
        Vector3 stairScale = new Vector3(StairDepth / transform.localScale.x, stairHeight, 1f);
        Vector3 stairScaleOffset = new Vector3(0f, stairHeight, 0f);

        if (StairDirection == Direction.UP || StairDirection == Direction.DOWN)
        {
            stairLocation = new Vector3(0f, 0f, -transform.localScale.x / 2f);
            stairScale = new Vector3(1f, 1f, StairDepth/transform.localScale.z);
        }
        

        for (float startPos = 0; startPos < finalPos; startPos += StairDepth)
        {
            GameObject go = Instantiate(StairPiecePrefab, transform);
            go.transform.localPosition = stairLocation;
            stairLocation += stairOffset;
            go.transform.localScale = stairScale;
            stairScale += stairScaleOffset;
            BoxMaterialObj boxMatObj = go.GetComponent<BoxMaterialObj>();
            boxMatObj.SpriteTop = SpriteTop;
            boxMatObj.SizeOfTopSprite = SizeOfSprite;
            boxMatObj.SpriteFront = SpriteFront;
            boxMatObj.SizeOfFrontSprite = SizeOfSprite;
            boxMatObj.SpriteLeft = SpriteLeft;
            boxMatObj.SizeOfLeftSprite = SizeOfSprite;
            boxMatObj.SpriteRight = SpriteRight;
            boxMatObj.SizeOfRightSprite = SizeOfSprite;
        }
    }

    private void clearStairs()
    {
        int cleared = 0;
        int OldChildCount = transform.childCount;
        int timesClear = 0;
        while (timesClear < 5 && transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                cleared++;
                GameObject.DestroyImmediate(child.gameObject);
            }
            timesClear++;
        }
    }
}
