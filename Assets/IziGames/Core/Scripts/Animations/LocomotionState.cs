using UnityEngine;

[CreateAssetMenu(fileName = "New Locomotion State", menuName = "IziGames/Locomotion State", order = 0)]
[Icon("Assets/IziGames/Gizmos/animation.png")]
public class LocomotionState : ScriptableObject
{
    public string stateName;

    [Header("Animations")]
    public AnimationClip idle;
    public AnimationClip foward;
    public AnimationClip foward_left;
    public AnimationClip foward_right;
    public AnimationClip backward;
    public AnimationClip backward_left;
    public AnimationClip backward_right;
    public AnimationClip left;
    public AnimationClip right;

    [Header("Transition Settings")]
    [Range(0, 1)]
    public float transitionDuration = 0.2f;
    [Range(0, 1)]
    public float transitionOffset = 0f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float blendingSpeed = 2.5f;
    public bool applyRootMotion = false;

    [Header("Settings")]
    public float transitionTime = 0.2f;
    public bool loop = true;
    public bool useRootMotion = true;

    [Header("Transitions")]
    public AnimationClip transitionArrive;
    public AnimationClip transitionDepart;
}
