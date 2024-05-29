using System.Collections.Generic;
using UnityEngine;

namespace PoseInfo
{
    public class PoseEnum : MonoBehaviour
    {
        // Keypoint format
        public enum Keypoint : int
        {
            Root,
            Rhip, Rknee, Rankle,
            Lhip, Lknee, Lankle,
            Belly, Neck, Nose, Head,
            Lshoulder, Lelbow, Lwrist,
            Rshoulder, Relbow, Rwrist
        };
        // Keypoints pairs for indexing
        public enum Bone : int
        {
            RootToRhip, RhipToRknee, RkneeToRankle,
            RootToLhip, LhipToLknee, LkneeToLankle,
            BellyToNeck, NeckToNose, NoseToHead,
            NeckToLshoulder, LshoulderToLelbow, LelbowToLwrist,
            NeckToRshoulder, RshoulderToRelbow, RelbowToRwrist
        }
        // Joint pairs
        public static readonly Dictionary<Bone, (Keypoint, Keypoint)> BoneToKeypointPair = new Dictionary<Bone, (Keypoint, Keypoint)>
        {
            { Bone.RootToRhip, (Keypoint.Root, Keypoint.Rhip) },
            { Bone.RhipToRknee, (Keypoint.Rhip, Keypoint.Rknee) },
            { Bone.RkneeToRankle, (Keypoint.Rknee, Keypoint.Rankle) },
            { Bone.RootToLhip, (Keypoint.Root, Keypoint.Lhip) },
            { Bone.LhipToLknee, (Keypoint.Lhip, Keypoint.Lknee) },
            { Bone.LkneeToLankle, (Keypoint.Lknee, Keypoint.Lankle) },
            { Bone.BellyToNeck, (Keypoint.Belly, Keypoint.Neck) },
            { Bone.NeckToNose, (Keypoint.Neck, Keypoint.Nose) },
            { Bone.NoseToHead, (Keypoint.Nose, Keypoint.Head) },
            { Bone.NeckToLshoulder, (Keypoint.Neck, Keypoint.Lshoulder) },
            { Bone.LshoulderToLelbow, (Keypoint.Lshoulder, Keypoint.Lelbow) },
            { Bone.LelbowToLwrist, (Keypoint.Lelbow, Keypoint.Lwrist) },
            { Bone.NeckToRshoulder, (Keypoint.Neck, Keypoint.Rshoulder) },
            { Bone.RshoulderToRelbow, (Keypoint.Rshoulder, Keypoint.Relbow) },
            { Bone.RelbowToRwrist, (Keypoint.Relbow, Keypoint.Rwrist) }
        };
    }
}