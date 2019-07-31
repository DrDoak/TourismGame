using System;
using System.Collections.Generic;
using UnityEngine;

public enum AttackState { STARTUP, ATTACK, RECOVERY, INACTIVE };
public delegate void AttackProgress(AttackState attackState);

[System.Serializable]
public class AttackAnimInfo {
	public float AnimSpeed = 1f;
	public float StartUpTime = 0.5f;
	public float AttackTime = 0.0f;
	public float RecoveryTime = 0.5f;
	public string StartUpAnimation = "none";
	public string RecoveryAnimation = "none";
}

[System.Serializable]
public class HitboxInfo {
	public Vector3 HitboxScale = new Vector3 (1.0f, 1.0f,1.0f);
	public Vector3 HitboxOffset = new Vector3(0f,0f);
	public float Damage = 10.0f;
	public float FocusDamage = -1f;
	public float Penetration = 0.0f;
	public float Stun = 0.3f;
	public float HitboxDuration = 0.5f;
	public Vector3 Knockback = new Vector3(10.0f,10.0f,0f);
	public bool FixedKnockback = true;
	public bool ResetKnockback = true;
	public ElementType Element = ElementType.PHYSICAL;
	public bool ApplyProps = true;
	public bool FollowCharacter = true;
	public float FreezeTime = 0.0f;
	public float Delay = 0.0f;
}

[System.Serializable]
public class SoundInfo {
	//public GameObject AttackFX;
	public AudioClip StartupSoundFX ;
	public AudioClip AttackSoundFX ;
}

[System.Serializable]
public class AIInfo {
	public float Frequency = 1f;
	public bool UniqueAIPrediction = false;
	public bool AutoAttack = false;
	public Vector3 AIPredictionHitbox = Vector3.zero;
	public Vector3 AIPredictionOffset = Vector3.zero;
	public bool DrawPredictionHitbox = false;
}

[System.Serializable]
public class SentimentAttack {
	public int RequiredMinSentiment = 0;
	public int ConsumedSentiment = 0;
	public bool DrainOnWhiff = true;
	public bool TransferSentiment = false;
}

/*[System.Serializable]
public class AttackFXList {
	public List<AttackFXInfo> OnStartUp = new List<AttackFXInfo>();
	public List<AttackFXInfo> OnAttack = new List<AttackFXInfo>();
	public List<AttackFXInfo> OnRecovery = new List<AttackFXInfo>();
}*/

public class ActionInfo : MonoBehaviour
{
	private AttackState m_progress;
	private float m_timeSinceStart = 0.0f;
	private Dictionary<AttackState, float> m_progressEndTimes;
	private Dictionary<AttackState, Action> m_inTickFunctions;
	private Dictionary<AttackState, Action> m_progressCalls;
	private float m_AttackDelay = 0.0f;

	public AttackState CurrentProgress { get { return m_progress; } }
	private event AttackProgress ProgressEvent = delegate {};

	public string AttackName = "default";

	public List<HitboxInfo> m_HitboxInfo;
	public AttackAnimInfo m_AttackAnimInfo;
	public AIInfo m_AIInfo;
	public SoundInfo m_SoundInfo;
	public SentimentAttack m_SentimentInfo;

	//public AttackFXList m_attackFXList;

	protected CharacterBase m_charBase;
	protected HitboxMaker m_hitboxMaker;

	//private Dictionary<AttackFXInfo,float> m_queuedFX;


	internal void Awake()
	{
        m_charBase = GetComponent<CharacterBase>();
		m_hitboxMaker = GetComponent<HitboxMaker>();
		//m_queuedFX = new Dictionary<AttackFXInfo, float> ();

		m_progress = AttackState.INACTIVE;
		m_progressEndTimes = new Dictionary<AttackState, float>()
		{
			{ AttackState.STARTUP, m_AttackAnimInfo.StartUpTime },
			{ AttackState.ATTACK,  m_AttackAnimInfo.StartUpTime +  m_AttackAnimInfo.AttackTime },
			{ AttackState.RECOVERY,  m_AttackAnimInfo.StartUpTime +  m_AttackAnimInfo.AttackTime +  m_AttackAnimInfo.RecoveryTime },
			{ AttackState.INACTIVE, 0 }
		};
		m_AttackDelay = 0f;
		m_inTickFunctions= new Dictionary<AttackState, Action>()
		{
			{ AttackState.STARTUP, StartUpTick},
			{ AttackState.ATTACK, AttackTick },
			{ AttackState.RECOVERY, RecoveryTick },
			{ AttackState.INACTIVE, ConcludeTick },
		};		
		m_progressCalls = new Dictionary<AttackState, Action>()
		{
			{ AttackState.STARTUP, OnStartUp},
			{ AttackState.ATTACK, OnAttack },
			{ AttackState.RECOVERY, OnRecovery },
			{ AttackState.INACTIVE, OnConclude }
		};
	}
    public void SetOwner(GameObject go)
    {
        m_charBase = go.GetComponent<CharacterBase>();
        m_hitboxMaker = go.GetComponent<HitboxMaker>();
    }
	public void AddListener(AttackProgress ap)
	{
		ProgressEvent += ap;
	}
		
