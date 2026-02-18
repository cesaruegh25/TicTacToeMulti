using UnityEngine;
using Fusion;

public class NetworkStarted : MonoBehaviour
{
    public NetworkRunner runnerPrefab;

    public NetworkObject ticTtacToePrefabs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Start()
    {
        var runner = Instantiate(runnerPrefab);
        runner.ProvideInput = false;
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "TicTacToeRoom",
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        });

        if (runner.IsSharedModeMasterClient)
        {
            runner.Spawn(ticTtacToePrefabs);
        }

    }

}
