using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { UP, RIGHT, DOWN, LEFT }

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class Orientation : MonoBehaviour
{

    // Tracking m_sprite orientation (flipping if left)...
    //private SpriteRenderer m_sprite;
    public bool FacingLeft = false;
    public Direction CurrentDirection = Direction.DOWN;

    // Use this for initialization
    internal void Awake()
    {
        //m_sprite = GetComponent<SpriteRenderer>();
        if (GetComponent<PersistentItem>() != null)
            GetComponent<PersistentItem>().InitializeSaveLoadFuncs(storeData, loadData);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetDirection(Direction d)
    {
        CurrentDirection = d;
        if (d == Direction.LEFT)
        {
            FacingLeft = true;
        }
        else
        {
                
                
        }
        if (d == Direction.RIGHT)
        {
            FacingLeft = false;
        }
        if (GetComponent<AnimatorSprite>() != null)
        {
            GetComponent<AnimatorSprite>().SetDirection(d);
        }
    }
    public void SetDirection (bool facingLeft)
    {
        if (facingLeft)
            SetDirection(Direction.LEFT);
        else
            SetDirection(Direction.RIGHT);
    }
    public Direction VectorToDirection(Vector3 v)
    {
        if (Mathf.Abs(v.z) > Mathf.Abs(v.x))
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
    public Vector3 OrientVectorToDirection2D(Vector3 v, bool negativesAllowed = true)
    {
        Vector3 newV = new Vector3(v.x, v.y, v.z);
        if (FacingLeft)
        {
            newV.x = -v.x;
        }
        return newV;
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
    public Direction DirectionToPoint(Vector3 point)
    {
        Vector3 me = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 you = new Vector3(point.x, 0f, point.z);
        float angleToPoint = Vector3.SignedAngle(Vector3.right, you - me, Vector3.up);
        Direction testDirection = Direction.UP;
        if (angleToPoint <= -45f && angleToPoint >= -135f)
            testDirection = Direction.UP;
        if (angleToPoint < -135f || angleToPoint > 135f)
            testDirection = Direction.LEFT;
        if (angleToPoint < 135f && angleToPoint > 45f)
            testDirection = Direction.DOWN;
        if (angleToPoint < 45f && angleToPoint > -45f)
            testDirection = Direction.RIGHT;
        return testDirection;
    }
    public bool FacingPoint(Vector3 point)
    {
        return FacingPoint(point,CurrentDirection);
    }
    public bool FacingPoint(Vector3 point, Direction d)
    {
        return DirectionToPoint(point) == CurrentDirection;
    }
    private void storeData(CharData d)
    {
        d.SetInt("Direction", (int)CurrentDirection);
        d.SetBool("FacingLeft", FacingLeft);
    }

    private void loadData(CharData d)
    {
        SetDirection((Direction)d.GetInt("Direction"));
        FacingLeft = d.GetBool("FacingLeft");
    }
}

