using System.Collections.Generic;
using UnityEngine;

namespace Core {

    /**
    * LAYER UTIL - ONE SCRIPT TO HOUSE EVERY LAYER MASK, ETC.
    *
    * USAGE:
    *
    * - Add all of your layers to the LayerType enum
    * - Update static getters to include all of your layers
    *
    * ```
    * Layer.init(); // initialize the Layer in Awake() or Start()
    * Layer.Water.mask(); // get mask for Water layer
    * Layer.Ground.value(); // get layer number for Ground layer
    * ```
    *
    * PROS:
    *
    * - one-time setup
    * - update single file when adding/updating layers
    * - warns if a layer does not exist
    * - allows for intellisense autosuggest for layers (e.g. start typing "Layer.Gr" to get the ground layer)
    *
    * CONS:
    *
    * - must initialize upon app start
    */

    public enum LayerType {
        Default,
        Player,
        Enemy,
        NPC,
        Environment,
    }

    public static class Layer {
        public static LayerMaskItem Default => layerMaskItems[LayerType.Default.ToString()];
        public static LayerMaskItem Player => layerMaskItems[LayerType.Player.ToString()];
        public static LayerMaskItem Enemy => layerMaskItems[LayerType.Enemy.ToString()];
        public static LayerMaskItem NPC => layerMaskItems[LayerType.NPC.ToString()];
        public static LayerMaskItem Environment => layerMaskItems[LayerType.Environment.ToString()];

        static Dictionary<string, LayerMaskItem> layerMaskItems = new Dictionary<string, LayerMaskItem>();
        static Dictionary<int, LayerMaskItem> layerMaskLookup = new Dictionary<int, LayerMaskItem>();
        static bool initialized = false;

        /// <summary>Initialize layers (call in Awake or Start)</summary>
        public static void Init() {
            if (initialized) return;

            foreach (string name in System.Enum.GetNames(typeof(LayerType))) {
                layerMaskItems.Add(name, new LayerMaskItem(name));
                layerMaskLookup.Add(layerMaskItems[name].value, layerMaskItems[name]);
            }

            initialized = true;
        }

        public static LayerMaskItem Lookup(int layer) {
            try {
                return layerMaskLookup[layer];
            } catch (System.Exception) {
                return Default;
            }
        }
    }

    public struct LayerMaskItem {
        public LayerMaskItem(string layerName) {
            _name = layerName;
            _mask = LayerMask.GetMask(layerName);
            _layerType = System.Enum.Parse<LayerType>(_name);
            if (_mask == 0 && layerName != "Default") Debug.LogWarning("Warning: layer \"" + layerName + "\" may not exist");
        }
        string _name;
        int _mask;
        LayerType _layerType;
        public string name => _name;
        public int mask => _mask;
        public LayerType layerType => _layerType;
        public int value => LayerUtils.ToLayer(_mask);
        public bool ContainsLayer(int layer) { return LayerUtils.LayerMaskContainsLayer(_mask, layer); }
        public override string ToString() { return _name + " | " + _layerType + " | " + value + " | " + _mask; }
        public bool Equals(int number) { return this.value == number; }
    }
}

