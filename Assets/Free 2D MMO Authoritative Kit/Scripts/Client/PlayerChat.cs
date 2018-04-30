using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class PlayerChat : Chat
{
    HandlersCode handlersCode = new HandlersCode();

    private void Start()
    {
        // register chat handler on client - message code 131
        NetworkManager.singleton.client.RegisterHandler((short)handlersCode.chatCode, ReceiveMessage);
    }

	private void ReceiveMessage(NetworkMessage message)
	{
		//reading message
		string text = message.ReadMessage<StringMessage> ().value;

		AddMessage (text);
	}

	public void ServerReceiveMessage(NetworkMessage message)
	{
		StringMessage myMessage = new StringMessage ();
		//we are using the connectionId as player name only to exemplify
		myMessage.value = message.conn.connectionId + ": " + message.ReadMessage<StringMessage> ().value;

		//sending to all connected clients
		NetworkServer.SendToAll ((short)handlersCode.chatCode, myMessage);
	}

	public override void SendMessage (UnityEngine.UI.InputField input)
	{
		StringMessage myMessage = new StringMessage ();
		//getting the value of the input
		myMessage.value = input.text;

		//sending to server
		NetworkManager.singleton.client.Send ((short)handlersCode.chatCode, myMessage);
        
        //clear input
        input.text = "";
	}
}