	public void Progress()
	{
		m_inTickFunctions [m_progress] ();
		m_timeSinceStart += Time.deltaTime;

        //updateFXQueue ();
        //Debug.Log("mTime: " + m_timeSinceStart + " next: " + m_progressEndTimes[m_progress] + m_AttackDelay);
		if (m_timeSinceStart < m_progressEndTimes [m_progress] + m_AttackDelay)
			return;
        //Debug.Log("Progressing event to: " + m_progress);
		m_progressCalls[NextInProgression()]();
		ProgressEvent.Invoke(m_progress);
	}

	private AttackState NextInProgression()
	{
		m_progress = (m_progress == AttackState.INACTIVE) ? AttackState.STARTUP : m_progress + 1;
		return m_progress;
	}

	public void DelayCurrentAttack(float delay) {
		m_AttackDelay += delay;
	}

	public void ResetAndProgress()
	{
		m_timeSinceStart = 0;
		m_AttackDelay = 0f;
		m_progress = AttackState.INACTIVE;
		Progress();
	}
		
	public virtual void OnHitConfirm(GameObject other, HitInfo hb, HitResult hr) {}

	public virtual void OnInterrupt(float stunTime, bool successfulHit, HitInfo hi) {}

	protected virtual void OnStartUp()
	{
		if (m_AIInfo.UniqueAIPrediction == false && m_HitboxInfo.Count > 0){
			m_AIInfo.AIPredictionHitbox = m_HitboxInfo[0].HitboxScale;
			m_AIInfo.AIPredictionOffset = m_HitboxInfo[0].HitboxOffset;
		}
        /*if (m_SoundInfo.StartupSoundFX != null)
			FindObjectOfType<AudioManager> ().PlayClipAtPos (m_SoundInfo.StartupSoundFX,transform.position,0.5f,0f,0.25f);

		foreach (AttackFXInfo afi in m_attackFXList.OnStartUp) {
			if (afi.Delay <= 0f)
				CreateAttackFX (afi);
			else
				m_queuedFX.Add (afi,Time.timeSinceLevelLoad + afi.Delay);
		}*/
    }

    protected virtual void OnAttack()
	{
		if (m_HitboxInfo.Count > 0) {
			createHitboxes ();
		}
        /*if (m_SoundInfo.AttackSoundFX != null)
			FindObjectOfType<AudioManager> ().PlayClipAtPos (m_SoundInfo.AttackSoundFX,transform.position,0.5f,0f,0.25f);
            
		foreach (AttackFXInfo afi in m_attackFXList.OnAttack) {
			if (afi.Delay <= 0f)
				CreateAttackFX (afi);
			else
				m_queuedFX.Add (afi,Time.timeSinceLevelLoad + afi.Delay);
		}*/
    }

    protected virtual void OnRecovery() {
		/*foreach (AttackFXInfo afi in m_attackFXList.OnRecovery) {
			if (afi.Delay <= 0f)
				CreateAttackFX (afi);
			else
				m_queuedFX.Add (afi,Time.timeSinceLevelLoad + afi.Delay);
		}*/
	} 

	protected virtual void OnConclude() {}

	protected virtual void StartUpTick() { } 
	protected virtual void AttackTick() { } 
	protected virtual void RecoveryTick() {	} 
	protected virtual void ConcludeTick() { } 

	protected void createHitboxes()
	{
		foreach (HitboxInfo hi in m_HitboxInfo) {
			if (hi.Delay <= 0f)
				m_hitboxMaker.CreateHitbox (hi);
			else
				GetComponent<CharacterBase> ().QueueHitbox (hi, hi.Delay);
		}
	}

