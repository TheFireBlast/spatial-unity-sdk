using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SpatialSys.UnitySDK
{
    public class SpatialThumbnailCamera : SpatialComponentBase
    {
        public override string prettyName => "Thumbnail Camera";
        public override string tooltip => "Use to specify the location in which live thumbnails are captured for your space. These are the thumbnails you will see in your spaces list or on the Spatial homepage.";

        public override string documentationURL => "https://www.notion.so/spatialxr/Thumbnail-Camera-40283993ab6f4a4abf15190a20cf63ef";

        [HideInInspector]
        public float fieldOfView;// set during scene process

        private void Reset()
        {
            Camera c = GetComponent<Camera>();
            if (c == null)
            {
                c = gameObject.AddComponent<Camera>();
            }
            c.fieldOfView = 85f;
        }
    }
}