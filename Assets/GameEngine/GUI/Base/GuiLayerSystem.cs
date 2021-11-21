using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Piratera.GUI
{
    public enum LayerId
    {
        GUI = 0,
        POPUP = 1,
        LOADING = 2,
        IMPORTANT = 3
    }
    class GuiLayerSystem : MonoBehaviour
    {
        private List<GameObject> layers = new List<GameObject>();
        Dictionary<string, GameObject> guis = new Dictionary<string, GameObject>();

        void Start()
        {
            foreach (LayerId val in Enum.GetValues(typeof(LayerId)))
            {
                CreateLayer(val);
            }
        }

        private void CreateLayer(LayerId layerId)
        {
            GameObject panel = new GameObject("Layer-System: " + layerId);
            panel.transform.SetParent(transform);
            panel.transform.localPosition = Vector3.zero;
            panel.AddComponent<CanvasRenderer>();

            RectTransform parentRectTransform = GetComponent<RectTransform>();
            
            RectTransform rectTransform = panel.AddComponent<RectTransform>();
            rectTransform.sizeDelta = parentRectTransform.sizeDelta;
            rectTransform.localScale = Vector3.one;

            layers.Add(panel);
        }

        public GameObject AddGui(GameObject prefap, LayerId layerId, string guiId)
        {
            
            GameObject layer = layers[((int)layerId)];
            if (layer != null)
            {
                GameObject gui = Instantiate(prefap, layer.transform);
               
                if (guis.ContainsKey(guiId))
                {
                    DestroyGui(guiId);
                    Debug.LogWarning("Duplicate GUI " + guiId);
                }
                guis[guiId] = gui;
                return gui;
            }

            return null;
          
        }

        internal void DestroyGui(string guiId)
        {
            if (guis.ContainsKey(guiId))
            {
                Destroy(guis[guiId]);
                guis.Remove(guiId);
            }
        }
    }
}
