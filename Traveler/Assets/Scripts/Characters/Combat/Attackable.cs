using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum FactionType { IMMUNE, ALLIES, ENEMIES, NEUTRAL };

[System.Serializable]
public class Resistence {
	public ElementType Element;
	public float Duration = 0f;
	public float Percentage = 0f;
	public bool Timed;
	public float StunResist = 0f;
	public float KnockbackResist = 0f;
	public bool AvoidOverflow = true;
	public float OverflowAmount = 0f;
}

public class Attackable : MonoBehaviour
{
	private const float BOTTOM_OF_WORLD = -1000f;

	[HideInInspector]
	public float Health { get { return m_health; } private set { m_health = value; } }

	[SerializeField]
	private float m_health = 100.0f;
	public float MaxHealth = 100.0f;

	public bool Alive = true;
	public float DeathTime = 0.0f;
	private float m_currDeathTime;
	public FactionType Faction = FactionType.NEUTRAL;

	public List<Resistence> Resistences  = new List<Resistence>();
	private Dictionary< ElementType, Resistence> m_fullResistences = new Dictionary< ElementType, Resistence>();
	private CharacterBase m_charBase;
    private BasicPhysics m_physics;

    public bool DisplayHealth = true;
	public bool CanTarget = true;
	public GameObject Killer;
	public GameObject DeathFX;
	//private HealthDisplay m_display;

	internal void Awake()
	{
        m_charBase = GetComponent<CharacterBase>();
        m_physics = GetComponent<BasicPhysics>();
        m_health = Mathf.Min (Health, MaxHealth);
		m_currDeathTime = 0.0f;
		InitResistences ();
		if (GetComponent<PersistentItem> () != null)
			GetComponent<PersistentItem> ().InitializeSaveLoadFuncs (storeData,loadData);
	}

	internal void InitResistences() {
		m_fullResistences.Clear ();
		for (int i=0; i < 5; i++) {
			Resistence r = new Resistence ();

			r.Element = (ElementType)i;
			m_fullResistences.Add ( (ElementType)i, r);
		}
		foreach (Resistence r in Resistences) {
			AddResistence (r);
		}
		AddResistence (ElementType.BIOLOGICAL, 100f, false, false);
	}

	internal void Start() {
        UIBarInfo ubi = new UIBarInfo();
        ubi.FillColor = Color.red;
        ubi.UILabel = "Health";
        ubi.funcUpdate = UpdateHealthValues;
        ubi.target = gameObject;
        if (GetComponent<CharacterBase>() != null)
            GetComponent<CharacterBase>().AddUIBar(ubi);
        /*
		if (DisplayHealth) {
			if (GetComponent<BasicMovement>() != null && GetComponent<BasicMovement> ().IsCurrentPlayer) {
				UIBarInfo ubi = new UIBarInfo ();
				ubi.FillColor = Color.red;
				ubi.UILabel = "Health";
				ubi.funcUpdate = UpdateHealthValues;
				ubi.target = gameObject;
				FindObjectOfType<GUIHandler>().AddUIBar (ubi);
			} else {
				m_display = Instantiate (UIList.Instance.HealthBarPrefab, this.transform).GetComponent<HealthDisplay>();
				m_display.SetMaxHealth (MaxHealth);
			}
		} */
    }

	public void UpdateHealthValues(UIBarInfo ubi) {
		ubi.element.GetComponent<UIBar> ().UpdateValues (Mathf.Round(ubi.target.GetComponent<Attackable>().Health), 
			Mathf.Round(ubi.target.GetComponent<Attackable>().MaxHealth));
	}
    
