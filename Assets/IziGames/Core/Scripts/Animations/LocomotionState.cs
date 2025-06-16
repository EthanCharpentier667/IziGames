using UnityEngine;

[CreateAssetMenu(fileName = "New Locomotion State", menuName = "IziGames/Locomotion State", order = 0)]
[Icon("Assets/IziGames/Gizmos/animation.png")]
public class LocomotionState : ScriptableObject
{
    public string stateName;

    [Header("Animations")]
    public AnimationClip idle;
    public AnimationClip walk_foward;
    public AnimationClip walk_foward_left;
    public AnimationClip walk_foward_right;
    public AnimationClip walk_backward;
    public AnimationClip walk_backward_left;
    public AnimationClip walk_backward_right;
    public AnimationClip walk_left;
    public AnimationClip walk_right;

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

    [Header("Transitions")]
    public AnimationClip transitionArrive;
    public AnimationClip transitionDepart;
}
