using NaughtyAttributes;
using UnityEngine;

public class PLAYER_DEBUG : MonoBehaviour
{
    [SerializeField] private Player target;

    [Button] public void DEBUG_SetTurnOrderFirstPlayer(){ target.DEBUG_SetTurnOrderFirstThis(); }
}