	void OnDrawGizmos() {
		if (m_AIInfo.DrawPredictionHitbox) {
			Gizmos.color = new Color (0, 0, 1, .25f);
			if (m_AIInfo.UniqueAIPrediction && m_hitboxMaker != null && m_hitboxMaker.GetComponent<Orientation>() != null) {
				Vector3 off = m_hitboxMaker.GetComponent<Orientation> ().OrientVectorToDirection2D (m_AIInfo.AIPredictionOffset);
				Gizmos.DrawCube (transform.position + new Vector3(off.x,off.y, off.z), new Vector3 (m_AIInfo.AIPredictionHitbox.x, m_AIInfo.AIPredictionHitbox.y, m_AIInfo.AIPredictionHitbox.z));
			} else if (m_hitboxMaker != null && m_hitboxMaker.GetComponent<Orientation>() != null)
            {
				Vector3 off = m_hitboxMaker.GetComponent<Orientation> ().OrientVectorToDirection2D(m_HitboxInfo[0].HitboxOffset);
				Gizmos.DrawCube (transform.position + new Vector3(off.x,off.y, off.z), new Vector3 (m_HitboxInfo[0].HitboxScale.x, m_HitboxInfo[0].HitboxScale.y, m_HitboxInfo[0].HitboxScale.z));
			}
		}
	}
	
    public bool IsInActiveZone(Vector3 targetPoint)
    {
        if (!m_AIInfo.UniqueAIPrediction)
        {
            m_AIInfo.AIPredictionHitbox = m_HitboxInfo[0].HitboxScale;
            m_AIInfo.AIPredictionOffset = m_HitboxInfo[0].HitboxOffset;
        }
        float dir = m_hitboxMaker.GetComponent<Orientation>().FacingLeft ? -1f : 1f;
        float xDiff = Mathf.Abs(transform.position.x + (dir * m_AIInfo.AIPredictionOffset.x) - targetPoint.x);
        float yDiff = Mathf.Abs(transform.position.y + m_AIInfo.AIPredictionOffset.y - targetPoint.y);
        float zDiff = Mathf.Abs(transform.position.z + m_AIInfo.AIPredictionOffset.z - targetPoint.z);
        if ((m_AIInfo.AIPredictionHitbox.x)/2f > xDiff &&
            (m_AIInfo.AIPredictionHitbox.y)/2f > yDiff && 
            (m_AIInfo.AIPredictionHitbox.z)/2f > zDiff)
        {
            return true;
        }
        return false;
    }
	/*protected GameObject CreateAttackFX(AttackFXInfo afxi) {
		Vector3 p = afxi.Offset + new Vector3(
			UnityEngine.Random.Range (-afxi.OffsetVariation.x / 2f, afxi.OffsetVariation.x / 2f),
			UnityEngine.Random.Range (-afxi.OffsetVariation.y / 2f, afxi.OffsetVariation.y / 2f),
			UnityEngine.Random.Range (-afxi.OffsetVariation.z / 2f, afxi.OffsetVariation.z / 2f));
		
		GameObject fx = Instantiate (afxi.EffectPrefab, transform);
		fx.transform.position = transform.position + GetComponent<Orientation>().OrientVectorToDirection (p);
		float zrot = afxi.RotationZ + UnityEngine.Random.Range (-afxi.RotationVariation / 2f, afxi.RotationVariation / 2f);
		fx.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,zrot));
		Vector3 s = afxi.Scale + new Vector3(
			UnityEngine.Random.Range (-afxi.ScaleVariation.x / 2f, afxi.ScaleVariation.x / 2f),
			UnityEngine.Random.Range (-afxi.ScaleVariation.y / 2f, afxi.ScaleVariation.y / 2f),
			UnityEngine.Random.Range (-afxi.ScaleVariation.z / 2f, afxi.ScaleVariation.z / 2f));
		//fx.transform.localScale = new Vector3 ((afxi.FlipOnX && m_physics.FacingLeft)?-s.x:s.x, s.y, s.z);

		return fx;
	}*/

	private void updateFXQueue() {
        /*Dictionary<AttackFXInfo,float> newQueue = new Dictionary<AttackFXInfo, float>();
		foreach (AttackFXInfo afi in m_queuedFX.Keys) {
			if (m_queuedFX [afi] > Time.timeSinceLevelLoad)
				CreateAttackFX (afi);
			else
				newQueue.Add (afi,m_queuedFX[afi]);
		}
		m_queuedFX = newQueue;*/
    }
}

