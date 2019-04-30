using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HitboxMaker : MonoBehaviour
{

	public FactionType Faction;
	CharacterBase m_charBase;
    Orientation m_orient;

	void Awake () {
        m_charBase = GetComponent<CharacterBase>();
        m_orient = GetComponent<Orientation>();
	}

	void Start() {
		if (GetComponent<Attackable> ()) {
			Faction = GetComponent<Attackable> ().Faction;
		}
	}
	public LineHitbox createLineHB(float range, Vector2 aimPoint, Vector2 offset,float damage, float stun, float hitboxDuration,
		Vector2 knockback, bool followObj = true,ElementType element = ElementType.PHYSICAL) {
		if (m_charBase != null) {
			//aimPoint = m_orient.OrientVectorToDirection (aimPoint);
			//offset = m_orient.OrientVectorToDirection (offset);
		}
		Vector3 newPos = new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, 0);
		GameObject go = Instantiate(HitboxList.Instance.HitboxLine,newPos,Quaternion.identity) as GameObject; 
		LineHitbox line = go.GetComponent<LineHitbox> ();
		line.setRange (range);
		line.Damage = damage;
		line.setAimPoint (aimPoint);
		line.Duration = hitboxDuration;
		/*if (m_charBase != null)
			line.Knockback = m_orient.OrientVectorToDirection(knockback);*/
		line.IsFixedKnockback = true;
		line.Creator = gameObject;
		line.Faction = Faction;
		line.AddElement(element);
		line.Stun = stun;
		line.Init();

		//ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnHitboxCreate(line));
		return line;
	}

    /*public Hitbox CreateHitbox(HitboxInfo hbi) {
		//Vector2 cOff = (m_charBase == null) ? hbi.HitboxOffset : m_orient.OrientVectorToDirection(hbi.HitboxOffset);
		//Vector3 newPos = transform.position + (Vector3)cOff;
		var go = GameObject.Instantiate(HitboxList.Instance.Hitbox, newPos, Quaternion.identity);

		Hitbox newBox = go.GetComponent<Hitbox>();
        if (hbi.FollowCharacter) {
			go.transform.SetParent (gameObject.transform);
			newBox.transform.localScale = m_orient.OrientVectorToDirection(new Vector2 (hbi.HitboxScale.x / transform.localScale.x, hbi.HitboxScale.y / transform.localScale.y), false);
		} else {
			newBox.SetScale ((m_charBase == null) ? hbi.HitboxScale : m_orient.OrientVectorToDirection(hbi.HitboxScale,false));
		}
		newBox.Damage = hbi.Damage;
		newBox.FocusDamage = hbi.FocusDamage;
		newBox.Penetration = hbi.Penetration;
		newBox.Duration = hbi.HitboxDuration;
		newBox.Knockback = (m_charBase == null) ? hbi.Knockback : m_orient.OrientVectorToDirection(hbi.Knockback);
		newBox.IsFixedKnockback = hbi.FixedKnockback;
		newBox.Stun = hbi.Stun;
		newBox.FreezeTime = hbi.FreezeTime;
		newBox.AddElement(hbi.Element);
		newBox.Creator = gameObject;
		newBox.Faction = Faction;
		newBox.IsResetKnockback = hbi.ResetKnockback;
		if (hbi.FollowCharacter)
			newBox.SetFollow (gameObject,hbi.HitboxOffset);
		if (hbi.ApplyProps)
			ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnHitboxCreate(newBox));
		newBox.Init();
		return newBox;
	}*/

    /*public Hitbox CreateHitbox(Vector2 hitboxScale, Vector2 offset, float damage, float stun, float hitboxDuration, Vector2 knockback, bool fixedKnockback = true,
        bool followObj = true, ElementType element = ElementType.PHYSICAL, bool applyProps = true)
    {
        //Vector2 cOff = (m_charBase == null) ? offset : m_orient.OrientVectorToDirection(offset);
        Vector3 newPos = transform.position + (Vector3)cOff;
        var go = GameObject.Instantiate(HitboxList.Instance.Hitbox, newPos, Quaternion.identity);

        Hitbox newBox = go.GetComponent<Hitbox>();
        if (followObj) {
            go.transform.SetParent (gameObject.transform);
            newBox.transform.localScale = m_orient.OrientVectorToDirection(new Vector2 (hitboxScale.x / transform.localScale.x, hitboxScale.y / transform.localScale.y), false);
        } else {
            newBox.SetScale ((m_charBase == null) ? hitboxScale : m_orient.OrientVectorToDirection(hitboxScale,false));
        }
        newBox.Damage = damage;
        newBox.Duration = hitboxDuration;
        newBox.Knockback = (m_charBase == null) ? knockback : m_orient.OrientVectorToDirection(knockback);
        newBox.IsFixedKnockback = fixedKnockback;
        newBox.Stun = stun;
        newBox.AddElement(element);
        newBox.Creator = gameObject;
        newBox.Faction = Faction;
        if (followObj)
            newBox.SetFollow (gameObject,offset);
        if (applyProps)
            ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnHitboxCreate(newBox));
        newBox.Init();
        return newBox;
    }*/

    /*public HitboxDoT CreateHitboxDoT(Vector2 hitboxScale, Vector2 offset, float damage, float stun, float hitboxDuration, Vector2 knockback, bool fixedKnockback = true,
        bool followObj = true, ElementType element = ElementType.PHYSICAL)
    {
        //Vector2 cOff = (m_charBase == null) ? offset : m_orient.OrientVectorToDirection(offset);
        //Vector3 newPos = transform.position + (Vector3)cOff;
        var go = GameObject.Instantiate(HitboxList.Instance.HitboxDoT, newPos, Quaternion.identity);

        HitboxDoT newBox = go.GetComponent<HitboxDoT>();
        if (followObj) {
            go.transform.SetParent (gameObject.transform);
            Vector2 ls = new Vector2 (hitboxScale.x / transform.localScale.x, hitboxScale.y / transform.localScale.y);
            newBox.transform.localScale = (m_charBase == null) ?  ls : m_orient.OrientVectorToDirection(ls , false);
        } else {
            newBox.SetScale ((m_charBase == null) ? hitboxScale : m_orient.OrientVectorToDirection(hitboxScale,false));
        }
        newBox.Damage = damage;
        newBox.Duration = hitboxDuration;
        newBox.Knockback = (m_charBase == null) ? knockback : m_orient.OrientVectorToDirection(knockback);
        newBox.IsFixedKnockback = fixedKnockback;
        newBox.Stun = stun;
        newBox.AddElement(element);
        newBox.Creator = gameObject;
        newBox.Faction = Faction;

        ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnHitboxCreate(newBox)); 
        newBox.Init();
        return newBox;
    }*/

    /*public HitboxMulti CreateHitboxMulti(Vector2 hitboxScale, Vector2 offset, float damage, float stun, float hitboxDuration, Vector2 knockback, bool fixedKnockback = true,
        bool followObj = true, ElementType element = ElementType.PHYSICAL, float refreshTime = 0.2f)
    {
        //Vector2 cOff = (m_charBase == null) ? offset : m_orient.OrientVectorToDirection(offset);
        //Vector3 newPos = transform.position + (Vector3)cOff;
        //var go = GameObject.Instantiate(HitboxList.Instance.HitboxMulti, newPos, Quaternion.identity);
        HitboxMulti newBox = go.GetComponent<HitboxMulti>();

        if (followObj) {
            go.transform.SetParent (gameObject.transform);
            Vector2 ls = new Vector2 (hitboxScale.x / transform.localScale.x, hitboxScale.y / transform.localScale.y);
            newBox.transform.localScale = (m_charBase == null) ?  ls : m_orient.OrientVectorToDirection(ls , false);
        } else {
            newBox.SetScale ((m_charBase == null) ? hitboxScale : m_orient.OrientVectorToDirection(hitboxScale,false));
        }
        newBox.Damage = damage;
        newBox.Duration = hitboxDuration;
        newBox.Knockback = (m_charBase == null) ? knockback : m_orient.OrientVectorToDirection(knockback);
        newBox.IsFixedKnockback = fixedKnockback;
        newBox.Stun = stun;
        newBox.AddElement(element);
        newBox.Creator = gameObject;
        newBox.Faction = Faction;
        newBox.refreshTime = refreshTime;

        ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnHitboxCreate(newBox));
    newBox.Init();
        return newBox;
    }*/
    /*public GameObject CreateItem( GameObject prefab, Vector2 creationPoint, Vector2 throwPoint,
        float throwSpeed) {
        //Vector2 cOff = (m_charBase == null) ? creationPoint : m_orient.OrientVectorToDirection(creationPoint);
        Vector3 newPos = transform.position + (Vector3)cOff;
        GameObject go = Instantiate (prefab, newPos, Quaternion.identity);
        if (go.GetComponent<CharacterBase> () != null) {
            Vector2 throwVec = throwSpeed * throwPoint.normalized;
            //go.GetComponent<CharacterBase> ().AddToVelocity (m_orient.OrientVectorToDirection (throwVec));
        }
        return go;
    }*/
    /*
    public Projectile CreateProjectile(GameObject prefab, Vector2 creationPoint, Vector2 targetPoint,
        float projectileSpeed, float damage, float stun, float projectileDuration, Vector2 knockback, 
        bool fixedKnockback = true, ElementType element = ElementType.PHYSICAL)
    {
        Vector2 cOff = (m_charBase == null) ? creationPoint : m_orient.OrientVectorToDirection(creationPoint);
        Vector3 newPos = transform.position + (Vector3)cOff;
        GameObject go;
        if (prefab != null) {
            go = GameObject.Instantiate (prefab, newPos, Quaternion.identity);
        } else {
            go = GameObject.Instantiate (HitboxList.Instance.StandardProjectile, newPos, Quaternion.identity);
        }
        Projectile newProjectile = go.GetComponent<Projectile>();

        newProjectile.Damage = damage;
        newProjectile.Duration = projectileDuration;
        newProjectile.Knockback = (m_charBase == null) ? knockback : m_orient.OrientVectorToDirection(knockback);
        newProjectile.IsFixedKnockback = fixedKnockback;
        newProjectile.Stun = stun;
        newProjectile.AddElement(element);
        newProjectile.Creator = gameObject;
        newProjectile.Faction = Faction;
        newProjectile.AimPoint = (m_charBase == null) ? targetPoint : m_orient.OrientVectorToDirection(targetPoint);
        newProjectile.ProjectileSpeed = projectileSpeed;

        //ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnHitboxCreate(newProjectile));
        newProjectile.Init();
        return newProjectile;
    }*/

    public void ClearHitboxes()
	{
		foreach (Hitbox hb in GetComponentsInChildren<Hitbox>())
			Destroy(hb.gameObject);
	}

	public void RegisterHit(GameObject otherObj, HitInfo hi, HitResult hr)
	{
		if (m_charBase)
            m_charBase.RegisterHit (otherObj,hi,hr);
	}

}
