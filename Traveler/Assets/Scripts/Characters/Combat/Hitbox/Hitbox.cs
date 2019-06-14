using UnityEngine;
using System.Collections.Generic;

public enum HitResult { NONE, FOCUSHIT,HIT,HEAL, BLOCKED, REFLECTED };

public enum ElementType { PHYSICAL, FIRE, BIOLOGICAL, PSYCHIC, LIGHTNING };

public class HitInfo  {
	public float Damage = 10f;
	public float FocusDamage = 10f;
	public float Penetration = 0f;
	public Vector3 Knockback = new Vector3();
	public bool IsFixedKnockback = false;
	public bool ResetKnockback = true;
	public float Stun = 0f;
	public float FreezeTime = 0f;
	public List<ElementType> Element = new List<ElementType>();
	public Hitbox mHitbox;

	public ActionInfo Attack;
	public GameObject Creator;
	public GameObject target;
	public float LastTimeHit;

	public bool HasElement(ElementType element) {
		foreach (ElementType et in Element) {
			if (et == element)
				return true;
		}
		return false;
	}
	public float TimeSinceLastHit() {
		return Time.timeSinceLevelLoad - LastTimeHit;
	}
}

public class Hitbox : MonoBehaviour {

	[SerializeField]
	private float m_damage = 10.0f;
	public float Damage { get { return m_damage; } set { m_damage = value; } }

	[SerializeField]
	private float m_focusDamage = -1f;
	public float FocusDamage { get { return m_focusDamage; } set { m_focusDamage = value; } }

	[SerializeField]
	private float m_penetration = 0.0f;
	public float Penetration { get { return m_penetration; } set { m_penetration = value; } }


	[SerializeField]
	private float m_duration = 1.0f;
	public float Duration { get { return m_duration; } set { m_duration = value; } }

	[SerializeField]
	protected bool m_hasDuration = true;

	[SerializeField]
	private bool m_isFixedKnockback = false;
	public bool IsFixedKnockback { get { return m_isFixedKnockback; } set { m_isFixedKnockback = value; } }

	[SerializeField]
	private bool m_resetKnockback = true;
	public bool IsResetKnockback { get { return m_resetKnockback; } set { m_resetKnockback = value; } }

	[SerializeField]
	private Vector3 m_knockback = new Vector3(0.0f,40.0f,0f);
	public Vector3 Knockback { get { return m_knockback; } set { m_knockback = value; } }

	[SerializeField]
	private float m_stun = 0.0f;
	public float Stun { get { return m_stun; } set { m_stun = value; } }

	[SerializeField]
	private bool m_isRandomKnockback = false;
	public bool IsRandomKnockback { get { return m_isRandomKnockback; } set { m_isRandomKnockback = value; } }

	[SerializeField]
	private float m_FreezeTime = 0f;
	public float FreezeTime { get { return m_FreezeTime; } set { m_FreezeTime = value; } }

	//private ElementType m_element = ElementType.PHYSICAL;
	[SerializeField]
	private List<ElementType> m_elementList = new List<ElementType> ();
	public List<ElementType> Element { get { return m_elementList; } set { m_elementList = value; } }

	public FactionType Faction = FactionType.HOSTILE;

	[HideInInspector]
	public GameObject Creator { get; set; }

	[SerializeField]
	private GameObject m_followObj;

	[SerializeField]
	public Vector3 m_followOffset;

	[SerializeField]
	private List<Collider> m_upRightDownLeftColliders;

	private CharacterBase m_charBase;
	private Vector4 m_knockbackRanges;
	public List<Attackable> m_collidedObjs = new List<Attackable> ();
	public List<Attackable> m_overlappingControl = new List<Attackable> (); 
	public bool HitboxActive { get { return m_hitboxActive; } private set { m_hitboxActive = value; } }

	private bool m_hitboxActive = true;
    private BoxCollider m_box;

	virtual public void Init()
	{
        m_charBase = Creator.GetComponent<CharacterBase>();
        m_box = GetComponent<BoxCollider>();

        if (m_focusDamage == -1f)
			m_focusDamage = m_damage;
		
		if (m_isRandomKnockback)
			RandomizeKnockback ();
		m_hasDuration = m_duration > 0;
		Tick();
		Debug.Log ("Hitbox initialized d "+ m_duration);
		if (m_elementList.Count == 0)
			m_elementList.Add (ElementType.PHYSICAL);
	}

	virtual internal void Update()
	{
		Tick();
	}

	protected virtual void Tick()
	{
		//Debug.Log ("Hitbox created");
		if (m_followObj != null)
			FollowObj();
		if (m_charBase != null) {
			//SwitchActiveCollider (m_charBase.FacingLeft);
		}
		if (m_hasDuration)
			MaintainOrDestroyHitbox();
	}

