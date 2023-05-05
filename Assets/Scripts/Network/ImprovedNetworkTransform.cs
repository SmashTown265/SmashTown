

using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;

namespace Unity.Netcode.Components
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Netcode/Network Transform")]
    [DefaultExecutionOrder(100000)]
    public class ImprovedNetworkTransform : NetworkTransform
    {
        public new delegate (Vector3 pos, Quaternion rotOut, Vector3 scale) OnClientRequestChangeDelegate(Vector3 pos, Quaternion rot, Vector3 scale);

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

        protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
        {
            if (!(base.NetworkObject.gameObject == base.gameObject))
            {
                NetworkTransformState networkState = default(NetworkTransformState);
                if (serializer.IsWriter)
                {
                    networkState.IsTeleportingNextFrame = true;
                    ApplyTransformToNetworkStateWithInfo(ref networkState, m_CachedNetworkManager.LocalTime.Time, base.transform);
                    networkState.NetworkSerialize(serializer);
                }
                else
                {
                    networkState.NetworkSerialize(serializer);
                    AddInterpolatedState(networkState);
                }
            }
        }

        protected void TryCommitTransformToServer(Transform transformToCommit, double dirtyTime)
        {
            if (!base.IsOwner && !m_CachedIsServer)
            {
                NetworkLog.LogError("Non-owner instance, " + base.name + ", is trying to commit a transform!");
                return;
            }

            if (CanCommitToTransform)
            {
                UpdateAuthoritativeState(base.transform);
                return;
            }

            Vector3 pos = (InLocalSpace ? transformToCommit.localPosition : transformToCommit.position);
            Quaternion rot = (InLocalSpace ? transformToCommit.localRotation : transformToCommit.rotation);
            if (!m_CachedIsServer)
            {
                SetStateServerRpc(pos, rot, transformToCommit.localScale, shouldTeleport: false);
            }
            else
            {
                SetStateClientRpc(pos, rot, transformToCommit.localScale, shouldTeleport: false);
            }
        }

        private void TryCommitTransform(Transform transformToCommit, double dirtyTime)
        {
            if (!CanCommitToTransform && !base.IsOwner)
            {
                NetworkLog.LogError("[" + base.name + "] is trying to commit the transform without authority!");
            }
            else if (ApplyTransformToNetworkState(ref m_LocalAuthoritativeNetworkState, dirtyTime, transformToCommit))
            {
                ReplicatedNetworkState.Value = m_LocalAuthoritativeNetworkState;
            }
        }

        private void ResetInterpolatedStateToCurrentAuthoritativeState()
        {
            double time = base.NetworkManager.ServerTime.Time;
            Vector3 vector = (InLocalSpace ? base.transform.localPosition : base.transform.position);
            m_PositionXInterpolator.ResetTo(vector.x, time);
            m_PositionYInterpolator.ResetTo(vector.y, time);
            m_PositionZInterpolator.ResetTo(vector.z, time);
            Quaternion targetValue = (InLocalSpace ? base.transform.localRotation : base.transform.rotation);
            m_RotationInterpolator.ResetTo(targetValue, time);
            Vector3 localScale = base.transform.localScale;
            m_ScaleXInterpolator.ResetTo(localScale.x, time);
            m_ScaleYInterpolator.ResetTo(localScale.y, time);
            m_ScaleZInterpolator.ResetTo(localScale.z, time);
        }

        internal NetworkTransformState ApplyLocalNetworkState(Transform transform)
        {
            m_LocalAuthoritativeNetworkState.ClearBitSetForNextTick();
            ApplyTransformToNetworkStateWithInfo(ref m_LocalAuthoritativeNetworkState, m_CachedNetworkManager.LocalTime.Time, transform);
            return m_LocalAuthoritativeNetworkState;
        }

        internal bool ApplyTransformToNetworkState(ref NetworkTransformState networkState, double dirtyTime, Transform transformToUse)
        {
            return ApplyTransformToNetworkStateWithInfo(ref networkState, dirtyTime, transformToUse);
        }

        private bool ApplyTransformToNetworkStateWithInfo(ref NetworkTransformState networkState, double dirtyTime, Transform transformToUse)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            Vector3 vector = (InLocalSpace ? transformToUse.localPosition : transformToUse.position);
            Vector3 vector2 = (InLocalSpace ? transformToUse.localEulerAngles : transformToUse.eulerAngles);
            Vector3 localScale = transformToUse.localScale;
            if (InLocalSpace != networkState.InLocalSpace)
            {
                networkState.InLocalSpace = InLocalSpace;
                flag = true;
            }

            if (SyncPositionX && (Mathf.Abs(networkState.PositionX - vector.x) >= PositionThreshold || networkState.IsTeleportingNextFrame))
            {
                networkState.PositionX = vector.x;
                networkState.HasPositionX = true;
                flag2 = true;
            }

            if (SyncPositionY && (Mathf.Abs(networkState.PositionY - vector.y) >= PositionThreshold || networkState.IsTeleportingNextFrame))
            {
                networkState.PositionY = vector.y;
                networkState.HasPositionY = true;
                flag2 = true;
            }

            if (SyncPositionZ && (Mathf.Abs(networkState.PositionZ - vector.z) >= PositionThreshold || networkState.IsTeleportingNextFrame))
            {
                networkState.PositionZ = vector.z;
                networkState.HasPositionZ = true;
                flag2 = true;
            }

            if (SyncRotAngleX && (Mathf.Abs(Mathf.DeltaAngle(networkState.RotAngleX, vector2.x)) >= RotAngleThreshold || networkState.IsTeleportingNextFrame))
            {
                networkState.RotAngleX = vector2.x;
                networkState.HasRotAngleX = true;
                flag3 = true;
            }

            if (SyncRotAngleY && (Mathf.Abs(Mathf.DeltaAngle(networkState.RotAngleY, vector2.y)) >= RotAngleThreshold || networkState.IsTeleportingNextFrame))
            {
                networkState.RotAngleY = vector2.y;
                networkState.HasRotAngleY = true;
                flag3 = true;
            }

            if (SyncRotAngleZ && (Mathf.Abs(Mathf.DeltaAngle(networkState.RotAngleZ, vector2.z)) >= RotAngleThreshold || networkState.IsTeleportingNextFrame))
            {
                networkState.RotAngleZ = vector2.z;
                networkState.HasRotAngleZ = true;
                flag3 = true;
            }

            if (SyncScaleX && (Mathf.Abs(networkState.ScaleX - localScale.x) >= ScaleThreshold || networkState.IsTeleportingNextFrame))
            {
                networkState.ScaleX = localScale.x;
                networkState.HasScaleX = true;
                flag4 = true;
            }

            if (SyncScaleY && (Mathf.Abs(networkState.ScaleY - localScale.y) >= ScaleThreshold || networkState.IsTeleportingNextFrame))
            {
                networkState.ScaleY = localScale.y;
                networkState.HasScaleY = true;
                flag4 = true;
            }

            if (SyncScaleZ && (Mathf.Abs(networkState.ScaleZ - localScale.z) >= ScaleThreshold || networkState.IsTeleportingNextFrame))
            {
                networkState.ScaleZ = localScale.z;
                networkState.HasScaleZ = true;
                flag4 = true;
            }

            flag = flag || flag2 || flag3 || flag4;
            if (flag)
            {
                networkState.SentTime = dirtyTime;
            }

            networkState.IsDirty |= flag;
            return flag;
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
                if (SyncPositionX)
                {
                    vector.x = m_PositionXInterpolator.GetInterpolatedValue();
                }

                if (SyncPositionY)
                {
                    vector.y = m_PositionYInterpolator.GetInterpolatedValue();
                }

                if (SyncPositionZ)
                {
                    vector.z = m_PositionZInterpolator.GetInterpolatedValue();
                }

                if (SyncScaleX)
                {
                    localScale.x = m_ScaleXInterpolator.GetInterpolatedValue();
                    localScale.x = value.ScaleX;
                }

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

        private void AddInterpolatedState(NetworkTransformState newState)
        {
            double sentTime = newState.SentTime;
            Vector3 vector = (newState.InLocalSpace ? base.transform.localPosition : base.transform.position);
            Quaternion quaternion = (newState.InLocalSpace ? base.transform.localRotation : base.transform.rotation);
            Vector3 eulerAngles = quaternion.eulerAngles;
            if (newState.InLocalSpace != InLocalSpace || newState.IsTeleportingNextFrame)
            {
                InLocalSpace = newState.InLocalSpace;
                Vector3 localScale = base.transform.localScale;
                foreach (BufferedLinearInterpolator<float> allFloatInterpolator in m_AllFloatInterpolators)
                {
                    allFloatInterpolator.Clear();
                }

                m_RotationInterpolator.Clear();
                if (newState.HasPositionX)
                {
                    m_PositionXInterpolator.ResetTo(newState.PositionX, sentTime);
                    vector.x = newState.PositionX;
                }

                if (newState.HasPositionY)
                {
                    m_PositionYInterpolator.ResetTo(newState.PositionY, sentTime);
                    vector.y = newState.PositionY;
                }

                if (newState.HasPositionZ)
                {
                    m_PositionZInterpolator.ResetTo(newState.PositionZ, sentTime);
                    vector.z = newState.PositionZ;
                }

                if (newState.InLocalSpace)
                {
                    base.transform.localPosition = vector;
                }
                else
                {
                    base.transform.position = vector;
                }

                if (newState.HasScaleX)
                {
                    m_ScaleXInterpolator.ResetTo(newState.ScaleX, sentTime);
                    localScale.x = newState.ScaleX;
                }

                if (newState.HasScaleY)
                {
                    m_ScaleYInterpolator.ResetTo(newState.ScaleY, sentTime);
                    localScale.y = newState.ScaleY;
                }

                if (newState.HasScaleZ)
                {
                    m_ScaleZInterpolator.ResetTo(newState.ScaleZ, sentTime);
                    localScale.z = newState.ScaleZ;
                }

                base.transform.localScale = localScale;
                if (newState.HasRotAngleX)
                {
                    eulerAngles.x = newState.RotAngleX;
                }

                if (newState.HasRotAngleY)
                {
                    eulerAngles.y = newState.RotAngleY;
                }

                if (newState.HasRotAngleZ)
                {
                    eulerAngles.z = newState.RotAngleZ;
                }

                quaternion.eulerAngles = eulerAngles;
                base.transform.rotation = quaternion;
                m_RotationInterpolator.ResetTo(quaternion, sentTime);
                return;
            }

            if (newState.HasPositionX)
            {
                m_PositionXInterpolator.AddMeasurement(newState.PositionX, sentTime);
            }

            if (newState.HasPositionY)
            {
                m_PositionYInterpolator.AddMeasurement(newState.PositionY, sentTime);
            }

            if (newState.HasPositionZ)
            {
                m_PositionZInterpolator.AddMeasurement(newState.PositionZ, sentTime);
            }

            if (newState.HasScaleX)
            {
                m_ScaleXInterpolator.AddMeasurement(newState.ScaleX, sentTime);
            }

            if (newState.HasScaleY)
            {
                m_ScaleYInterpolator.AddMeasurement(newState.ScaleY, sentTime);
            }

            if (newState.HasScaleZ)
            {
                m_ScaleZInterpolator.AddMeasurement(newState.ScaleZ, sentTime);
            }

            if (newState.HasRotAngleChange)
            {
                if (newState.HasRotAngleX)
                {
                    eulerAngles.x = newState.RotAngleX;
                }

                if (newState.HasRotAngleY)
                {
                    eulerAngles.y = newState.RotAngleY;
                }

                if (newState.HasRotAngleZ)
                {
                    eulerAngles.z = newState.RotAngleZ;
                }

                quaternion.eulerAngles = eulerAngles;
                m_RotationInterpolator.AddMeasurement(quaternion, sentTime);
            }
        }

        private void OnNetworkStateChanged(NetworkTransformState oldState, NetworkTransformState newState)
        {
            if (base.NetworkObject.IsSpawned && !CanCommitToTransform && Interpolate)
            {
                AddInterpolatedState(newState);
            }
        }


        private void Awake()
        {
            m_RotationInterpolator = new BufferedLinearInterpolatorQuaternion();
            m_PositionXInterpolator = new BufferedLinearInterpolatorFloat();
            m_PositionYInterpolator = new BufferedLinearInterpolatorFloat();
            m_PositionZInterpolator = new BufferedLinearInterpolatorFloat();
            m_ScaleXInterpolator = new BufferedLinearInterpolatorFloat();
            m_ScaleYInterpolator = new BufferedLinearInterpolatorFloat();
            m_ScaleZInterpolator = new BufferedLinearInterpolatorFloat();
            if (m_AllFloatInterpolators.Count == 0)
            {
                m_AllFloatInterpolators.Add(m_PositionXInterpolator);
                m_AllFloatInterpolators.Add(m_PositionYInterpolator);
                m_AllFloatInterpolators.Add(m_PositionZInterpolator);
                m_AllFloatInterpolators.Add(m_ScaleXInterpolator);
                m_AllFloatInterpolators.Add(m_ScaleYInterpolator);
                m_AllFloatInterpolators.Add(m_ScaleZInterpolator);
            }
        }

        public override void OnNetworkSpawn()
        {
            m_CachedIsServer = base.IsServer;
            m_CachedNetworkManager = base.NetworkManager;
            Initialize();
            if (CanCommitToTransform)
            {
                Vector3 pos = (InLocalSpace ? base.transform.localPosition : base.transform.position);
                Quaternion rot = (InLocalSpace ? base.transform.localRotation : base.transform.rotation);
                SetStateInternal(pos, rot, base.transform.localScale, shouldTeleport: true);
                TryCommitTransform(base.transform, m_CachedNetworkManager.LocalTime.Time);
            }
        }

        public override void OnNetworkDespawn()
        {
            NetworkVariable<NetworkTransformState> replicatedNetworkState = ReplicatedNetworkState;
            replicatedNetworkState.OnValueChanged = (NetworkVariable<NetworkTransformState>.OnValueChangedDelegate)Delegate.Remove(replicatedNetworkState.OnValueChanged, new NetworkVariable<NetworkTransformState>.OnValueChangedDelegate(OnNetworkStateChanged));
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            m_ReplicatedNetworkStateServer.Dispose();
            m_ReplicatedNetworkStateOwner.Dispose();
        }

        public override void OnGainedOwnership()
        {
            Initialize();
        }

        public override void OnLostOwnership()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (base.IsSpawned)
            {
                CanCommitToTransform = (IsServerAuthoritative() ? base.IsServer : base.IsOwner);
                NetworkVariable<NetworkTransformState> replicatedNetworkState = ReplicatedNetworkState;
                m_LocalAuthoritativeNetworkState = replicatedNetworkState.Value;
                if (CanCommitToTransform)
                {
                    replicatedNetworkState.OnValueChanged = (NetworkVariable<NetworkTransformState>.OnValueChangedDelegate)Delegate.Remove(replicatedNetworkState.OnValueChanged, new NetworkVariable<NetworkTransformState>.OnValueChangedDelegate(OnNetworkStateChanged));
                    return;
                }

                replicatedNetworkState.OnValueChanged = (NetworkVariable<NetworkTransformState>.OnValueChangedDelegate)Delegate.Combine(replicatedNetworkState.OnValueChanged, new NetworkVariable<NetworkTransformState>.OnValueChangedDelegate(OnNetworkStateChanged));
                ResetInterpolatedStateToCurrentAuthoritativeState();
            }
        }

        public void SetState(Vector3? posIn = null, Quaternion? rotIn = null, Vector3? scaleIn = null, bool shouldGhostsInterpolate = true)
        {
            if (!base.IsSpawned)
            {
                return;
            }

            if (!base.IsOwner && !m_CachedIsServer)
            {
                throw new Exception("Non-owner client instance cannot set the state of the NetworkTransform!");
            }

            Vector3 pos = (posIn.HasValue ? posIn.Value : (InLocalSpace ? base.transform.localPosition : base.transform.position));
            Quaternion rot = (rotIn.HasValue ? rotIn.Value : (InLocalSpace ? base.transform.localRotation : base.transform.rotation));
            Vector3 scale = ((!scaleIn.HasValue) ? base.transform.localScale : scaleIn.Value);
            if (!CanCommitToTransform)
            {
                if (m_CachedIsServer)
                {
                    m_ClientIds[0] = base.OwnerClientId;
                    m_ClientRpcParams.Send.TargetClientIds = m_ClientIds;
                    SetStateClientRpc(pos, rot, scale, !shouldGhostsInterpolate, m_ClientRpcParams);
                }
                else
                {
                    SetStateServerRpc(pos, rot, scale, !shouldGhostsInterpolate);
                }
            }
            else
            {
                SetStateInternal(pos, rot, scale, !shouldGhostsInterpolate);
            }
        }

        private void SetStateInternal(Vector3 pos, Quaternion rot, Vector3 scale, bool shouldTeleport)
        {
            if (InLocalSpace)
            {
                base.transform.localPosition = pos;
                base.transform.localRotation = rot;
            }
            else
            {
                base.transform.SetPositionAndRotation(pos, rot);
            }

            base.transform.localScale = scale;
            m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = shouldTeleport;
            TryCommitTransform(base.transform, m_CachedNetworkManager.LocalTime.Time);
        }

        [ClientRpc]
        private void SetStateClientRpc(Vector3 pos, Quaternion rot, Vector3 scale, bool shouldTeleport, ClientRpcParams clientRpcParams = default(ClientRpcParams))
        {
            NetworkManager networkManager = base.NetworkManager;
            if ((object)networkManager != null && networkManager.IsListening)
            {
                if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
                {
                    FastBufferWriter bufferWriter = __beginSendClientRpc(1724438000u, clientRpcParams, RpcDelivery.Reliable);
                    bufferWriter.WriteValueSafe(in pos);
                    bufferWriter.WriteValueSafe(in rot);
                    bufferWriter.WriteValueSafe(in scale);
                    bufferWriter.WriteValueSafe(in shouldTeleport, default(FastBufferWriter.ForPrimitives));
                    __endSendClientRpc(ref bufferWriter, 1724438000u, clientRpcParams, RpcDelivery.Reliable);
                }

                if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost))
                {
                    SetStateInternal(pos, rot, scale, shouldTeleport);
                }
            }
        }

        [ServerRpc]
        private void SetStateServerRpc(Vector3 pos, Quaternion rot, Vector3 scale, bool shouldTeleport)
        {
            NetworkManager networkManager = base.NetworkManager;
            if ((object)networkManager == null || !networkManager.IsListening)
            {
                return;
            }

            if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
            {
                if (base.OwnerClientId != networkManager.LocalClientId)
                {
                    if (networkManager.LogLevel <= LogLevel.Normal)
                    {
                        Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
                    }

                    return;
                }

                ServerRpcParams serverRpcParams = default(ServerRpcParams);
                FastBufferWriter bufferWriter = __beginSendServerRpc(640767722u, serverRpcParams, RpcDelivery.Reliable);
                bufferWriter.WriteValueSafe(in pos);
                bufferWriter.WriteValueSafe(in rot);
                bufferWriter.WriteValueSafe(in scale);
                bufferWriter.WriteValueSafe(in shouldTeleport, default(FastBufferWriter.ForPrimitives));
                __endSendServerRpc(ref bufferWriter, 640767722u, serverRpcParams, RpcDelivery.Reliable);
            }

            if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
            {
                if (OnClientRequestChange != null)
                {
                    (pos, rot, scale) = OnClientRequestChange(pos, rot, scale);
                }

                SetStateInternal(pos, rot, scale, shouldTeleport);
            }
        }

        private void UpdateAuthoritativeState(Transform transformSource)
        {
            if (!ReplicatedNetworkState.IsDirty() && m_LocalAuthoritativeNetworkState.IsDirty)
            {
                m_LastSentState = m_LocalAuthoritativeNetworkState;
                m_LocalAuthoritativeNetworkState.ClearBitSetForNextTick();
            }

            TryCommitTransform(transformSource, m_CachedNetworkManager.LocalTime.Time);
        }

        protected virtual void Update()
        {
            if (!base.IsSpawned)
            {
                return;
            }

            if (CanCommitToTransform)
            {
                UpdateAuthoritativeState(base.transform);
                return;
            }

            if (Interpolate)
            {
                NetworkTime serverTime = base.NetworkManager.ServerTime;
                float deltaTime = Time.deltaTime;
                double time = serverTime.Time;
                double time2 = serverTime.TimeTicksAgo(1).Time;
                foreach (BufferedLinearInterpolator<float> allFloatInterpolator in m_AllFloatInterpolators)
                {
                    allFloatInterpolator.Update(deltaTime, time2, time);
                }

                m_RotationInterpolator.Update(deltaTime, time2, time);
            }

            ApplyAuthoritativeState();
        }

        public void Teleport(Vector3 newPosition, Quaternion newRotation, Vector3 newScale)
        {
            if (!CanCommitToTransform)
            {
                throw new Exception("Teleporting on non-authoritative side is not allowed!");
            }

            SetStateInternal(newPosition, newRotation, newScale, shouldTeleport: true);
        }

        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }

        internal bool IsServerAuthoritative()
        {
            return OnIsServerAuthoritative();
        }

        static ImprovedNetworkTransform()
        {
            
        }

    }
}