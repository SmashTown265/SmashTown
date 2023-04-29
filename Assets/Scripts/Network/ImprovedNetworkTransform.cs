
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Netcode.Components
{
    [DisallowMultipleComponent]
    public class ImprovedNetworkTransform : NetworkTransform
    {
        internal struct NetworkTransformState : INetworkSerializable
        {
            private const int k_InLocalSpaceBit = 0;

            private const int k_PositionXBit = 1;

            private const int k_PositionYBit = 2;

            private const int k_PositionZBit = 3;

            private const int k_RotAngleXBit = 4;

            private const int k_RotAngleYBit = 5;

            private const int k_RotAngleZBit = 6;

            private const int k_ScaleXBit = 7;

            private const int k_ScaleYBit = 8;

            private const int k_ScaleZBit = 9;

            private const int k_TeleportingBit = 10;

            private ushort m_Bitset;

            internal float PositionX;

            internal float PositionY;

            internal float PositionZ;

            internal float RotAngleX;

            internal float RotAngleY;

            internal float RotAngleZ;

            internal float ScaleX;

            internal float ScaleY;

            internal float ScaleZ;

            internal double SentTime;

            internal bool IsDirty;

            internal int EndExtrapolationTick;

            internal bool InLocalSpace
            {
                get
                {
                    return (m_Bitset & 1) != 0;
                }
                set
                {
                    if (value)
                    {
                        m_Bitset |= 1;
                    }
                    else
                    {
                        m_Bitset = (ushort)(m_Bitset & 0xFFFFFFFEu);
                    }
                }
            }

            internal bool HasPositionX
            {
                get
                {
                    return (m_Bitset & 2) != 0;
                }
                set
                {
                    if (value)
                    {
                        m_Bitset |= 2;
                    }
                    else
                    {
                        m_Bitset = (ushort)(m_Bitset & 0xFFFFFFFDu);
                    }
                }
            }

            internal bool HasPositionY
            {
                get
                {
                    return (m_Bitset & 4) != 0;
                }
                set
                {
                    if (value)
                    {
                        m_Bitset |= 4;
                    }
                    else
                    {
                        m_Bitset = (ushort)(m_Bitset & 0xFFFFFFFBu);
                    }
                }
            }

            internal bool HasPositionZ
            {
                get
                {
                    return (m_Bitset & 8) != 0;
                }
                set
                {
                    if (value)
                    {
                        m_Bitset |= 8;
                    }
                    else
                    {
                        m_Bitset = (ushort)(m_Bitset & 0xFFFFFFF7u);
                    }
                }
            }

            internal bool HasPositionChange => HasPositionX | HasPositionY | HasPositionZ;

            internal bool HasRotAngleX
            {
                get
                {
                    return (m_Bitset & 0x10) != 0;
                }
                set
                {
                    if (value)
                    {
                        m_Bitset |= 16;
                    }
                    else
                    {
                        m_Bitset = (ushort)(m_Bitset & 0xFFFFFFEFu);
                    }
                }
            }

            internal bool HasRotAngleY
            {
                get
                {
                    return (m_Bitset & 0x20) != 0;
                }
                set
                {
                    if (value)
                    {
                        m_Bitset |= 32;
                    }
                    else
                    {
                        m_Bitset = (ushort)(m_Bitset & 0xFFFFFFDFu);
                    }
                }
            }

            internal bool HasRotAngleZ
            {
                get
                {
                    return (m_Bitset & 0x40) != 0;
                }
                set
                {
                    if (value)
                    {
                        m_Bitset |= 64;
                    }
                    else
                    {
                        m_Bitset = (ushort)(m_Bitset & 0xFFFFFFBFu);
                    }
                }
            }

            internal bool HasRotAngleChange => HasRotAngleX | HasRotAngleY | HasRotAngleZ;

            internal bool HasScaleX
            {
                get
                {
                    return (m_Bitset & 0x80) != 0;
                }
                set
                {
                    if (value)
                    {
                        m_Bitset |= 128;
                    }
                    else
                    {
                        m_Bitset = (ushort)(m_Bitset & 0xFFFFFF7Fu);
                    }
                }
            }

            internal bool HasScaleY
            {
                get
                {
                    return (m_Bitset & 0x100) != 0;
                }
                set
                {
                    if (value)
                    {
                        m_Bitset |= 256;
                    }
                    else
                    {
                        m_Bitset = (ushort)(m_Bitset & 0xFFFFFEFFu);
                    }
                }
            }

            internal bool HasScaleZ
            {
                get
                {
                    return (m_Bitset & 0x200) != 0;
                }
                set
                {
                    if (value)
                    {
                        m_Bitset |= 512;
                    }
                    else
                    {
                        m_Bitset = (ushort)(m_Bitset & 0xFFFFFDFFu);
                    }
                }
            }

            internal bool HasScaleChange => HasScaleX | HasScaleY | HasScaleZ;

            internal bool IsTeleportingNextFrame
            {
                get
                {
                    return (m_Bitset & 0x400) != 0;
                }
                set
                {
                    if (value)
                    {
                        m_Bitset |= 1024;
                    }
                    else
                    {
                        m_Bitset = (ushort)(m_Bitset & 0xFFFFFBFFu);
                    }
                }
            }

            internal void ClearBitSetForNextTick()
            {
                m_Bitset &= (ushort)(m_Bitset & 1);
                IsDirty = false;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref SentTime, default(FastBufferWriter.ForPrimitives));
                serializer.SerializeValue(ref m_Bitset, default(FastBufferWriter.ForPrimitives));
                if (HasPositionX)
                {
                    serializer.SerializeValue(ref PositionX, default(FastBufferWriter.ForPrimitives));
                }

                if (HasPositionY)
                {
                    serializer.SerializeValue(ref PositionY, default(FastBufferWriter.ForPrimitives));
                }

                if (HasPositionZ)
                {
                    serializer.SerializeValue(ref PositionZ, default(FastBufferWriter.ForPrimitives));
                }

                if (HasRotAngleX)
                {
                    serializer.SerializeValue(ref RotAngleX, default(FastBufferWriter.ForPrimitives));
                }

                if (HasRotAngleY)
                {
                    serializer.SerializeValue(ref RotAngleY, default(FastBufferWriter.ForPrimitives));
                }

                if (HasRotAngleZ)
                {
                    serializer.SerializeValue(ref RotAngleZ, default(FastBufferWriter.ForPrimitives));
                }

                if (HasScaleX)
                {
                    serializer.SerializeValue(ref ScaleX, default(FastBufferWriter.ForPrimitives));
                }

                if (HasScaleY)
                {
                    serializer.SerializeValue(ref ScaleY, default(FastBufferWriter.ForPrimitives));
                }

                if (HasScaleZ)
                {
                    serializer.SerializeValue(ref ScaleZ, default(FastBufferWriter.ForPrimitives));
                }

                if (serializer.IsReader)
                {
                    IsDirty = HasPositionChange || HasRotAngleChange || HasScaleChange;
                }
            }
        }

        public const float PositionThresholdDefault = 0.001f;

        public const float RotAngleThresholdDefault = 0.01f;

        public const float ScaleThresholdDefault = 0.01f;

        public OnClientRequestChangeDelegate OnClientRequestChange;

        public bool SyncPositionX = true;

        public bool SyncPositionY = true;

        public bool SyncPositionZ = true;

        public bool SyncRotAngleX = true;

        public bool SyncRotAngleY = true;

        public bool SyncRotAngleZ = true;

        public bool SyncScaleX = true;

        public bool SyncScaleY = true;

        public bool SyncScaleZ = true;

        public float PositionThreshold = 0.001f;

        [Range(0.001f, 360f)]
        public float RotAngleThreshold = 0.01f;

        public float ScaleThreshold = 0.01f;

        [Tooltip("Sets whether this transform should sync in local space or in world space")]
        public bool InLocalSpace;

        public bool Interpolate = true;
        
        [SerializeField]public bool InterpolatePositionX = true;

        [SerializeField]public bool InterpolatePositionY = true;

        [SerializeField]public bool InterpolatePositionZ = true;

        [SerializeField]public bool InterpolateRotAngleX = true;

        [SerializeField] public bool InterpolateRotAngleY = true;

        [SerializeField] public bool InterpolateRotAngleZ = true;

        [SerializeField] public bool InterpolateScaleX = false;

        [SerializeField] public bool InterpolateScaleY = true;

        [SerializeField] public bool InterpolateScaleZ = true;

        protected bool m_CachedIsServer;

        protected NetworkManager m_CachedNetworkManager;

        private readonly NetworkVariable<NetworkTransformState> m_ReplicatedNetworkStateServer = new NetworkVariable<NetworkTransformState>();

        private readonly NetworkVariable<NetworkTransformState> m_ReplicatedNetworkStateOwner = new NetworkVariable<NetworkTransformState>(default(NetworkTransformState), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private NetworkTransformState m_LocalAuthoritativeNetworkState;

        private ClientRpcParams m_ClientRpcParams = new ClientRpcParams
        {
            Send = default(ClientRpcSendParams)
        };

        private List<ulong> m_ClientIds = new List<ulong> { 0uL };

        private BufferedLinearInterpolator<float> m_PositionXInterpolator;

        private BufferedLinearInterpolator<float> m_PositionYInterpolator;

        private BufferedLinearInterpolator<float> m_PositionZInterpolator;

        private BufferedLinearInterpolator<Quaternion> m_RotationInterpolator;

        private BufferedLinearInterpolator<float> m_ScaleXInterpolator;

        private BufferedLinearInterpolator<float> m_ScaleYInterpolator;

        private BufferedLinearInterpolator<float> m_ScaleZInterpolator;

        private readonly List<BufferedLinearInterpolator<float>> m_AllFloatInterpolators = new List<BufferedLinearInterpolator<float>>(6);

        private NetworkTransformState m_LastSentState;

        private bool SynchronizePosition
        {
            get
            {
                if (!SyncPositionX && !SyncPositionY)
                {
                    return SyncPositionZ;
                }

                return true;
            }
        }

        private bool SynchronizeRotation
        {
            get
            {
                if (!SyncRotAngleX && !SyncRotAngleY)
                {
                    return SyncRotAngleZ;
                }

                return true;
            }
        }

        private bool SynchronizeScale
        {
            get
            {
                if (!SyncScaleX && !SyncScaleY)
                {
                    return SyncScaleZ;
                }

                return true;
            }
        }

        public bool CanCommitToTransform { get; protected set; }

        internal NetworkVariable<NetworkTransformState> ReplicatedNetworkState
        {
            get
            {
                if (!IsServerAuthoritative())
                {
                    return m_ReplicatedNetworkStateOwner;
                }

                return m_ReplicatedNetworkStateServer;
            }
        }

        internal NetworkTransformState GetLastSentState()
        {
            return m_LastSentState;
        }
        private void ApplyAuthoritativeState()
        {
            NetworkTransformState value = ReplicatedNetworkState.Value;
            Vector3 vector = (value.InLocalSpace ? base.transform.localPosition : base.transform.position);
            Vector3 euler = (value.InLocalSpace ? base.transform.localEulerAngles : base.transform.eulerAngles);
            Vector3 localScale = base.transform.localScale;
            InLocalSpace = value.InLocalSpace;
            bool flag = !value.IsTeleportingNextFrame && Interpolate;
            if (flag)
            {
                if (SyncPositionX && InterpolatePositionX)
                {
                    vector.x = m_PositionXInterpolator.GetInterpolatedValue();
                }

                if (SyncPositionY && InterpolatePositionY)
                {
                    vector.y = m_PositionYInterpolator.GetInterpolatedValue();
                }

                if (SyncPositionZ && InterpolatePositionZ)
                {
                    vector.z = m_PositionZInterpolator.GetInterpolatedValue();
                }

                if (SyncScaleX && InterpolateScaleX)
                {
                    localScale.x = m_ScaleXInterpolator.GetInterpolatedValue();
                }
                else
                    localScale.x = value.ScaleX;

                if (SyncScaleY)
                {
                    localScale.y = m_ScaleYInterpolator.GetInterpolatedValue();
                }

                if (SyncScaleZ)
                {
                    localScale.z = m_ScaleZInterpolator.GetInterpolatedValue();
                }

                if (SynchronizeRotation)
                {
                    Vector3 eulerAngles = m_RotationInterpolator.GetInterpolatedValue().eulerAngles;
                    if (SyncRotAngleX)
                    {
                        euler.x = eulerAngles.x;
                    }

                    if (SyncRotAngleY)
                    {
                        euler.y = eulerAngles.y;
                    }

                    if (SyncRotAngleZ)
                    {
                        euler.z = eulerAngles.z;
                    }
                }
            }
            else
            {
                if (value.HasPositionX)
                {
                    vector.x = value.PositionX;
                }

                if (value.HasPositionY)
                {
                    vector.y = value.PositionY;
                }

                if (value.HasPositionZ)
                {
                    vector.z = value.PositionZ;
                }

                if (value.HasScaleX)
                {
                    localScale.x = value.ScaleX;
                }

                if (value.HasScaleY)
                {
                    localScale.y = value.ScaleY;
                }

                if (value.HasScaleZ)
                {
                    localScale.z = value.ScaleZ;
                }

                if (value.HasRotAngleX)
                {
                    euler.x = value.RotAngleX;
                }

                if (value.HasRotAngleY)
                {
                    euler.y = value.RotAngleY;
                }

                if (value.HasRotAngleZ)
                {
                    euler.z = value.RotAngleZ;
                }
            }

            if (value.HasPositionChange || (flag && SynchronizePosition))
            {
                if (InLocalSpace)
                {
                    base.transform.localPosition = vector;
                }
                else
                {
                    base.transform.position = vector;
                }
            }

            if (value.HasRotAngleChange || (flag && SynchronizeRotation))
            {
                if (InLocalSpace)
                {
                    base.transform.localRotation = Quaternion.Euler(euler);
                }
                else
                {
                    base.transform.rotation = Quaternion.Euler(euler);
                }
            }

            if (value.HasScaleChange || (flag && SynchronizeScale))
            {
                base.transform.localScale = localScale;
            }
        }
        internal bool IsServerAuthoritative()
        {
            return OnIsServerAuthoritative();
        }

        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }

    }
}