	private void CheckDeath()
	{
		Alive = Health > 0;
		if (Alive)
			return;
		if (m_currDeathTime > DeathTime) {
			ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnDeath ());
			/*if (GetComponent<BasicMovement> () && GetComponent<BasicMovement> ().IsCurrentPlayer)
				PauseGame.OnPlayerDeath ();
			if (DeathFX != null) {
				Instantiate (DeathFX, transform.position,Quaternion.identity);
			}*/
			Destroy (gameObject);
		}
		GetComponent<SpriteRenderer>().color = Color.Lerp (Color.white, Color.black, (m_currDeathTime) / DeathTime);
		m_currDeathTime += Time.deltaTime;
	}

	private void CheckResistanceValidities()
	{
		foreach (Resistence r in Resistences)
		{
			if (r.Timed) {
				r.Duration -= Time.deltaTime;
				if (r.Duration <= 0.0f)
					Resistences.Remove(r);
			}
		}
	}

	internal void Update() {
		if (transform.position.y < BOTTOM_OF_WORLD)
			SetHealth (-1000.0f);
		CheckDeath();
		CheckResistanceValidities ();
	}

	//resistences
	public Resistence SetResistence(ElementType element, float percentage, bool overflow = false, bool isTimed = false, float duration = 0f, 
		float resistStun = 0f, float resistKnockback = 0f) {
		ClearResistence (element);
		Resistence r = newResist (element, percentage, overflow, isTimed, duration, resistStun, resistKnockback);
		AddResistence (r);
		return r;
	}

	private Resistence newResist(ElementType element, float percentage, bool overflow = false, bool isTimed = false, float duration = 0f, 
		float resistStun = 0f, float resistKnockback = 0f) {
		Resistence r = new Resistence ();
		r.Element = element;
		r.Percentage = percentage;
		r.Duration = duration;
		r.StunResist = resistStun;
		r.KnockbackResist = resistKnockback;
		r.Timed = isTimed;
		r.AvoidOverflow = overflow;
		return r;
	}
	public Resistence AddResistence(ElementType element, float percentage, bool overflow = false, bool isTimed = false, float duration = 0f, 
		float resistStun = 0f, float resistKnockback = 0f) {
		Resistence r = newResist (element, percentage, overflow, isTimed, duration, resistStun, resistKnockback);
		AddResistence (r);
		return r;
	}

	public void AddResistence(Resistence r) {
		Resistence fr = m_fullResistences[r.Element];
		float pDiff = r.Percentage;
		if (r.AvoidOverflow) {
			pDiff = Mathf.Min (100f - fr.Percentage, r.Percentage);
			r.OverflowAmount = Mathf.Max(0f, r.Percentage - (100f - fr.Percentage));
		}
		fr.Percentage += pDiff;
		fr.StunResist += r.StunResist;
		fr.KnockbackResist += r.KnockbackResist;
		if (!Resistences.Contains(r))
			Resistences.Add (r);
	}
	public void RemoveResistence(Resistence r) {
		if (!Resistences.Contains (r))
			return;
		Resistence fr = m_fullResistences[r.Element];
		fr.Percentage -= r.Percentage;
		fr.StunResist -= r.StunResist;
		fr.KnockbackResist -= r.KnockbackResist;
		Resistences.Remove (r);
		foreach (Resistence or in Resistences) {
			if (or.Element == r.Element && r.AvoidOverflow) {
				float pDiff = Mathf.Min (100f - fr.Percentage, or.OverflowAmount);
				or.OverflowAmount = Mathf.Max(0f, or.OverflowAmount - (100f - fr.Percentage));
				fr.Percentage += pDiff;
			}
		}
	}

	public void ClearResistence(ElementType element) {
		foreach (Resistence r in Resistences) {
			if (r.Element == element) {
				Resistences.Remove (r);
			}
		}
		Resistence re = new Resistence();
		re.Element = element;
		m_fullResistences [element] = re;
	}

	public Resistence GetResistence(ElementType element) {
		return m_fullResistences [element];
	}

	private void ApplyHitToPhysics(HitInfo hi)
	{
		Resistence r = GetAverageResistences (hi.Element);
		m_physics.FreezeInAir (hi.FreezeTime);
		
		Vector3 kb = hi.Knockback - (hi.Knockback * Mathf.Min(1f,(r.KnockbackResist/100f)));
		if (!m_physics)
			return;
		if (hi.IsFixedKnockback)
		{
			if (kb.y != 0f && hi.ResetKnockback)
				m_physics.CancelVerticalMomentum ();
			m_physics.AddToVelocity(kb);
			return;
		}

		Vector3 hitVector = transform.position - hi.mHitbox.transform.position;
		float angle = Mathf.Atan2(hitVector.y,hitVector.x); //*180.0f / Mathf.PI;
		Vector2 force = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		force.Scale(new Vector2(kb.magnitude, kb.magnitude));
		float counterF = m_physics.m_trueVelocity.y * (1 / Time.deltaTime);
		if (counterF < 0)
			force.y = force.y - counterF;
		
		m_physics.AddToVelocity(force);
		hi.Knockback = force;
	}

	void TakeDoT(HitboxDoT hbdot) {
		Resistence r =  GetAverageResistences(hbdot.Element);
		float d = hbdot.Damage - (hbdot.Damage * (r.Percentage / 100f));
		DamageObj (d * Time.deltaTime);
		if (Health <= 0f) {
			Killer = hbdot.Creator;
		}
		if (hbdot.IsFixedKnockback) {
			Vector2 kb = hbdot.Knockback  - (hbdot.Knockback * Mathf.Min(1f,(r.KnockbackResist/100f)));
			if (GetComponent<BasicPhysics>() != null)
				GetComponent<BasicPhysics>().AddToVelocity (kb * Time.deltaTime); 
		} else {
			Vector3 otherPos = gameObject.transform.position;
			float angle = Mathf.Atan2 (transform.position.y - otherPos.y, transform.position.x - otherPos.x); //*180.0f / Mathf.PI;
			float magnitude = hbdot.Knockback.magnitude;
			float forceX = Mathf.Cos (angle) * magnitude;
			float forceY = Mathf.Sin (angle) * magnitude;
			Vector2 force = new Vector2 (-forceX, -forceY);
			if (GetComponent<BasicPhysics>() != null)
				GetComponent<BasicPhysics>().AddToVelocity (force*Time.deltaTime);
		}
	}
	public HitResult TakeHit(Hitbox hb) {
		return TakeHit (hb.ToHitInfo ());
	}
	public HitResult TakeHit(HitInfo hi)
	{
		ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnHit (hi, hi.Creator));
		HitboxDoT hd = hi.mHitbox as HitboxDoT;
		if (hd != null) {
			TakeDoT (hd);
			return HitResult.NONE;
		}
		if (GetComponent<AICharacter>()) {
			GetComponent<AICharacter> ().OnHit (hi);
		}
		Resistence r =  GetAverageResistences(hi.Element);
		float d;
		//Debug.Log ("Damage; " + hi.Damage + " r: " + r.Percentage);
		d = hi.Damage - (hi.Damage * (r.Percentage / 100f));
		float fD = hi.FocusDamage;
		d = DamageObj (d);

		ApplyHitToPhysics(hi);
		float s = hi.Stun - (hi.Stun * Mathf.Min(1f,(r.StunResist/100f)));
		if (hi.Stun > 0f && m_charBase) {
			if (s <= 0f)
				return HitResult.BLOCKED;
            m_charBase.RegisterStun (s, true, hi, (d >= 0f && fD >= 0f));
		}
		if (Health <= 0f) {
			Killer = hi.Creator;
		}
		if (d == 0f && fD == 0f) {
			return HitResult.NONE;
		} else if (d < 0f) {
			return HitResult.HIT;
		} else if (fD >= 0f) {
			return HitResult.FOCUSHIT;
		} else {
			return HitResult.HEAL;
		}
	}

	internal void OnTriggerEnter2D(Collider2D other)
	{
		//ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnAttack ());
	}
	public void SetHealth (float newHealth) {
		DamageObj (Health - newHealth);
	}
	public float DamageObj(float damage)
	{
		if (!Alive)
			return 0f;
		float healthBefore = Health;
		Health = Mathf.Max(Mathf.Min(MaxHealth, Health - damage), 0);
		Alive = (Health > 0);
		float diff =  Health - healthBefore;
		/*if (m_display != null) {
			m_display.ChangeValue (diff, Health);
		}*/
		return diff;
	}

	public bool CanAttack(FactionType otherFaction) {
		return (otherFaction == FactionType.NEUTRAL || Faction == FactionType.NEUTRAL || otherFaction != Faction);
	}

	public void SetFaction(FactionType f) {
		Faction = f;
		if (GetComponent<HitboxMaker> ())
			GetComponent<HitboxMaker> ().Faction = f;
	}

	private Resistence GetAverageResistences(List<ElementType> le) {
		Resistence newR = new Resistence ();
		int numResists = 0;
		foreach (ElementType et in le) {
			Resistence pr = GetResistence (et);
			newR.Percentage += pr.Percentage;
			newR.KnockbackResist += pr.KnockbackResist;
			newR.StunResist += pr.StunResist;
			numResists ++;
		}
		if (numResists > 0) {
			newR.Percentage /= numResists;
			newR.KnockbackResist /= numResists;
			newR.StunResist /= numResists;
		} else {
			newR.Percentage = 0f;
			newR.KnockbackResist = 0f;
			newR.StunResist = 0f;
		}
		return newR;
	}

	private void storeData(CharData d) {
		d.SetFloat("Health", Health);
		d.SetFloat("MaxHealth", MaxHealth);
		d.SetInt("Faction", (int)Faction);
	}

	private void loadData(CharData d) {
		MaxHealth = d.GetFloat("MaxHealth");
        Health = d.GetFloat("Health");
		Faction = (FactionType)d.GetInt("Faction");
	}
}
