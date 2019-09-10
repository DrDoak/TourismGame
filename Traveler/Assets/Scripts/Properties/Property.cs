using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.UI;

public class Property : MonoBehaviour, ICustomMessageTarget
{

    public virtual void OnCreation() { }
	public virtual void OnHit(HitInfo hi, GameObject attacker) { }
	public virtual void OnHitConfirm(HitInfo myHitbox, GameObject objectHit, HitResult hr) { }
	public virtual void OnSight(Observable observedObj) { }

	public virtual void OnAttack() { }
    public virtual void OnDeath() { }
    public virtual void OnUpdate() {
        if (Timed)
        {
            Duration -= Time.deltaTime;
            if (Duration < 0f)
                RemoveSelf();
        }
    }
    //public virtual void OnCollision() { }

	public virtual void OnAttack(ActionInfo ai) { }
	public virtual void OnControllableChange(bool isControllable) { }
	public virtual void OnHitboxCreate (Hitbox hitboxCreated) {}

	public virtual void OnAddProperty() { }
	public virtual void OnRemoveProperty() {}

	public virtual void OnJump() {}

	public virtual void OnSave(CharData d) {
        d.PersistentBools[PropertyName + "_timed"] = Timed;
        d.PersistentFloats[PropertyName + "_duration"] = Duration;
    }
	public virtual void OnLoad(CharData d) {
        Timed = d.PersistentBools[PropertyName + "_timed"];
        Duration = d.PersistentFloats[PropertyName + "_duration"];
    }

	public bool Stealable = true;
	public bool Viewable = true;
	public bool Stackable = false;
	public bool InstantUse = false;
    public bool Timed = false;
    public float Duration = 0.0f;
	public string PropertyName = "";
	public virtual string DefaultPropertyName {get {return "None";}}

	[TextArea(3,5)]
	public string Description = "";

	public virtual string DefaultDescription { get { return "No description provided"; } }
	public Sprite icon;

	public void InitPropertyData() {
		if (PropertyName == "")
			PropertyName = DefaultPropertyName;
		if (Description == "")
			Description = DefaultDescription;
	}

	public void CopyPropInfo(Property p) {
		Stealable = p.Stealable;
		Viewable = p.Viewable;
		Stackable = p.Stackable;
		InstantUse = p.InstantUse;
		if (PropertyName == "")
			PropertyName = p.PropertyName;
		if (Description == "")
			Description = p.Description;
		icon = p.icon;;
	}

    protected void RemoveSelf()
    {
        GetComponent<PropertyHolder>().RequestRemoveProperty(PropertyName);
    }
}