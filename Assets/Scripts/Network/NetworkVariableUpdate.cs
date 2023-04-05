using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkVariableUpdate : NetworkBehaviour
{
	private const int k_InitialValue = 1111;
	private NetworkVariable<int> m_SomeValue = new NetworkVariable<int>();
    private NetworkVariable<Vector3> player1Position = new NetworkVariable<Vector3>();
    private NetworkVariable<Vector3> player1Velocity = new NetworkVariable<Vector3>();
    private NetworkVariable<State> player1State = new NetworkVariable<State>();
    private NetworkVariable<Vector3> player2Position = new NetworkVariable<Vector3>();
    private NetworkVariable<Vector3> player2Velocity = new NetworkVariable<Vector3>();
    private NetworkVariable<State> player2State = new NetworkVariable<State>();
    

	public override void OnNetworkSpawn()
	{
		if (IsServer)
		{
			NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
		}
		else
		{
			if (m_SomeValue.Value != k_InitialValue)
			{

			}
			else
			{

			}
			m_SomeValue.OnValueChanged += OnSomeValueChanged;
		}
	}

	private void NetworkManager_OnClientConnectedCallback(ulong obj)
	{
		StartCoroutine(StartChangingNetworkVariable());
    }

	private void OnSomeValueChanged(int previous, int current)
	{

	}

	private IEnumerator StartChangingNetworkVariable()
	{
		var count = 0;
		var updateFrequency = new WaitForSeconds(0.5f);
		while (count < 4)
		{

			yield return updateFrequency;
		}
		NetworkManager.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
	}
}