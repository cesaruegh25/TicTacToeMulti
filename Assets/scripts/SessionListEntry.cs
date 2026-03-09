using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionListEntry : MonoBehaviour
{
    public TextMeshProUGUI roomName, playerCount;
    public Button joinButton;

    public void JoinRoom()
    {
        NetworkManager.runnerIstance.StartGame(new StartGameArgs()
        {
            SessionName = roomName.text,
            GameMode = GameMode.Shared,
        });

    }

}