	public void SetScale(Vector3 scale)
	{
		transform.localScale = scale;
	}

	public void SetFollow(GameObject obj, Vector3 offset)
	{
		m_followObj = obj;
		m_followOffset = offset;
		Vector3 newOffset = new Vector3 (offset.x, offset.y, offset.z);
		if (m_followObj.GetComponent<BasicPhysics> () != null &&
		    m_followObj.GetComponent<Orientation> ().FacingLeft) {
			newOffset.x *= -1f;
			//Debug.Log ("Reverse");
		}
		m_followOffset = newOffset;
	}

	public void SetKnockbackRanges (float minX, float maxX,float minY, float maxY)
	{
		IsRandomKnockback = true;
		IsFixedKnockback = true;
		m_knockbackRanges = new Vector4 (minX, maxX, minY, maxY);
	}

	public HitboxInfo ToHitboxInfo() {
		HitboxInfo hbi = new HitboxInfo ();
		return hbi;

	}
	private void MaintainOrDestroyHitbox()
	{
		if (m_duration <= 0.0f) {
			Debug.Log ("Hitbox destroyed!" + m_duration);
			GameObject.Destroy (gameObject);
		}
		Duration -= Time.deltaTime;
	}

	private void FollowObj()
	{
		transform.position = new Vector3(m_followObj.transform.position.x + m_followOffset.x, m_followObj.transform.position.y + m_followOffset.y, m_followObj.transform.position.z + m_followOffset.z);
	}

	private void RandomizeKnockback ()
	{
		m_knockback.x = Random.Range (m_knockbackRanges.x, m_knockbackRanges.y);
		m_knockback.y = Random.Range (m_knockbackRanges.z, m_knockbackRanges.w);
	}

	protected virtual HitResult OnAttackable(Attackable atkObj)
	{
		if (!canAttack(atkObj))
			return HitResult.NONE;
		if (IsRandomKnockback)
			RandomizeKnockback();
		HitInfo newHI = ToHitInfo ();
		newHI.LastTimeHit = Time.timeSinceLevelLoad;
		newHI.Knockback = new Vector3 (Knockback.x, Knockback.y,Knockback.z);
		newHI.target = atkObj.gameObject;
		HitResult r = atkObj.TakeHit(newHI);
		m_collidedObjs.Add (atkObj);

		if (!m_overlappingControl.Contains (atkObj))
			m_overlappingControl.Add (atkObj);
		if (Creator != null) {
			Creator.GetComponent<HitboxMaker> ().RegisterHit (atkObj.gameObject, newHI, r);
		}
		CreateHitFX ( atkObj.gameObject, Knockback, r);
		return r;
	}

	public virtual HitInfo ToHitInfo() {
		HitInfo hi = new HitInfo ();
		hi.Damage = Damage;
		hi.FocusDamage = FocusDamage;
		hi.Stun = Stun;
		hi.Element = m_elementList;
		hi.Penetration = Penetration;
		hi.IsFixedKnockback = IsFixedKnockback;
		hi.ResetKnockback = m_resetKnockback;
		//Debug.Log (hi.ResetKnockback);
		hi.FreezeTime = m_FreezeTime;
		//Debug.Log ("MFreeze: " + m_FreezeTime);

		hi.Creator = Creator;
		hi.mHitbox = this;
		return hi;
	}

	protected bool canAttack(Attackable atkObj) {
		return !(!atkObj || (atkObj.gameObject == Creator) || m_collidedObjs.Contains (atkObj) || !atkObj.CanAttack (Faction));
	}
		
	internal HitResult OnTriggerEnter(Collider other)
	{
		if (!m_hitboxActive)
			return HitResult.NONE;
		OnHitObject (other);
		return OnAttackable (other.gameObject.GetComponent<Attackable> ());
	}
	protected virtual void OnHitObject(Collider other) {
	}
	internal void OnTriggerExit(Collider other)
	{
		/*
		 * TODO: Delay removal of collided object to avoid stuttered collisions 
		 */
		/*
		if (other.gameObject.GetComponent<Attackable> () && collidedObjs.Contains(other.gameObject.GetComponent<Attackable>())) {
			collidedObjs.Remove (other.gameObject.GetComponent<Attackable> ());
		}
		*/
		if (!m_hitboxActive)
			return;
		if (other.gameObject.GetComponent<Attackable> () 
			&& m_overlappingControl.Contains(other.gameObject.GetComponent<Attackable>())) {
			m_overlappingControl.Remove (other.gameObject.GetComponent<Attackable> ());
		}
	}

	private void SwitchActiveCollider(bool FacingLeft)
	{
		if (m_upRightDownLeftColliders.Count == 0)
			return;
		var dirIndex = ConvertDirToUpRightDownLeftIndex(FacingLeft);
		// Or'd check on enabled in case collider falls under several categories
		for (var i = 0; i < m_upRightDownLeftColliders.Count; i++)
		{
			m_upRightDownLeftColliders[i].enabled |= (i == dirIndex);
		}
	}

