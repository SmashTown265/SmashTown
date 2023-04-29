
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Netcode.Components
{
    [DisallowMultipleComponent]
    public class ImprovedNetworkTransform : NetworkTransform
    {
        Vector3 scale = Vector3.zero;

        private readonly NetworkVariable<NetworkTransformState> m_ReplicatedNetworkStateServer = new NetworkVariable<NetworkTransformState>();

        private readonly NetworkVariable<NetworkTransformState> m_ReplicatedNetworkStateOwner = new NetworkVariable<NetworkTransformState>(default(NetworkTransformState), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        internal NetworkVariable<NetworkTransformState> ReplicatedNetworkState
        {
            get
            {
                if (!OnIsServerAuthoritative())
                {
                    return m_ReplicatedNetworkStateOwner;
                }

                return m_ReplicatedNetworkStateServer;
            }
        }
        public void LateUpdate()
        {
            scale.Set(ReplicatedNetworkState.Value.ScaleX,ReplicatedNetworkState.Value.ScaleY,ReplicatedNetworkState.Value.ScaleZ);
            transform.localScale = scale;    
        }
        
        
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

        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }

    }
}
