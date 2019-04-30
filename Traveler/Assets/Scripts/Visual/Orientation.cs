using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { UP, RIGHT, DOWN, LEFT }

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class Orientation : MonoBehaviour
{

    // Tracking m_sprite orientation (flipping if left)...
    private SpriteRenderer m_sprite;
    public Direction CurrentDirection = Direction.DOWN;
    // Use this for initialization
    internal void Awake()
    {
        m_sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetDirection(Direction d)
    {
        CurrentDirection = d;
        if (m_sprite != null && m_sprite.sprite)
        {
            if (d == Direction.LEFT)
            {
                m_sprite.flipX = true;
            }
            else
            {
                m_sprite.flipX = false;
            }
        }
        if (GetComponent<AnimatorSprite>() != null)
        {
            GetComponent<AnimatorSprite>().SetDirection(d);
        }
    }

    public Direction VectorToDirection(Vector3 v)
    {
        if (Mathf.Abs(v.z) >= Mathf.Abs(v.x))
        {
            if (v.z > 0)
            {
                return Direction.UP;
            }
            else
            {
                return Direction.DOWN;
            }

            
        } else
        {
            if (v.x > 0)
            {
                return Direction.RIGHT;
            }
            else
            {
                return Direction.LEFT;
            }
        }
        
    }

    public Vector3 OrientVectorToDirection(Vector3 v,bool negativesAllowed = true)
    {
        return OrientVectorToDirection(CurrentDirection, v, negativesAllowed);
    }

    public Vector3 OrientVectorToDirection(Direction d, Vector3 v, bool negativesAllowed = true)
    {
        Vector3 newV = new Vector3(v.x, v.y, v.z);
        if (d == Direction.UP)
        {
            newV.x = -v.z;
            newV.z = v.x;
        }
        else if (d == Direction.LEFT)
        {
            newV.x = -v.x;
            newV.z = -v.z;
        }
        else if (d == Direction.DOWN)
        {
            newV.x = v.z;
            newV.z = -v.x;
        }
        if (!negativesAllowed)
        {
            newV.x = Mathf.Abs(newV.x);
            newV.z = Mathf.Abs(newV.z);
        }
        return newV;
    }
}

