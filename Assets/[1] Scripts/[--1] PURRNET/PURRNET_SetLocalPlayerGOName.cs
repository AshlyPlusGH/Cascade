using PurrNet;
using UnityEngine;

public class PURRNET_SetLocalPlayerGOName : NetworkBehaviour
{
    [SerializeField] private bool debug;
        private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

    [Space(10)]

    [SerializeField] private bool var1;

    //Not Working for Clients
    protected override void OnSpawned(){ if (isOwner) gameObject.name = "Local Player"; }
}