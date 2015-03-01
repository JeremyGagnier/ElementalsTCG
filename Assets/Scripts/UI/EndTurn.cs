using UnityEngine;
using System.Collections;

public class EndTurn : MonoBehaviour
{
    public Game gameMgr;

    void OnMouseDown()
    {
        gameMgr.EndTurn();
    }
}
