
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Netcode.Components
{
    [DisallowMultipleComponent]
    public class ImprovedNetworkTransform : NetworkTransform
    {
        Vector3 scale = new Vector3(1.5f,1,0);

        public void OnLateUpdate()
        {
            scale.x = 1.5f * Mathf.Sign(transform.localScale.x + .00000001f);
            transform.localScale = scale;
        }

        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }

    }
}
