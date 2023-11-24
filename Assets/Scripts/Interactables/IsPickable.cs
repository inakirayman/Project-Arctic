using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPickable : IsInteractable
{
    
    public override void Interact(PlayerInfo playerInfo)
    { 
        Debug.Log($"Picking up the {gameObject.name} by playerID {playerInfo.PlayerID}");

        transform.position = playerInfo.Hands.position;
        transform.parent = playerInfo.Hands;

    }

   
}
