using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private int _playerID = 0;
    public int PlayerID => _playerID;

    public Transform Hands;


}
