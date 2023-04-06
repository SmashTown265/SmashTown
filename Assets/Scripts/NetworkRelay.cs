using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using Unity.Services.Relay.Http;
using UnityEngine.UI;
using NetworkEvent = Unity.Networking.Transport.NetworkEvent;

public class NetworkRelay : MonoBehaviour
{
	private NetworkSceneManager networkSM;
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    public TMP_InputField inputfield;
    public TMP_InputField joinCodeField;
    public string relayJoinCode;
    public static NetworkRelay Instance { get; private set; }
    public event EventHandler OnJoinStarted;
    public event EventHandler OnJoinFailed;

    //public Allocation allocation;

    private void Awake() 
    {
        // If there is already an instance of this object running
        // destroy the new one and keep the old one
        if(Instance  == null)
			Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
        // Keep instance of this gameObject running when loading new scenes
        DontDestroyOnLoad(gameObject);

        // Authenticate with Unity to do relay stuff
        InitializeUnityAuthentication();
        
    }

    private void Start()
    { 
	    networkSM = NetworkManager.Singleton.SceneManager;
    }

    private async void InitializeUnityAuthentication() 
    {
        if (UnityServices.State != ServicesInitializationState.Initialized) 
        {
            InitializationOptions initializationOptions = new InitializationOptions();

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public void HostGame()
    {
        // Start the Relay Server, and output the code
	    StartCoroutine(ConfigureTransportAndStartNgoAsHost());
	    
    }

    public void JoinGame()
    {
	    StartCoroutine(ConfigureTransportAndStartNgoAsConnectingPlayer());
    }

    public void GoToPlayerSelection()
    {
	    // Load the Player Selection Scene and wait for players to join
	    networkSM.LoadScene("PlayerSelection", LoadSceneMode.Single);
    }

    // Start a relay server and request relay join code
    public async Task<RelayServerData> AllocateRelayAndGetRelayJoinCode()
    {
	    Allocation allocation;
	    try
	    { 
		    allocation = await RelayService.Instance.CreateAllocationAsync(2);
		    
	    }
	    catch (RelayServiceException e)
	    {
		    Debug.Log(e);
		    throw;
	    }

	    print($"Host: {allocation.AllocationId}");

		try 
        {
            relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            GUIUtility.systemCopyBuffer = relayJoinCode;
            // Display the joinCode to the user.
            joinCodeField.text = relayJoinCode;
        }
        catch (RelayServiceException e) {
            Debug.Log(e);
            throw;
        }

	    return new RelayServerData(allocation, "dtls");
    }

    public async Task<RelayServerData> JoinRelay()
    {
	    JoinAllocation joinAllocation;
	    string joinCode = inputfield.text;
        try
        {
	        joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException e) 
        {
            Debug.Log(e);
            throw;
        }

        Debug.Log($"client: {joinAllocation.ConnectionData[0]} {joinAllocation.ConnectionData[1]}");
        Debug.Log($"host: {joinAllocation.HostConnectionData[0]} {joinAllocation.HostConnectionData[1]}");
        Debug.Log($"client: {joinAllocation.AllocationId}");


        return new RelayServerData(joinAllocation, "dtls");
    }

    IEnumerator ConfigureTransportAndStartNgoAsHost()
    {
	    var serverRelayUtilityTask = AllocateRelayAndGetRelayJoinCode();
	    while (!serverRelayUtilityTask.IsCompleted)
	    {
		    yield return null;
	    }
	    if (serverRelayUtilityTask.IsFaulted)
	    {
		    Debug.LogError("Exception thrown when attempting to start Relay Server. Server not started. Exception: " + serverRelayUtilityTask.Exception.Message);
		    yield break;
	    }

	    var relayServerData = serverRelayUtilityTask.Result;



	    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
	    NetworkManager.Singleton.StartHost();

	    yield return null;
    }
    
    IEnumerator ConfigureTransportAndStartNgoAsConnectingPlayer()
    {
	    // Populate RelayJoinCode beforehand through the UI
	    var clientRelayUtilityTask = JoinRelay();

	    while (!clientRelayUtilityTask.IsCompleted)
	    {
		    yield return null;
	    }

	    if (clientRelayUtilityTask.IsFaulted)
	    {
		    Debug.LogError("Exception thrown when attempting to connect to Relay Server. Exception: " + clientRelayUtilityTask.Exception.Message);
		    yield break;
	    }

	    var relayServerData = clientRelayUtilityTask.Result;

	    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
	    NetworkManager.Singleton.StartClient();

	    yield return null;
    }
    //public async void QuickJoin() 
    //{
    //    OnJoinStarted?.Invoke(this, EventArgs.Empty);
    //    try 
    //    {
    //        joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

    //        string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

    //        JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

    //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

    //        KitchenGameMultiplayer.Instance.StartClient();
    //    } 
    //    catch (LobbyServiceException e) {
    //        Debug.Log(e);
    //        OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
    //    }
    //}


    //public async void KickPlayer(string playerId) 
    //{
    //    if (IsServer) 
    //    {
    //        try 
    //        {
    //            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
    //        } 
    //        catch (LobbyServiceException e) 
    //        {
    //            Debug.Log(e);
    //        }
    //    }
    //}
}