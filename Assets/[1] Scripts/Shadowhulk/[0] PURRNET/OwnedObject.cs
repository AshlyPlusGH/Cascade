using PurrNet;

public class OwnedObject : NetworkBehaviour
{
    public void SetPlayerID(PlayerID? newPlayerID){ GiveOwnership(newPlayerID); }
}