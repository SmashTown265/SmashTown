
using UnityEngine;

namespace Unity.Netcode.Components
{
    /// <summary>
    /// NetworkRigidbody allows for the use of <see cref="Rigidbody2D"/> on network objects. By controlling the kinematic
    /// mode of the rigidbody and disabling it on all peers but the authoritative one.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(ImprovedNetworkTransform))]
 
    public class ImprovedNetworkRigidbody2D : NetworkBehaviour
    {
        private Rigidbody2D m_Rigidbody;
        private ImprovedNetworkTransform m_ImprovedNetworkTransform;

        private bool m_OriginalKinematic;
        private RigidbodyInterpolation2D m_OriginalInterpolation;

        // Used to cache the authority state of this rigidbody during the last frame
        private bool m_IsAuthority;

        /// <summary>
        /// Gets a bool value indicating whether this <see cref="ImprovedNetworkRigidbody2D"/> on this peer currently holds authority.
        /// </summary>
        private bool HasAuthority => m_ImprovedNetworkTransform.CanCommitToTransform;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_ImprovedNetworkTransform = GetComponent<ImprovedNetworkTransform>();
        }

        private void FixedUpdate()
        {
            if (IsSpawned)
            {
                if (HasAuthority != m_IsAuthority)
                {
                    m_IsAuthority = HasAuthority;
                    UpdateRigidbodyKinematicMode();
                }
            }
        }

        // Puts the rigidbody in a kinematic non-interpolated mode on everyone but the server.
        private void UpdateRigidbodyKinematicMode()
        {
            if (m_IsAuthority == false)
            {
                m_OriginalKinematic = m_Rigidbody.isKinematic;
                m_Rigidbody.isKinematic = true;

                m_OriginalInterpolation = m_Rigidbody.interpolation;
                // Set interpolation to none, the ImprovedNetworkTransform component interpolates the position of the object.
                m_Rigidbody.interpolation = RigidbodyInterpolation2D.None;
            }
            else
            {
                // Resets the rigidbody back to it's non replication only state. Happens on shutdown and when authority is lost
                m_Rigidbody.isKinematic = m_OriginalKinematic;
                m_Rigidbody.interpolation = m_OriginalInterpolation;
            }
        }

        /// <inheritdoc />
        public override void OnNetworkSpawn()
        {
            m_IsAuthority = HasAuthority;
            m_OriginalKinematic = m_Rigidbody.isKinematic;
            m_OriginalInterpolation = m_Rigidbody.interpolation;
            UpdateRigidbodyKinematicMode();
        }

        /// <inheritdoc />
        public override void OnNetworkDespawn()
        {
            UpdateRigidbodyKinematicMode();
        }
    }
}

