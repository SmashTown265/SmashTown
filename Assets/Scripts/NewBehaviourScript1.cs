using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class NewBehaviourScript1 : NetworkBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }
        [ServerRpc]
        public void SendServerRPC()
        {
	         
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}