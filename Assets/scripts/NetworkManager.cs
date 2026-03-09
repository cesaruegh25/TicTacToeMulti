using UnityEngine;
using Fusion;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using Fusion.Sockets;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner runnerPrefab;
    public static NetworkRunner runnerIstance;
    public NetworkObject ticTtacToePrefabs;

    private List<SessionInfo> sessionList =new List<SessionInfo>();

    [SerializeField] private string lobbyName = "default";
    [SerializeField] private Transform sessionListContentParent;
    [SerializeField] private GameObject sessionListEntryPrefab;
    [SerializeField] private Dictionary<string, GameObject> sessionListUIDictionary = new Dictionary<string, GameObject>();

    [SerializeField] private SceneAsset gameScene;
    [SerializeField] private SceneAsset lobbyScene;
    //[SerializeField] private GameObject playerPrefab;


    private void Start()
    {
        DontDestroyOnLoad(this);
        runnerIstance = Instantiate(runnerPrefab);
        /* anteriormente se usaba para crear la sala y cargar la escena, pero ahora se hace desde el lobby
        runnerIstance.ProvideInput = false;
        await runnerIstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "TicTacToeRoom",
            SceneManager = runnerIstance.GetComponent<NetworkSceneManagerDefault>()
        });

        if (runnerIstance.IsSharedModeMasterClient)
        {
            runnerIstance.Spawn(ticTtacToePrefabs);
        }*/

        runnerIstance.AddCallbacks(this);

        // conexion con el servidor

        runnerIstance.JoinSessionLobby(SessionLobby.Shared, lobbyName);

    }

    public static void ReturnToLobby()
    {
        //despawn del objeto jugador
        //runnerIstance.Despawn(runnerIstance.GetPlayerObject(runnerIstance.LocalPlayer));

        // desconectarse del servidor y volver al lobby
        runnerIstance.Shutdown(true, ShutdownReason.Ok);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        SceneManager.LoadScene(lobbyScene.name);
    }

    public void CreateRandomSession()
    {
        int randomInt = Random.Range(1000, 9999);
        string randomSessionName = "room_" + randomInt.ToString();

        runnerIstance.StartGame(new StartGameArgs()
        {
            Scene = SceneRef.FromIndex(GetSceneIndex(gameScene.name)),
            SessionName = randomSessionName,
            GameMode = GameMode.Shared,
            PlayerCount = 2, // Limitar a 2 jugadores para el tic-tac-toe
            IsVisible = true // La sala será visible en la lista de salas del lobby
        });
    }

    private int GetSceneIndex(string name)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == name)
            {
                return i;
            }
        }
        return -1;
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // Eliminar listado
        DeleteOldSessionFromUI(sessionList);

        // Volvemos a generarlo
        CompareLists(sessionList);
    }

    private void CompareLists(List<SessionInfo> sessionList)
    {
        foreach (SessionInfo session in sessionList)
        {
            if (!sessionListUIDictionary.ContainsKey(session.Name)) // tenemos la sesion en el diccionario
            {
                UpdateEntryUI(session);
            }
            else // no tenemos la sesion en el diccionario, hay que crearla
            {
                CreateEntryUI(session);
            }
        }
    }

    private void CreateEntryUI(SessionInfo session)
    {
        GameObject newEntry = Instantiate(sessionListEntryPrefab);
        newEntry.transform.parent = sessionListContentParent;
        SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();
        sessionListUIDictionary.Add(session.Name, newEntry);

        entryScript.roomName.text = session.Name;
        entryScript.playerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
        entryScript.joinButton.interactable = session.IsOpen;

        newEntry.SetActive(session.IsVisible);

    }

    private void UpdateEntryUI(SessionInfo session)
    {
        sessionListUIDictionary.TryGetValue(session.Name, out GameObject newEntry);
        SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();
        entryScript.roomName.text = session.Name;
        entryScript.playerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
        entryScript.joinButton.interactable = session.IsOpen;

        newEntry.SetActive(session.IsVisible);


    }

    private void DeleteOldSessionFromUI(List<SessionInfo> sessionList)
    {
        bool isContained = false;
        GameObject uiToDelete = null;
        foreach (KeyValuePair<string, GameObject> kvp in sessionListUIDictionary)
        {
            string sessionKey = kvp.Key;
            foreach (SessionInfo sessionInfo in sessionList)
            {
                if (sessionKey == sessionInfo.Name)
                {
                    isContained = true;
                    break;
                }
            }

            if (!isContained)
            {
                uiToDelete = kvp.Value;
                sessionListUIDictionary.Remove(sessionKey);
                Destroy(uiToDelete);
            }
        }
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // si utilizamos el player prefabs
        /*
        if (player == runner.LocalPlayer)
        {
            NetworkObject playerObject = runner.Spawn(playerPrefab, Vector3.zero);
            runner.SetPlayerObject(player, playerObject);
        }
        */
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (runner.IsSharedModeMasterClient)
        {
            runner.Spawn(ticTtacToePrefabs);
        }
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }
    /*
public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
{
   if (runner.IsSharedModeMasterClient)
   {
       runner.Spawn(ticTtacToePrefabs, Vector3.zero, Quaternion.identity, player);
   }
}

public static void StartGame(string sessionName)
{
   runnerIstance.ProvideInput = false;
   runnerIstance.StartGame(new StartGameArgs()
   {
       GameMode = GameMode.Shared,
       SessionName = sessionName,
       SceneManager = runnerIstance.GetComponent<NetworkSceneManagerDefault>()
   });
}
*/
}