	private int ConvertDirToUpRightDownLeftIndex(bool FacingLeft)
	{
		if (FacingLeft)
			return 3;
		return 1;
	}


	public void AddElement(ElementType element) {
		m_elementList.Add (element);
	}
	public bool HasElement(ElementType element) {
		foreach (ElementType et in m_elementList) {
			if (et == element)
				return true;
		}
		return false;
	}
	public void ClearElement() {
		m_elementList.Clear ();
	}
	protected void CreateHitFX(GameObject hitObj, Vector3 knockback, HitResult hr) {
		foreach (ElementType et in m_elementList) {
			m_hitFX (et, hitObj, knockback, hr);
			m_playerSound (et, hr);
		}
	}
	private void m_hitFX(ElementType et, GameObject hitObj, Vector3 knockback, HitResult hr) {
		/*GameObject fx = null;
		if (hr == HitResult.BLOCKED || hr == HitResult.FOCUSHIT) {
			fx = GameObject.Instantiate (FXHit.Instance.FXHitBlock, hitObj.transform.position, Quaternion.identity);
		}
		if (hr == HitResult.HEAL) {
			fx = GameObject.Instantiate (FXHit.Instance.FXHeal, hitObj.transform.position, Quaternion.identity);
		} else if (hr == HitResult.HIT || hr == HitResult.FOCUSHIT) {
			switch (et) {
			case ElementType.PHYSICAL:
				fx = GameObject.Instantiate (FXHit.Instance.FXHitPhysical, hitObj.transform.position, Quaternion.identity);
				break;
			case ElementType.FIRE:
				fx = GameObject.Instantiate (FXHit.Instance.FXHitFire, hitObj.transform.position, Quaternion.identity);
				break;
			case ElementType.LIGHTNING:
				fx = GameObject.Instantiate (FXHit.Instance.FXHitLightning, hitObj.transform.position, Quaternion.identity);
				break;
			case ElementType.BIOLOGICAL:
				fx = GameObject.Instantiate (FXHit.Instance.FXHitBiological, hitObj.transform.position, Quaternion.identity);
				break;
			case ElementType.PSYCHIC:
				fx = GameObject.Instantiate (FXHit.Instance.FXHitPsychic, hitObj.transform.position, Quaternion.identity);
				break;
			default:
				Debug.Log ("Hit Effect not yet added");
				break;
			}
		}
		if (fx != null) {
			fx.GetComponent<Follow> ().followObj = hitObj;
			float angle = (Mathf.Atan2 (knockback.y, knockback.x) * 180) / Mathf.PI;
			fx.transform.Rotate (new Vector3 (0f, 0f, angle));
		}*/
	}
	protected void m_playerSound(ElementType et,  HitResult hr) {
		
		/*if (hr == HitResult.BLOCKED || hr == HitResult.FOCUSHIT) {
			FindObjectOfType<AudioManager> ().PlayClipAtPos (FXHit.Instance.SFXGuard,transform.position,0.5f,0f,0.25f);
		} else if (hr == HitResult.HEAL) {
			FindObjectOfType<AudioManager> ().PlayClipAtPos (FXHit.Instance.SFXHeal,transform.position,0.5f,0f,0.25f);
		} else if (hr == HitResult.HIT) {
			switch (et) {
			case ElementType.PHYSICAL:
				FindObjectOfType<AudioManager> ().PlayClipAtPos (FXHit.Instance.SFXPhysical,transform.position,0.5f,0f,0.25f);
				break;
			case ElementType.FIRE:
				FindObjectOfType<AudioManager> ().PlayClipAtPos (FXHit.Instance.SFXFire,transform.position,0.5f,0f,0.25f);
				break;
			case ElementType.LIGHTNING:
				FindObjectOfType<AudioManager> ().PlayClipAtPos (FXHit.Instance.SFXElectric,transform.position,0.75f,0f,0.25f);
				break;
			case ElementType.BIOLOGICAL:
				//FindObjectOfType<AudioManager> ().PlayClipAtPos (FXHit.Instance.SFXPhysical,transform.position,0.5f,0f,0.25f);
				break;
			case ElementType.PSYCHIC:
				FindObjectOfType<AudioManager> ().PlayClipAtPos (FXHit.Instance.SFXPsychic,transform.position,0.5f,0f,0.25f);
				break;
			default:
				Debug.Log ("Hit Effect not yet added");
				break;
			}
		}*/
	}

	public virtual void SetHitboxActive(bool a) {
		m_hitboxActive = a;
		m_collidedObjs.Clear ();
		m_overlappingControl.Clear ();
	}
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .8f);
        Gizmos.DrawCube(transform.position, m_box.size);
    }
}