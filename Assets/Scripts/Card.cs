using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CardType
{
    CREATURE,
    SPELL,
    WEAPON
}

public enum Attribute
{
    MURLOC,
    PIRATE,
    DEMON,
    MECH
}

public class Card : MonoBehaviour
{
    // Game Variables
    public int attack;
    public int mana;
    public int health;
    public CardType basicType;
    public List<Attribute> attributes;
    public string effectString;

    private int _currentAttack;
    public int currentAttack
    {
        get
        {
            return _currentAttack;
        }
        set
        {
            _currentAttack = value;
            attackText.text = _currentAttack.ToString();
            if (_currentAttack > attack)
            {
                attackText.color = new Color(0, 200, 0);
            }
            else
            {
                attackText.color = new Color(255, 255, 255);
            }
        }
    }
    private int _currentMana;
    public int currentMana
    {
        get
        {
            return _currentMana;
        }
        set
        {
            _currentMana = value;
            manaText.text = _currentMana.ToString();
            if (_currentMana > mana)
            {
                manaText.color = new Color(200, 0, 0);
            }
            else if (_currentMana < mana)
            {
                manaText.color = new Color(0, 200, 0);
            }
            else
            {
                manaText.color = new Color(255, 255, 255);
            }
        }
    }
    private int _currentHealth;
    public int currentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;
            healthText.text = _currentHealth.ToString();
            if (_currentHealth == maxHealth && maxHealth > health)
            {
                healthText.color = new Color(0, 200, 0);
            }
            else if (_currentHealth == maxHealth)
            {
                healthText.color = new Color(255, 255, 255);
            }
            else
            {
                healthText.color = new Color(200, 0, 0);
            }
        }
    }
    private int _maxHealth;
    public int maxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }

    public Effect effectScript;

    // Internal Variables
    public int player;

    private bool exhausted = true;
	public bool Exhausted
    {
        get
        {
            return exhausted;
        }
        set
        {
            exhausted = value;
		    hoverGraphic.gameObject.SetActive (!value);
        }
    }

    private bool dragAttack = false;

    private List<Modifier> modifiers;

    private float targetX;
    private float xDistFromTarget;
    private List<Vector3> mousePositions = new List<Vector3>();

    private bool selected;

    private float forceOfPull = 20.0f;
    private float bounceFactor = 0.35f;

    public bool wasHovered = false;

    // Object References
    public Hand hand;
    public Field field;
    public Game gameMgr;

    public Sprite cardBG;
    public Sprite cardEffectBG;
    public Sprite cardBack;

    private GameObject manaSprite;
    private GameObject attackSprite;
    private GameObject healthSprite;
    private TextMesh nameText;
    private TextMesh effectText;
    private TextMesh manaText;
    private TextMesh attackText;
    private TextMesh healthText;

    private SpriteRenderer hoverGraphic;

    /*
    [SerializeField]
    private GameObject untargetableLine;
    private LineRenderer untLine;
    [SerializeField]
    private GameObject targetableLine;
    private LineRenderer tLine;
    */

	void Awake ()
	{
		Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Cards"), LayerMask.NameToLayer ("Cards"));
		xDistFromTarget = targetX - this.rigidbody2D.position.x;
		modifiers = new List<Modifier> ();
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
        manaSprite = this.transform.FindChild("ManaSprite").gameObject;
        attackSprite = this.transform.FindChild("AttackSprite").gameObject;
        healthSprite = this.transform.FindChild("HealthSprite").gameObject;
        manaText = manaSprite.transform.FindChild("Mana").GetComponent<TextMesh>();
        attackText = attackSprite.transform.FindChild("Attack").GetComponent<TextMesh>();
        healthText = healthSprite.transform.FindChild("Health").GetComponent<TextMesh>();
        nameText = this.transform.FindChild("Name").GetComponent<TextMesh>();
        effectText = this.transform.FindChild("Effect").GetComponent<TextMesh>();

		hoverGraphic = this.transform.FindChild("ParticleTexture").GetComponent<SpriteRenderer> ();
        hoverGraphic.gameObject.SetActive(false);

        if (basicType == CardType.SPELL)
        {
            attackSprite.SetActive(false);
            healthSprite.SetActive(false);
        }

        this.ResetCard();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//float cameraHeight = Camera.main.orthographicSize;
		//float cameraWidth =  Camera.main.aspect * cameraHeight;
		
		//float cameraLeft =    Camera.main.transform.position.x - cameraWidth;
		//float cameraRight =   Camera.main.transform.position.x + cameraWidth;
		//float cameraFloor =   Camera.main.transform.position.y - cameraHeight;
		//float cameraCeiling = Camera.main.transform.position.y + cameraHeight;

        if(this.hand == null && this.field == null)
		{
			this.gameObject.SetActive (false);
		}
		else if(field != null)
		{
            // Force position
			if(player == 0)
			{
				Vector3 newPosition = new Vector3(Camera.main.transform.position.x + targetX, Camera.main.transform.position.y - this.transform.collider2D.bounds.extents.y * 11.2f / 7f - 0.1f, 0);
				if (this.transform.position != newPosition)
				{
					this.transform.position = newPosition;
				}
			}
			else if(player == 1)
			{
                Vector3 newPosition = new Vector3(Camera.main.transform.position.x + targetX, Camera.main.transform.position.y + this.transform.collider2D.bounds.extents.y * 11.2f / 7f * 0.25f + 0.1f, 0);
				if (this.transform.position != newPosition)
				{
					this.transform.position = newPosition;
				}
			}
		}
		CheckDeath ();
	}

    public void ResetCard()
    {
        this.currentMana = this.mana;
        this.currentAttack = this.attack;
        this.maxHealth = this.health;
        this.currentHealth = this.health;
        this.UpdateText();
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

    public void DeckToHandTransition(Hand h)
    {
        float cameraHeight = Camera.main.orthographicSize;
        float cameraWidth = Camera.main.aspect * cameraHeight;

        float cameraRight = Camera.main.transform.position.x + cameraWidth;
        float cameraCeiling = Camera.main.transform.position.y + cameraHeight;
        float cameraFloor = Camera.main.transform.position.y - cameraHeight;

        this.hand = h;
        this.transform.parent = this.hand.transform;
        this.rigidbody2D.isKinematic = false;
        if (player == 0)
        {
            this.transform.position = new Vector2(cameraRight - this.collider2D.bounds.extents.x, cameraFloor + this.collider2D.bounds.extents.y);
            this.rigidbody2D.velocity = new Vector2(-3, 13);
            this.rigidbody2D.gravityScale = Mathf.Abs(this.rigidbody2D.gravityScale);
        }
        else
        {
            this.transform.position = new Vector2(cameraRight - this.collider2D.bounds.extents.x, cameraCeiling - this.collider2D.bounds.extents.y);
            this.rigidbody2D.velocity = new Vector2(-3, -13);
            this.rigidbody2D.gravityScale = -Mathf.Abs(this.rigidbody2D.gravityScale);
        }
    }

    public void HandToFieldTransition(Field f)
    {
        this.hand = null;
        if (this.IsCreature())
        {
            this.field = f;
            this.transform.parent = f.transform;
            this.UpdateText();
        }
        SetOnField(true);
    }

    public void FieldToHandTransition(Hand h)
    {
        this.hand = h;
        this.field = null;
        this.transform.parent = this.hand.transform;
        this.rigidbody2D.isKinematic = false;
        this.SetVisible(this.gameMgr.turnPlayer() == this.player);
        hoverGraphic.gameObject.SetActive(false);
        SetOnField(false);
    }

    public void ResetCardPositionInHand()
    {
        targetX = Camera.main.transform.position.x + (hand.CardIndex(this) - hand.CardCount() / 2f + 0.5f) * this.collider2D.bounds.extents.x;
        xDistFromTarget = targetX - this.rigidbody2D.position.x;
    }

    public void ResetCardPositionOnField()
    {
        targetX = Camera.main.transform.position.x + (field.CardIndex(this) * 2f - field.CardCount() + 1) * this.collider2D.bounds.extents.x * 1.1f;
        xDistFromTarget = targetX - this.rigidbody2D.position.x;
    }
	
	void OnMouseDown()
	{
		selected = true;
		if (field != null)
		{
			gameMgr.CardClicked(this);
		}
		if (player == gameMgr.turnPlayer())
		{
			mousePositions.Clear();
			for (int i = 0; i < 8; ++i)
			{
				mousePositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}
			this.rigidbody2D.isKinematic = true;
		}
	}

	void OnMouseUp ()
	{
		selected = false;
		if (player == gameMgr.turnPlayer ())
		{
			float cameraHeight = Camera.main.orthographicSize;
			//float cameraWidth =  Camera.main.aspect * cameraHeight;
			
			//float cameraLeft =    Camera.main.transform.position.x - cameraWidth;
			//float cameraRight =   Camera.main.transform.position.x + cameraWidth;
			float cameraFloor =   Camera.main.transform.position.y - cameraHeight;
			float cameraCeiling = Camera.main.transform.position.y + cameraHeight;

			if (hand != null)
			{
                bool distanceCondition = (this.player == 0 ? this.rigidbody2D.position.y >= cameraFloor + cameraHeight * 0.75f : this.rigidbody2D.position.y <= cameraCeiling - cameraHeight * 0.75f);
                bool targetsExist = true;
                foreach (TargetInfo tInfo in effectScript.targetInfo)
                {
                    if (!gameMgr.TargetExists(this, tInfo))
                    {
                        targetsExist = false;
                        break;
                    }
                }
                if (gameMgr.GetMana(this.player) >= this.currentMana && 
                    distanceCondition && 
                    (targetsExist || this.basicType == CardType.CREATURE))
				{
					this.rigidbody2D.isKinematic = true;
                    
					if (effectScript != null && 
                        effectScript.targetInfo.Count > 0 && 
                        targetsExist)
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

	public bool IsCreature()
	{
		return this.basicType == CardType.CREATURE;
	}

	public void TriggerPlayed(Card c)
	{
        effectScript.TriggerPlayed(gameMgr, this, c);
	}

	public void TriggerBattlecry(List<Target> targets)
	{
        effectScript.TriggerBattlecry(gameMgr, this, targets);
	}

	public void TriggerEndTurn()
	{
        effectScript.TriggerEndTurn(gameMgr, this);
        foreach (Modifier m in modifiers)
        {
            m.EndTurn();
        }
	}

	public void TriggerStartTurn()
    {
        if (gameMgr.turnPlayer() == player)
        {
            hoverGraphic.gameObject.SetActive(true);
            hoverGraphic.color = new Color(0, 255, 0);
            exhausted = false;
        }
        else
        {
            hoverGraphic.gameObject.SetActive(false);
        }
        effectScript.TriggerStartTurn(gameMgr, this);
	}
	
	public void TriggerCombat (bool isAttacker, Card other)
	{
        effectScript.TriggerCombat(gameMgr, this, isAttacker, other);
	}

	public void TriggerDirectAttack(int player)
	{
        effectScript.TriggerDirectAttack(gameMgr, this, player);
	}


	public void SetVisible(bool visible)
    {
        manaSprite.SetActive(visible);
        if (basicType != CardType.SPELL)
        {
            attackSprite.SetActive(visible);
            healthSprite.SetActive(visible);
        }
        this.transform.FindChild("CardBack").gameObject.SetActive(!visible);
	}

    public void SetOnField(bool onField)
    {
        // Could call SetVisible(true) for redundancy purposes.
        if (onField)
        {
            attackSprite.transform.localPosition = new Vector3(-3.25f, -0.5f, -0.0002f);
            healthSprite.transform.localPosition = new Vector3(3.25f, -0.5f, -0.0002f);
            this.GetComponent<BoxCollider2D>().center = new Vector2(0f, 2.1f);
            this.GetComponent<BoxCollider2D>().size = new Vector2(8f, 7f);
        }
        else
        {
            attackSprite.transform.localPosition = new Vector3(-3.25f, -4.75f, -0.0002f);
            healthSprite.transform.localPosition = new Vector3(3.25f, -4.75f, -0.0002f);
            this.GetComponent<BoxCollider2D>().size = new Vector2(8f, 11.2f);
            this.GetComponent<BoxCollider2D>().center = new Vector2(0f, -2.1f);
        }
        manaSprite.SetActive(!onField);

        this.transform.FindChild("Border").gameObject.SetActive(!onField);
        this.transform.FindChild("CreatureBorder").gameObject.SetActive(onField);
        this.transform.FindChild("EffectBackground").gameObject.SetActive(!onField);
        this.transform.FindChild("Name").gameObject.SetActive(!onField);
        this.transform.FindChild("Effect").gameObject.SetActive(!onField);
        this.GetComponent<SpriteRenderer>().enabled = !onField;
    }

	public void AddModifier(Modifier m)
	{
		modifiers.Add (m);
		m.Apply ();

        manaText.text = currentMana.ToString();
		attackText.text = currentAttack.ToString ();
		healthText.text = currentHealth.ToString ();
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

    public void UpdateText()
    {
        this.manaText.text = this.currentMana.ToString();
		this.attackText.text = this.currentAttack.ToString ();
        this.healthText.text = this.currentHealth.ToString();
        this.nameText.text = this.name;
        this.effectText.text = this.effectString;
        this.WrapText();
    }

    public void WrapText()
    {
        string builder = "";
        string text = effectText.text;
        effectText.text = "";
        float rowLimit = 1f;
        string[] parts = text.Split(' ');
        for (int i = 0; i < parts.Length; i++)
        {
            effectText.text += parts[i] + " ";
            if (effectText.renderer.bounds.extents.x > rowLimit)
            {
                effectText.text = builder.TrimEnd() + System.Environment.NewLine + parts[i] + " ";
            }
            builder = effectText.text;
        }
    }
}











