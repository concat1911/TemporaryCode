namespace VeryDisco.MFPS
{
    using FishNet;
    using UnityEngine;
    using FishNet.Object;
    using FishNet.Object.Synchronizing;
    using Shooter;
    
    public class PlayerInstance : NetworkBehaviour
    {
        [SyncVar(OnChange = nameof(on_name_changed))] 
        public string playerName;
        [SyncVar(OnChange = nameof(on_ready_changed))] 
        public bool isReady;
        [SyncVar] public bool isGameplaySceneLoaded;
        [SerializeField] private GameObject prefab; // controller prefab
        [SerializeField] private PlayerFPS playerFPS;

        public string PlayerName => playerName;
        public System.Action OnReadyChanged;
        public System.Action OnNameChanged;
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (Owner.IsLocalClient)
            {
                SetName();
                GameNetworkManager.instance.RegisterLocalPlayer(this);
            }

            GameNetworkManager.instance.Register(this);
            
            Debug.Log(playerName + " is connected");
        }
        
        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            GameNetworkManager.instance.UnRegister(this);
        }
        
        [ServerRpc]
        public void SpawnController()
        {
            if (IsOwner == false) return;
            
            Debug.Log("Spawn controller for " + playerName);

            GameObject controllerObj = Instantiate(prefab, transform);
            InstanceFinder.ServerManager.Spawn(controllerObj, Owner);
        }

        [ServerRpc]
        public void WaitingRoomReadyToggle()
        {
            isReady = !isReady;
        }

        private void SetName()
        {
            playerName = MenuUI.instance.HostAndJoinUI.PlayerNameInput;
        }

        private void on_ready_changed(bool prev, bool next, bool asServer)
        {
            OnReadyChanged?.Invoke();
        }
        
        private void on_name_changed(string prev, string next, bool asServer)
        {
            OnNameChanged?.Invoke();
        }
    }
}
