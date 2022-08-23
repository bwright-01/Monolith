namespace Core {

    public static class LayerUtils {
        // check to see whether a LayerMask contains a layer
        // see: https://answers.unity.com/questions/50279/check-if-layer-is-in-layermask.html
        public static bool LayerMaskContainsLayer(int mask, int layer) {
            bool contains = ((mask & (1 << layer)) != 0);
            return contains;
        }

        // get the layer num from a layermask
        // see: https://forum.unity.com/threads/get-the-layernumber-from-a-layermask.114553/#post-3021162
        public static int ToLayer(int layerMask) {
            int result = layerMask > 0 ? 0 : 31;
            while (layerMask > 1) {
                layerMask = layerMask >> 1;
                result++;
            }
            return result;
        }
    }
}