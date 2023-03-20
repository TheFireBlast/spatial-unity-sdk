using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Avatar State")]
    [UnitCategory("Spatial\\Get Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetAvatarStateNode : Unit
    {
        [DoNotSerialize]
        [NullMeansSelf]
        public ValueInput actor { get; private set; }
        [DoNotSerialize]
        [PortLabel("Avatar Exists")]
        public ValueOutput avatarExists { get; private set; }
        [DoNotSerialize]
        [PortLabel("Position")]
        public ValueOutput avatarPosition { get; private set; }
        [DoNotSerialize]
        [PortLabel("Rotation")]
        public ValueOutput avatarRotation { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);

            avatarExists = ValueOutput<bool>(nameof(avatarExists), (f) => ClientBridge.GetAvatarExists.Invoke(f.GetValue<int>(actor)));
            avatarPosition = ValueOutput<Vector3>(nameof(avatarPosition), (f) => ClientBridge.GetAvatarPositionWithActor.Invoke(f.GetValue<int>(actor)));
            avatarRotation = ValueOutput<Quaternion>(nameof(avatarRotation), (f) => ClientBridge.GetAvatarRotationWithActor.Invoke(f.GetValue<int>(actor)));
        }
    }

    [UnitTitle("Local Avatar State")]
    [UnitCategory("Spatial\\Get Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarStateNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Position")]
        public ValueOutput avatarPosition { get; private set; }
        [DoNotSerialize]
        [PortLabel("Rotation")]
        public ValueOutput avatarRotation { get; private set; }

        protected override void Definition()
        {
            avatarPosition = ValueOutput<Vector3>(nameof(avatarPosition), (f) => ClientBridge.GetLocalAvatarPosition.Invoke());
            avatarRotation = ValueOutput<Quaternion>(nameof(avatarRotation), (f) => ClientBridge.GetLocalAvatarRotation.Invoke());
        }
    }
}