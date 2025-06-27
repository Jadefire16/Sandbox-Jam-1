using Sandbox;
using Sandbox.Network;
using System;
public sealed class SceneManager : Component, Component.INetworkListener, ISceneLoadingEvents
{
	public static SceneManager Instance { get; set; }

	[Property] public SceneFile MainMenuScene { get; set; }
	[Property] public List<SceneFile> ArenaScenes { get; set; }

	public void AfterLoad(Scene scene)
	{
		SceneInformation info = scene.GetAllComponents<SceneInformation>().FirstOrDefault();
		if ( info.SceneTags.Contains("Init") )
			LoadScene(MainMenuScene);
	}

	public void LoadArenaScene()
	{
		int index = Random.Shared.Next(0, ArenaScenes.Count - 1);
		LoadScene(ArenaScenes[index], true);
	}

	public void LoadScene(SceneFile scene, bool callRPC = false)
	{
		Scene.Load(scene);
		if (callRPC && Networking.IsHost)
		{
			RPC_LoadScene(scene);
		}
	}

	[Rpc.Broadcast]
	public void RPC_LoadScene(SceneFile scene)
	{
		if ( Networking.IsHost || IsProxy )
			return;
		Scene.Load(scene);
	}

	protected override void OnAwake()
	{
		base.OnAwake();
		Log.Info("Awake");

		if ( Instance is not null && Instance != this )
		{
			GameObject.Destroy();
			Log.Warning($"Duplicate instance of {nameof(SceneManager)}");
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
