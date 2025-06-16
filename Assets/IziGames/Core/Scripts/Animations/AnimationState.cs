using UnityEngine;

[CreateAssetMenu(fileName = "New Animation State", menuName = "IziGames/Animation State", order = 0)]
[Icon("Assets/IziGames/Gizmos/animation.png")]
public class AnimationState : ScriptableObject
{
    [Header("Animations")]
    public AnimationClip animationClip;

    [Header("Settings")]
    public float transitionTime = 0.2f;
    public bool loop = true;

    [Header("Transitions")]
    public AnimationClip transitionArrive;
    public AnimationClip transitionDepart;
}
