using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK.VisualScripting
{
    public class ClientBridge
    {
        public delegate int GetLocalActorDelegate();
        public static GetLocalActorDelegate GetLocalActor;

        public delegate List<int> GetActorsDelegate();
        public static GetActorsDelegate GetActors;

        public delegate bool GetAvatarExistsDelegate(int actorNumber);
        public static GetAvatarExistsDelegate GetAvatarExists;

        public delegate Vector3 GetLocalAvatarPositionDelegate();
        public static GetLocalAvatarPositionDelegate GetLocalAvatarPosition;

        public delegate void SetLocalAvatarPositionDelegate(Vector3 position);
        public static SetLocalAvatarPositionDelegate SetLocalAvatarPosition;

        public delegate Quaternion GetLocalAvatarRotationDelegate();
        public static GetLocalAvatarRotationDelegate GetLocalAvatarRotation;

        public delegate Vector3 GetAvatarPositionWithActorDelegate(int actorNumber);
        public static GetAvatarPositionWithActorDelegate GetAvatarPositionWithActor;

        public delegate Quaternion GetAvatarRotationWithActorDelegate(int actorNumber);
        public static GetAvatarRotationWithActorDelegate GetAvatarRotationWithActor;

        public delegate int GetLocalActorPlatformDelegate();
        public static GetLocalActorPlatformDelegate GetLocalActorPlatform;

        // bool = actorNumber is valid
        public delegate Tuple<bool, int> GetActorPlatformDelegate(int actorNumber);
        public static GetActorPlatformDelegate GetActorPlatform;

        public delegate int GetRandomActorDelegate();
        public static GetRandomActorDelegate GetRandomActor;

        public delegate void AddForceToLocalAvatarDelegate(Vector3 force);
        public static AddForceToLocalAvatarDelegate AddForceToLocalAvatar;

        public delegate void SetLocalAvatarDelegate(string sku);
        public static SetLocalAvatarDelegate SetLocalAvatar;

        public delegate bool IsLocalOwnerDelegate();
        public static IsLocalOwnerDelegate IsLocalOwner;

        public delegate bool IsLocalHostDelegate();
        public static IsLocalHostDelegate IsLocalHost;

        public delegate bool HasLocalLovedSpaceDelegate();
        public static HasLocalLovedSpaceDelegate HasLocalLovedSpace;

        public delegate void OpenURLDelegate(string url);
        public static OpenURLDelegate OpenURL;

        public delegate Vector3 GetCameraPositionDelegate();
        public static GetCameraPositionDelegate GetCameraPosition;

        public delegate Quaternion GetCameraRotationDelegate();
        public static GetCameraRotationDelegate GetCameraRotation;

        public delegate Vector3 GetCameraForwardDelegate();
        public static GetCameraForwardDelegate GetCameraForward;

        public delegate void SendToastDelegate(string message, float duration);
        public static SendToastDelegate SendToast;

        public delegate void PlayLocalAvatarEmoteAnimationDelegate(string sku);
        public static PlayLocalAvatarEmoteAnimationDelegate PlayLocalAvatarEmoteAnimation;

        public delegate void SpawnPrefabObjectDelegate(string sku, Vector3 position, Quaternion rotation);
        public static SpawnPrefabObjectDelegate SpawnPrefabObject;

        // Quests
        public delegate void QuestDelegate(SpatialQuest quest);
        public static QuestDelegate StartQuest;
        public static QuestDelegate CompleteQuest;
        public static QuestDelegate ResetQuest;

        public delegate void QuestTaskDelegate(SpatialQuest quest, uint taskId);
        public static QuestTaskDelegate StartQuestTask;
        public static QuestTaskDelegate AddQuestTaskProgress;
        public static QuestTaskDelegate CompleteQuestTask;

        public delegate int GetQuestTaskProgressDelegate(SpatialQuest quest, uint task);
        public static GetQuestTaskProgressDelegate GetQuestTaskProgress;

        public delegate int GetQuestStatusDelegate(SpatialQuest quest);
        public static GetQuestStatusDelegate GetQuestStatus;

        public delegate int GetQuestTaskStatusDelegate(SpatialQuest quest, uint task);
        public static GetQuestTaskStatusDelegate GetQuestTaskStatus;

        public delegate bool TakeoverSyncedObjectOwnerhipDelegate(SpatialSyncedObject syncedObject);
        public static TakeoverSyncedObjectOwnerhipDelegate TakeoverSyncedObjectOwnership;

        public delegate SpatialSyncedObject GetSyncedObjectByIDDelegate(int id);
        public static GetSyncedObjectByIDDelegate GetSyncedObjectByID;

        public delegate bool GetSyncedObjectIsSyncedDelegate(SpatialSyncedObject syncedObject);
        public static GetSyncedObjectIsSyncedDelegate GetSyncedObjectIsSynced;

        public delegate int GetSyncedObjectIDDelegate(SpatialSyncedObject syncedObject);
        public static GetSyncedObjectIDDelegate GetSyncedObjectID;

        public delegate int GetSyncedObjectOwnerDelegate(SpatialSyncedObject syncedObject);
        public static GetSyncedObjectOwnerDelegate GetSyncedObjectOwner;

        public delegate bool GetSyncedObjectHasControlDelegate(SpatialSyncedObject syncedObject);
        public static GetSyncedObjectHasControlDelegate GetSyncedObjectHasControl;

        public delegate bool GetSyncedObjectIsLocallyOwnedDelegate(SpatialSyncedObject syncedObject);
        public static GetSyncedObjectIsLocallyOwnedDelegate GetSyncedObjectIsLocallyOwned;

        public delegate void SetSyncedAnimatorParameterDelegate(SpatialSyncedAnimator syncedAnimator, string parameterName, object value);
        public static SetSyncedAnimatorParameterDelegate SetSyncedAnimatorParameter;

        public delegate void SetSyncedAnimatorTriggerDelegate(SpatialSyncedAnimator syncedAnimator, string triggerName);
        public static SetSyncedAnimatorTriggerDelegate SetSyncedAnimatorTrigger;
    }
}