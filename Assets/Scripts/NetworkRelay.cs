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


    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    public TMP_InputField inputfield;

    public static NetworkRelay Instance { get; private set; }
    public event EventHandler OnJoinStarted;
    public event EventHandler OnJoinFailed;

    public Allocation allocation;

    private void Awake() 
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication() 
    {
        if (UnityServices.State != ServicesInitializationState.Initialized) 
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            //initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }


    public async void AllocateRelay() 
    {
        try 
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(2);
            GetRelayJoinCode();
            //return allocation;
        } 
        catch (RelayServiceException e) 
        {
            Debug.Log(e);

            //return default;
        }
    }

    public async void GetRelayJoinCode() 
    {
        try 
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            GUIUtility.systemCopyBuffer = relayJoinCode;
            print(relayJoinCode);
            
        } 
        catch (RelayServiceException e) {
            Debug.Log(e);
            //return default;
        }
    }

    public async void JoinRelay()
    {
	    string joinCode = inputfield.text;
        try
        {
	        print(joinCode);
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            print(joinAllocation.AllocationId);
        } 
        catch (RelayServiceException e) 
        {
            Debug.Log(e);
            
        }
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