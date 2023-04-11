using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SpatialSys.UnitySDK
{
    [System.Serializable]
    public struct CollisionPair
    {
        public int layer1;
        public int layer2;
        public bool ignore;

        public CollisionPair(int layer1, int layer2, bool ignore)
        {
            this.layer1 = layer1;
            this.layer2 = layer2;
            this.ignore = ignore;
        }
    }

    public class SavedProjectSettings : ScriptableObject
    {
        [HideInInspector]
        public List<CollisionPair> customCollisionSettings;
        [HideInInspector]
        public List<CollisionPair> customCollision2DSettings;
    }
}
