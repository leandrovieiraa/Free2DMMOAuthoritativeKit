using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class GameServer : NetworkManager
{
    HandlersCode handlersCode = new HandlersCode();

    UIObjects uiObjects;

    private string serverName;
    private string ip;
    private string port;

    public int onlinePlayers = 0;

    private void Start()
    {
        uiObjects = GetComponent<UIObjects>();
    }

    private void Update()
    {
        uiObjects.serverConnectionsText.text = "Connetions: <color=orange>" + onlinePlayers + "</color>";
    }

    public void StartupHost()
    {
        SetServerName();
        SetServerIPConfig();
        SetServerPortConfig();

        string ValidIpAddressRegex = "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9‌​]{2}|2[0-4][0-9]|25[0-5])$";    // IP validation 

        Regex reg = new Regex(ValidIpAddressRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        Match match = reg.Match(ip);

        if (!match.Success)
            Debug.Log("[GAMESERVER]: Error on Startup Host, please set a valid IP address and PORT number.");
        else
        {
            NetworkManager.singleton.StartServer();
            uiObjects.ServerConfigUI.SetActive(false);

            // register chat handler on server
            NetworkServer.RegisterHandler((short)handlersCode.chatCode, uiObjects.ChatUI.GetComponent<PlayerChat>().ServerReceiveMessage);
        }
    }

    public void JoinGame()
    {
        SetServerIPConfig();
        SetServerPortConfig();

        string ValidIpAddressRegex = "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9‌​]{2}|2[0-4][0-9]|25[0-5])$";    // IP validation 

        Regex reg = new Regex(ValidIpAddressRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        Match match = reg.Match(ip);

        if (!match.Success)
            Debug.Log("[GAMESERVER]: Error on Startup Host, please set a valid IP address and PORT number.");
        else
        {
            NetworkManager.singleton.StartClient();

            Debug.Log("[GAMESERVER]: Join Game: " + ip + ":" + port);
            uiObjects.ServerConfigUI.SetActive(false);
            uiObjects.ChatUI.SetActive(true);
        }
    }

    void SetServerName()
    {
        serverName = GameObject.Find("InputServerName").transform.Find("Text").GetComponent<Text>().text;
        NetworkManager.singleton.matchName = serverName;
    }

    void SetServerIPConfig()
    {
        ip = GameObject.Find("InputIP").transform.Find("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ip;
    }

    void SetServerPortConfig()
    {
        port = GameObject.Find("InputPort").transform.Find("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkPort = Convert.ToInt32(port);
    }

    public override void OnStartServer()
    {
        uiObjects.ServerInformationsUI.SetActive(true);

        uiObjects.serverNameText.text = "Server Name: <color=lime>" + serverName + "</color>";
        uiObjects.serverIpPortText.text = "Server IP: <color=lime>" + ip + ":" + port + "</color>";
        uiObjects.serverStatusText.text = "Server Status: <color=lime>Online</color>";

        Debug.Log("[GAMESERVER]: OnStartServer");
    }

    public override void OnServerConnect(NetworkConnection connection)
    {
        onlinePlayers++;
        Debug.Log("[GAMESERVER]: OnPlayerConnected, ID: " + connection.connectionId);
    }

    public override void OnServerDisconnect(NetworkConnection connection)
    {
        onlinePlayers--;
        Debug.Log("[GAMESERVER]: OnPlayerDisconnect, ID: " + connection.connectionId);
        OnServerRemovePlayer(connection, connection.playerControllers[0]);
    }

    public override void OnServerRemovePlayer(NetworkConnection connection, PlayerController player)
    {
        Debug.Log("[GAMESERVER]: OnServerRemovePlayer, ID: " + connection.connectionId);
        Destroy(player.gameObject);
    }
}
