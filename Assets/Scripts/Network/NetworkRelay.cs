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


public class NetworkRelay : MonoBehaviour
{
	private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    public TMP_InputField inputfield;
    public TMP_InputField joinCodeField;
    public string relayJoinCode;
    public static NetworkRelay Instance { get; private set; }
    public event EventHandler OnJoinStarted;

    public const int MAX_PLAYER_AMOUNT = 2;
    public event EventHandler OnCreateGameStarted;
    public event EventHandler OnCreateGameFailed;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;
    public event EventHandler OnPlayerConnected;
    public event EventHandler OnCreateGameSuccess;
    [HideInInspector] public bool online = false;
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
        print("Host before foreach");
        TMP_InputField[] temp = GameObject.FindObjectsOfType<TMP_InputField>(true);
        foreach (TMP_InputField tempItem in temp)
        {
            print("Host in foreach");
            if (tempItem.gameObject.CompareTag("JoinOutput"))
                joinCodeField = tempItem;
        }
        print("Host after foreach");
        // Start the Relay Server, and output the code
        online = true;
	    StartCoroutine(ConfigureTransportAndStartNgoAsHost());
    }

    public void JoinGame()
    {
        TMP_InputField[] temp = GameObject.FindObjectsOfType<TMP_InputField>(true);
        foreach (TMP_InputField tempItem in temp)
        {
            if (tempItem.gameObject.CompareTag("JoinInput"))
                inputfield = tempItem;
        }

        online = true;
	    StartCoroutine(ConfigureTransportAndStartNgoAsConnectingPlayer());
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
            //GUIUtility.systemCopyBuffer = relayJoinCode;
            joinCodeField.readOnly = false;
            joinCodeField.text = relayJoinCode;
            joinCodeField.readOnly = true;
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
        OnCreateGameStarted?.Invoke(this, EventArgs.Empty);
	    var serverRelayUtilityTask = AllocateRelayAndGetRelayJoinCode();
	    while (!serverRelayUtilityTask.IsCompleted)
	    {
		    yield return null;
	    }
	    if (serverRelayUtilityTask.IsFaulted)
	    {
		    Debug.LogError("Exception thrown when attempting to start Relay Server. Server not started. Exception: " + serverRelayUtilityTask.Exception.Message);
            OnCreateGameFailed?.Invoke(this, EventArgs.Empty);
		    yield break;
	    }

	    var relayServerData = serverRelayUtilityTask.Result;


        OnCreateGameSuccess?.Invoke(this, EventArgs.Empty);
	    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
	    NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
	    NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
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
	    OnJoinStarted?.Invoke(this, EventArgs.Empty);
	    NetworkManager.Singleton.StartClient();

	    yield return null;
    }
     private void clientIDNetworkList_OnListChanged(NetworkListEvent<ulong> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }


    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        if(clientId != NetworkManager.ServerClientId)
            OnPlayerConnected?.Invoke(this, EventArgs.Empty);

    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != "playerLobby")
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }


    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
    }
}