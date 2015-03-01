using UnityEngine;
using System.Collections;

public class ManaBar : MonoBehaviour
{
    public int maxMana = 1;

    private TextMesh text;

    private int _currentMana = 1;
    public int currentMana
    {
        get
        {
            return _currentMana;
        }
        set
        {
            _currentMana = value;
            if (_currentMana < 0)
            {
                Debug.LogError("Cannot spend more mana than you have!");
                _currentMana = 0;
            }
            else if (_currentMana > 10)
            {
                _currentMana = 10;
            }
            this.UpdateText();
        }
    }

    void Awake()
    {
        this.text = GetComponentInChildren<TextMesh>();
    }

	void Start()
    {
        this.UpdateText();
	}

    void UpdateText()
    {
        this.text.text = "Mana: " + currentMana.ToString() + "/" + maxMana.ToString();
    }
}
