using UnityEngine;

namespace LayerMasks
{
    public static class LayerMasks
    {
        private static LayerMask _layerMask = (1 << 3);
        public static LayerMask GetLayerMask() => _layerMask;
    }
}