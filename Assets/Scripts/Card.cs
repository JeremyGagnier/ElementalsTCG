﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour
{
	public int player;
	
	private Hand hand;
	private Field field;
	private Game gameMgr;

	private float targetX;
	private float xDistFromTarget;
	private List<Vector3> mousePositions = new List<Vector3>();

	private bool selected;

	[SerializeField]
	private float forceOfPull;
	[SerializeField]
	private float bounceFactor;
	[SerializeField]
	private bool isCreature;

	public Sprite cardImg;
	public Sprite creatureImg;
	public Sprite cardBackImg;

	[SerializeField]
	private Effects effectScript;

	private List<Modifier> modifiers;

	[SerializeField]
	private int attack;
	[SerializeField]
	private int retaliation;
	[SerializeField]
	private int health;

	[HideInInspector]
	public int maxHealth;
	[HideInInspector]
	public int currentAttack;
	[HideInInspector]
	public int currentRetaliation;
	[HideInInspector]
	public int currentHealth;

	public bool exhausted = true;
	private bool dragAttack = false;

	private TextMesh attackText;
	private TextMesh retaliationText;
	private TextMesh healthText;
	
	[SerializeField]
	private GameObject untargetableLine;
	private LineRenderer untLine;
	[SerializeField]
	private GameObject targetableLine;
	private LineRenderer tLine;

	private SpriteRenderer hoverGraphic;

	public bool wasHovered = false;

	public int discards = 0;
	public int glance = 0;
	public int toughness = 0;

	void Awake ()
	{
		Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Cards"), LayerMask.NameToLayer ("Cards"));
		xDistFromTarget = targetX - this.rigidbody2D.position.x;
		modifiers = new List<Modifier> ();

		maxHealth = health;
		currentAttack = attack;
		currentRetaliation = retaliation;
		currentHealth = health;
	}

	void Start ()
	{
		float cameraHeight = Camera.main.orthographicSize;
		float cameraWidth =  Camera.main.aspect * cameraHeight;

		float cameraRight =   Camera.main.transform.position.x + cameraWidth;
		//float cameraCeiling = Camera.main.transform.position.y + cameraHeight;

		hand = GetComponentInParent<Hand> ();
		if (hand != null) {
			targetX = Camera.main.transform.position.x + (hand.CardIndex (this) - hand.CardCount () / 2f) * this.collider2D.bounds.extents.x;
		} else {
			this.rigidbody2D.isKinematic = true;
			this.transform.position = new Vector2(cameraRight + this.collider2D.bounds.extents.x * 999f, 0);
		}

		attackText = this.transform.GetChild (0).GetComponent<TextMesh> ();
		retaliationText = this.transform.GetChild (1).GetComponent<TextMesh> ();
		healthText = this.transform.GetChild (2).GetComponent<TextMesh> ();

		if (isCreature)
		{
			attackText.text = currentAttack.ToString ();
			retaliationText.text = currentRetaliation.ToString ();
			healthText.text = currentHealth.ToString ();
		}
		else
		{
			attackText.text = "";
			retaliationText.text = "";
			healthText.text = "";
		}

		hoverGraphic = this.transform.GetChild (3).GetComponent<SpriteRenderer> ();
		hoverGraphic.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		float cameraHeight = Camera.main.orthographicSize;
		float cameraWidth =  Camera.main.aspect * cameraHeight;
		
		float cameraLeft =    Camera.main.transform.position.x - cameraWidth;
		float cameraRight =   Camera.main.transform.position.x + cameraWidth;
		float cameraFloor =   Camera.main.transform.position.y - cameraHeight;
		float cameraCeiling = Camera.main.transform.position.y + cameraHeight;

		if (this.rigidbody2D == null)
		{
			return;
		}
		
		if (hand == null && field == null)
		{
			this.gameObject.SetActive (false);
		}
		else if (field != null)
		{
			if (player == 0)
			{
				Vector3 newPosition = new Vector3(Camera.main.transform.position.x + targetX, Camera.main.transform.position.y - this.transform.collider2D.bounds.extents.y * 1.1f, 0);
				if (this.transform.position != newPosition)
				{
					this.transform.position = newPosition;
				}
			}
			else if (player == 1)
			{
				Vector3 newPosition = new Vector3(Camera.main.transform.position.x + targetX, Camera.main.transform.position.y + this.transform.collider2D.bounds.extents.y * 1.1f, 0);
				if (this.transform.position != newPosition)
				{
					this.transform.position = newPosition;
				}
			}
		}
		CheckDeath ();
	}

	void FixedUpdate()
	{
		float cameraHeight = Camera.main.orthographicSize;
		float cameraWidth =  Camera.main.aspect * cameraHeight;
		
		float cameraLeft =    Camera.main.transform.position.x - cameraWidth;
		float cameraRight =   Camera.main.transform.position.x + cameraWidth;
		float cameraFloor =   Camera.main.transform.position.y - cameraHeight;
		float cameraCeiling = Camera.main.transform.position.y + cameraHeight;

		if (hand != null)
		{
			float currentDistFromTarget = targetX - this.rigidbody2D.position.x;
			
			if (this.rigidbody2D.position.x - this.collider2D.bounds.extents.x < cameraLeft && this.rigidbody2D.velocity.x < 0f)
			{
				this.rigidbody2D.velocity = new Vector2(-this.rigidbody2D.velocity.x * bounceFactor, this.rigidbody2D.velocity.y);
				this.transform.position = new Vector3(cameraLeft + this.collider2D.bounds.extents.x, this.rigidbody2D.position.y, 0f);
				xDistFromTarget = targetX - this.rigidbody2D.position.x;
			}
			else if(this.rigidbody2D.position.x + this.collider2D.bounds.extents.x > cameraRight && this.rigidbody2D.velocity.x > 0f)
			{
				this.rigidbody2D.velocity = new Vector2(-this.rigidbody2D.velocity.x * bounceFactor, this.rigidbody2D.velocity.y);
				this.transform.position = new Vector3(cameraRight - this.collider2D.bounds.extents.x, this.rigidbody2D.position.y, 0f);
				xDistFromTarget = targetX - this.rigidbody2D.position.x;
			}
			
			if (this.rigidbody2D.position.y - this.collider2D.bounds.extents.y < cameraFloor && this.rigidbody2D.velocity.y < 0f)
			{
				this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, -this.rigidbody2D.velocity.y * bounceFactor);
				this.transform.position = new Vector3(this.rigidbody2D.position.x, cameraFloor + this.collider2D.bounds.extents.y, 0f);
			}
			else if (this.rigidbody2D.position.y + this.collider2D.bounds.extents.y > cameraCeiling && this.rigidbody2D.velocity.y > 0f)
			{
				this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, -this.rigidbody2D.velocity.y * bounceFactor);
				this.transform.position = new Vector3(this.rigidbody2D.position.x, cameraCeiling - this.collider2D.bounds.extents.y, 0f);
			}
			
			if (Mathf.Abs (currentDistFromTarget) < 0.04f)
			{
				if (this.rigidbody2D.velocity.x != 0f)
				{
					this.rigidbody2D.velocity = new Vector2(0f, this.rigidbody2D.velocity.y);
				}
			}
			else if (Mathf.Abs (currentDistFromTarget) <= Mathf.Abs (xDistFromTarget/2f) && Mathf.Sign (this.rigidbody2D.velocity.x) == Mathf.Sign (currentDistFromTarget))
			{
				Vector2 stoppingForce = new Vector2(-this.rigidbody2D.mass * 
				                                    this.rigidbody2D.velocity.x * 
				                                    this.rigidbody2D.velocity.x / 
				                                    (currentDistFromTarget * 2f), 0);
				this.rigidbody2D.AddForce (stoppingForce, ForceMode2D.Force);
			}
			else
			{
				Vector2 pullForce = new Vector2(Mathf.Min (Mathf.Sqrt (Mathf.Abs (currentDistFromTarget)),100), 0f) * forceOfPull * Mathf.Sign (currentDistFromTarget);
				this.rigidbody2D.AddForce (pullForce, ForceMode2D.Force);
			}

			if (selected)
			{
				this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);
			}
			else
			{
				this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -this.transform.position.x/8f);
			}
		}
	}
	
	void OnMouseDown ()
	{
		selected = true;
		if (field != null)
		{
			gameMgr.CardClicked (this);
		}
		if (player == gameMgr.turnPlayer ())
		{
			mousePositions.Clear ();
			for (int i = 0; i < 8; ++i)
			{
				mousePositions.Add (Camera.main.ScreenToWorldPoint (Input.mousePosition));
			}

			if (this.rigidbody2D != null)
			{
				this.rigidbody2D.isKinematic = true;
			}
		}
	}

	void OnMouseUp ()
	{
		selected = false;
		if (player == gameMgr.turnPlayer ())
		{
			float cameraHeight = Camera.main.orthographicSize;
			float cameraWidth =  Camera.main.aspect * cameraHeight;
			
			float cameraLeft =    Camera.main.transform.position.x - cameraWidth;
			float cameraRight =   Camera.main.transform.position.x + cameraWidth;
			float cameraFloor =   Camera.main.transform.position.y - cameraHeight;
			float cameraCeiling = Camera.main.transform.position.y + cameraHeight;

			if (hand != null)
			{
				bool distanceCondition = (player == 0 ? rigidbody2D.position.y >= cameraFloor + cameraHeight * 0.75f : rigidbody2D.position.y <= cameraCeiling - cameraHeight * 0.75f);
				if (gameMgr.hasMorePlays () && distanceCondition && hand.CardCount() >= discards + 1)
				{
					this.rigidbody2D.isKinematic = true;
					if (effectScript.targetInfo.Count > 0)
					{
						gameMgr.GetTargets (this, effectScript.targetInfo.Count);
					}
					else
					{
						gameMgr.Play (this);
					}
				}
				else
				{
					gameMgr.StopGettingTargets ();
					this.rigidbody2D.isKinematic = false;
					List<Vector3> mouseDifferences = new List<Vector3> ();
					for (int i = 0; i < mousePositions.Count - 1; ++i)
					{
						mouseDifferences.Add (mousePositions [i + 1] - mousePositions [i]);
					}
					Vector3 diffAvg = mouseDifferences [0];
					for (int i = 1; i < mouseDifferences.Count; ++i)
					{
						diffAvg += mouseDifferences [i];
					}
					diffAvg /= mouseDifferences.Count;
					this.rigidbody2D.AddForce (new Vector2 (diffAvg.x, diffAvg.y) * 25.0f, ForceMode2D.Impulse);

					xDistFromTarget = targetX - this.rigidbody2D.position.x;
				}
			}
			else if (field != null)
			{
				if (dragAttack)
				{
					Target target = gameMgr.EnemyField (player).GetHoverTarget ();
					if (target != null)
					{
						if (!exhausted && target.isCard && target.card.player != this.player)
						{
							gameMgr.Combat (this, target.card);
						}
						else if (!exhausted && !target.isCard && target.player != this.player)
						{
							gameMgr.Combat (this, target.player);
						}
					}
				}
			}
		}
	}

	void OnMouseDrag()
	{
		if (player == gameMgr.turnPlayer ())
		{
			if (hand != null)
			{
				Vector3 worldMousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				this.transform.position = new Vector3 (worldMousePos.x, worldMousePos.y, this.transform.position.z);
				mousePositions.RemoveAt (0);
				mousePositions.Add (worldMousePos);
			}
			else if (field != null)
			{
				dragAttack = true;
			}
		}
	}

	public void OnMouseEnterHack()
	{
		if (hand != null)
		{
			if (player == gameMgr.turnPlayer ())
			{
				hand.CardEntered (this);
			}
		}
		else if (field != null)
		{
			if (gameMgr.turnPlayer () != player || exhausted)
			{
				hoverGraphic.gameObject.SetActive (true);
				hoverGraphic.color = new Color (255, 0, 0);
			}
			
			if (cardImg != null)
			{
				this.GetComponent<SpriteRenderer> ().sprite = cardImg;
			}
		}
	}

	public void OnMouseExitHack()
	{
		if (hand != null)
		{
			if (player == gameMgr.turnPlayer ())
			{
				hand.CardExited (this);
			}
		}
		else if (field != null)
		{
			field.CardExited (this);
			if (!exhausted && gameMgr.turnPlayer () == player)
			{
				hoverGraphic.gameObject.SetActive (true);
				hoverGraphic.color = new Color (0, 255, 0);
			}
			else
			{
				hoverGraphic.gameObject.SetActive (false);
			}

			if (creatureImg != null)
			{
				this.GetComponent<SpriteRenderer> ().sprite = creatureImg;
			}
		}
	}

	public void OnMouseOverHack()
	{
		if (field != null)
		{
			field.CardEntered (this);
		}
	}

	public void CardEntered (int index)
	{
		int myIndex = hand.CardIndex (this);
		if (index < myIndex)
		{
			targetX = Camera.main.transform.position.x + (myIndex - hand.CardCount () / 2f + 1.5f) * this.collider2D.bounds.extents.x;
			xDistFromTarget = targetX - this.rigidbody2D.position.x;
		}
		else
		{
			targetX = Camera.main.transform.position.x + (myIndex - hand.CardCount () / 2f + 0.5f) * this.collider2D.bounds.extents.x;
		}
	}

	public void ResetCardPositionInHand ()
	{
		targetX = Camera.main.transform.position.x + (hand.CardIndex (this) - hand.CardCount () / 2f + 0.5f) * this.collider2D.bounds.extents.x;
		xDistFromTarget = targetX - this.rigidbody2D.position.x;
	}

	public void ResetCardPositionOnField ()
	{
		targetX = Camera.main.transform.position.x + (field.CardIndex (this) * 2f - field.CardCount () + 1) * this.collider2D.bounds.extents.x;
		xDistFromTarget = targetX - this.rigidbody2D.position.x;
	}

	public void DrawCard(Hand h)
	{
		float cameraHeight = Camera.main.orthographicSize;
		float cameraWidth =  Camera.main.aspect * cameraHeight;
		
		float cameraRight =   Camera.main.transform.position.x + cameraWidth;
		float cameraCeiling = Camera.main.transform.position.y + cameraHeight;
		float cameraFloor =   Camera.main.transform.position.y - cameraHeight;

		hand = h;
		this.transform.parent = hand.transform;
		this.rigidbody2D.isKinematic = false;
		if (player == 0)
		{
			this.transform.position = new Vector2 (cameraRight - this.collider2D.bounds.extents.x, cameraFloor + this.collider2D.bounds.extents.y);
			this.rigidbody2D.velocity = new Vector2 (-3, 13);
		}
		else
		{
			this.transform.position = new Vector2 (cameraRight - this.collider2D.bounds.extents.x, cameraCeiling - this.collider2D.bounds.extents.y);
			this.rigidbody2D.velocity = new Vector2 (-3, -13);
		}
	}

	public void Exhaust ()
	{
		exhausted = true;
		hoverGraphic.gameObject.SetActive (false);
	}

	public void SetPlayer (int p)
	{
		player = p;
		if (player == 1)
		{
			this.rigidbody2D.gravityScale = -this.rigidbody2D.gravityScale;
		}
	}

	public bool IsCreature()
	{
		return isCreature;
	}

	public void SetGameMgr (Game game)
	{
		gameMgr = game;
	}

	public void Play (Field f)
	{
		hand = null;
		if (isCreature)
		{
			field = f;
			this.transform.parent = f.transform;
			if (creatureImg != null)
			{
				this.GetComponent<SpriteRenderer> ().sprite = creatureImg;
			}
			attackText.text = currentAttack.ToString ();
			retaliationText.text = currentRetaliation.ToString ();
			healthText.text = currentHealth.ToString ();
		}
	}

	public void TriggerPlayed(Card c)
	{
		effectScript.TriggerPlayed (gameMgr, this, c);
	}

	public void TriggerBattlecry(List<Target> targets)
	{
		effectScript.TriggerBattlecry (gameMgr, this, targets);
	}

	public void TriggerEndTurn()
	{

		effectScript.TriggerEndTurn (gameMgr, this);
		foreach (Modifier m in modifiers)
		{
			m.EndTurn ();
		}
	}

	public void TriggerStartTurn()
	{
		if (gameMgr.turnPlayer() == player)
		{
			hoverGraphic.gameObject.SetActive (true);
			hoverGraphic.color = new Color (0, 255, 0);
			exhausted = false;
		}
		else
		{
			hoverGraphic.gameObject.SetActive (false);
		}
		effectScript.TriggerStartTurn (gameMgr, this);
	}
	
	public void TriggerCombat (bool isAttacker, Card other)
	{
		effectScript.TriggerCombat (gameMgr, this, isAttacker, other);
	}

	public void TriggerDirectAttack (int player)
	{
		effectScript.TriggerDirectAttack (gameMgr, this, player);
	}


	public void SetHandVisible (bool visible)
	{
		if (visible && cardImg != null)
		{
			GetComponent<SpriteRenderer> ().sprite = cardImg;
			if (isCreature)
			{
				attackText.text = currentAttack.ToString ();
				retaliationText.text = currentRetaliation.ToString ();
				healthText.text = currentHealth.ToString ();
			}
		}
		if (!visible && cardBackImg != null)
		{
			GetComponent<SpriteRenderer> ().sprite = cardBackImg;
			attackText.text = "";
			retaliationText.text = "";
			healthText.text = "";
		}
	}

	public void AddModifier(Modifier m)
	{
		modifiers.Add (m);
		m.Apply ();

		attackText.text = currentAttack.ToString ();
		retaliationText.text = currentRetaliation.ToString ();
		healthText.text = currentHealth.ToString ();

		if (currentRetaliation > retaliation)
		{
			retaliationText.color = new Color (0, 200, 0);
		}
		else
		{
			retaliationText.color = new Color(0, 0, 0);
		}

		if (currentAttack > attack)
		{
			attackText.color = new Color (0, 200, 0);
		}
		else
		{
			attackText.color = new Color(0, 0, 0);
		}

		if (currentHealth == maxHealth && maxHealth > health)
		{
			healthText.color = new Color (0, 200, 0);
		}
		else if (currentHealth == maxHealth)
		{
			healthText.color = new Color (0, 0, 0);
		}
		else
		{
			healthText.color = new Color(200, 0, 0);
		}
	}

	public void CheckDeath()
	{
		if (currentHealth <= 0)
		{
			gameMgr.Destroy (this);
			if (field != null)
			{
				field.CardExited (this);
				field = null;
			}
		}
	}

	public bool IsTargetValid(Target t)
	{
		return effectScript.IsTargetValid (t.player == player, t.isCard);
	}

	public void PickUp (Hand h)
	{
		hand = h;
		field = null;
		this.transform.parent = hand.transform;
		this.rigidbody2D.isKinematic = false;
		this.SetHandVisible (gameMgr.turnPlayer () == player);
		hoverGraphic.gameObject.SetActive (false);
	}

	public void SetField (Field f)
	{
		this.field = f;
		this.hand = null;
	}
}











