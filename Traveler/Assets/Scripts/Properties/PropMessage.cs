using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ICustomMessageTarget : IEventSystemHandler
{
	// functions that can be called via the messaging system
	void OnCreation();
	void OnControllableChange (bool isControllable);
	void OnHit(HitInfo hi, GameObject attacker);
	void OnHitConfirm (HitInfo myHitbox, GameObject objectHit, HitResult hr);
	void OnSight(Observable observedObj);
	void OnDeath();
	void OnUpdate();
	//void OnCollision();
	void OnAttack(ActionInfo ai);
	void OnHitboxCreate (Hitbox hitboxCreated);

	void OnJump();
}