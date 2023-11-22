using System;
using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;

namespace VeryDisco.MFPS
{
	using System.Collections.Generic;
	using UnityEngine;

	public class GameNetworkManager : NetworkBehaviour
	{
		public static GameNetworkManager instance { get; private set; }
		public PlayerInstance localPlayerInstance;
		[SerializeField] private List<PlayerInstance> activePlayers = new();
		public List<PlayerInstance> ActivePlayers => activePlayers;

		public Action<PlayerInstance> OnPlayerConnected;
		public Action<PlayerInstance> OnPlayerDisconnected;
		
		public bool IsAllPlayerReady()
		{
			bool valid = true;

			for (int i = 0; i < activePlayers.Count; i++)
			{
				if (activePlayers[i].isReady == false)
				{
					valid = false;
					break;
				}
			}

			return valid;
		}

		private void Awake()
		{
			if (instance != null)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;
		}
		
		public void RegisterLocalPlayer(PlayerInstance player)
		{
			localPlayerInstance = player;
		}
		
		public void Register(PlayerInstance player)
		{
			if (activePlayers.Contains(player)) return;
			
			activePlayers.Add(player);
			OnPlayerConnected?.Invoke(player);
		}

		public void UnRegister(PlayerInstance player)
		{
			if (activePlayers.Contains(player))
			{
				activePlayers.Remove(player);
				OnPlayerDisconnected?.Invoke(player);
			}
		}

		public void LoadLevel()
		{
			LoadScence("LevelTest");
			UnLoadScene("Hub");

			InstanceFinder.SceneManager.OnLoadEnd += OnLoadEnd;
		}

		void OnLoadEnd(SceneLoadEndEventArgs args)
		{
			InstanceFinder.SceneManager.OnLoadEnd -= OnLoadEnd;

			localPlayerInstance.SpawnController();
		}

		void LoadScence(string sceneName)
		{
			if (IsServer == false) return;
			
			SceneLoadData sceneLoadData = new(sceneName);
			InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);
		}

		void UnLoadScene(string sceneName)
		{
			if (IsServer == false) return;
			SceneUnloadData sceneLoadData = new(sceneName);
			InstanceFinder.SceneManager.UnloadGlobalScenes(sceneLoadData);
		}
	}
}


