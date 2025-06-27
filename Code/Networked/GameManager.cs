using Sandbox;
using Sandbox.Network;
using System;
public sealed class GameManager : Component, Component.INetworkListener, ISceneLoadingEvents
{
	public const string GameStateKey = "game_state";
	public static GameManager Instance { get; set; }
	[Property] public int MaxLobbyMembers { get; set; } = 8;


	public void AfterLoad(Scene scene)
	{
		SceneInformation info = scene.GetAllComponents<SceneInformation>().FirstOrDefault();
		if (info.SceneTags.Contains("Arena"))
			LoadSpawnPoints();
	}

	private void LoadSpawnPoints()
	{
		if(!Networking.IsHost)
			return;
		Log.Info("Loading Spawn Points");
	}

	public void InitializeNetworking()
	{
		Log.Info("Hosting Lobby...");

		var config = new LobbyConfig()
		{
			Hidden = true,
			Name = "RepWarLobby",
			MaxPlayers = MaxLobbyMembers,
			Privacy = LobbyPrivacy.Public
		};

		Networking.CreateLobby(config);
		Networking.SetData(GameStateKey, GameState.Starting.ToString());
	}

	protected override void OnAwake()
	{
		base.OnAwake();
		Log.Info("Awake");

		if(Instance is not null && Instance != this)
		{
			GameObject.Destroy();
			Log.Warning($"Duplicate instance of {nameof(GameManager)}");
		}
		GameObject.Flags |= GameObjectFlags.DontDestroyOnLoad;
		Instance = this;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Log.Info("Destroy");
		Instance = null;
	}
}


public enum GameState
{
	Starting,
	InProgress,
	Finishing,
	Finished
}
