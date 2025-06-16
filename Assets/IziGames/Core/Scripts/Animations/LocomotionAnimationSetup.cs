using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

#if UNITY_EDITOR
public static class LocomotionAnimationSetup
{
    
    // Dans Character.cs ou un autre script

    public static void PlayAnimation(Animator animator, LocomotionState locomotionState, float transitionTime, string stateName)
    {
        if (animator == null) return;
        float fadeTime = locomotionState != null ? locomotionState.transitionDuration : transitionTime;
        animator.CrossFade(stateName, fadeTime);
    }
    /// <summary>
    /// Applique les animations d'un LocomotionState à un AnimatorController existant
    /// </summary>
    public static void ApplyLocomotionState(AnimatorController targetController, LocomotionState locomotionState)
    {
        if (targetController == null || locomotionState == null)
        {
            Debug.LogError("Missing references for animation setup");
            return;
        }

        bool anyChanges = false;

        foreach (var layer in targetController.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.motion is BlendTree mainBlendTree)
                {
                    if (UpdateBlendTreeRecursively(mainBlendTree, locomotionState))
                    {
                        anyChanges = true;
                    }
                }
            }
        }

        if (anyChanges)
        {
            EditorUtility.SetDirty(targetController);
            AssetDatabase.SaveAssets();
            Debug.Log("Locomotion state applied to animator controller");
        }
    }
    
    /// <summary>
    /// Applique les animations d'un LocomotionState à un Animator et son AnimatorController
    /// </summary>
    public static void ApplyLocomotionState(Animator targetAnimator, LocomotionState locomotionState)
    {
        if (targetAnimator == null)
        {
            Debug.LogError("Target animator is null");
            return;
        }
        
        AnimatorController controller = targetAnimator.runtimeAnimatorController as AnimatorController;
        if (controller == null)
        {
            Debug.LogError("Target animator doesn't have an AnimatorController");
            return;
        }
        
        ApplyLocomotionState(controller, locomotionState);
    }
    
    /// <summary>
    /// Applique un LocomotionState à un Character
    /// </summary>
    public static void ApplyLocomotionStateToCharacter(Character character)
    {
        if (character == null)
        {
            Debug.LogError("Character is null");
            return;
        }
        
        if (character.animator == null)
        {
            Debug.LogError("Character doesn't have an animator reference");
            return;
        }
        
        if (character.animationState == null)
        {
            Debug.LogError("Character doesn't have a locomotion state reference");
            return;
        }
        
        ApplyLocomotionState(character.animator, character.animationState);
    }

    private static bool UpdateBlendTreeRecursively(BlendTree tree, LocomotionState state)
    {
        bool anyChanges = false;
        var children = tree.children;
        
        for (int i = 0; i < children.Length; i++)
        {
            var child = children[i];
            
            if (child.motion is BlendTree childTree)
            {
                if (UpdateBlendTreeRecursively(childTree, state))
                {
                    anyChanges = true;
                }
                continue;
            }

            string motionName = child.motion?.name?.ToLower() ?? "";
            AnimationClip newClip = null;
            
            if (motionName.Contains("idle"))
                newClip = state.idle;
            else if (motionName.Contains("walk_forward_left") || motionName.Contains("walk_f_l"))
                newClip = state.walk_foward_left;
            else if (motionName.Contains("walk_forward_right") || motionName.Contains("walk_f_r"))
                newClip = state.walk_foward_right;
            else if (motionName.Contains("walk_backward_left") || motionName.Contains("walk_b_l"))
                newClip = state.walk_backward_left;
            else if (motionName.Contains("walk_backward_right") || motionName.Contains("walk_b_r"))
                newClip = state.walk_backward_right;
            else if (motionName.Contains("walk_f") || motionName.Contains("walk_forward"))
                newClip = state.walk_foward;
            else if (motionName.Contains("walk_b") || motionName.Contains("walk_back"))
                newClip = state.walk_backward;
            else if (motionName.Contains("walk_l") || motionName.Contains("walk_left"))
                newClip = state.walk_left;
            else if (motionName.Contains("walk_r") || motionName.Contains("walk_right"))
                newClip = state.walk_right;
            else if (motionName.Contains("run_f") || motionName.Contains("run_forward"))
                newClip = state.walk_foward;
            else if (motionName.Contains("run_b") || motionName.Contains("run_back"))
                newClip = state.walk_backward;
            else if (motionName.Contains("run_l") || motionName.Contains("run_left"))
                newClip = state.walk_left;
            else if (motionName.Contains("run_r") || motionName.Contains("run_right"))
                newClip = state.walk_right;
            
            if (newClip != null && child.motion != newClip)
            {
                child.motion = newClip;
                children[i] = child;
                anyChanges = true;
            }
        }
        
        if (anyChanges)
        {
            tree.children = children;
        }
        
        return anyChanges;
    }
}

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        Character character = (Character)target;
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Apply Animation State"))
        {
            if (character.animator != null && character.animationState != null)
            {
                LocomotionAnimationSetup.ApplyLocomotionStateToCharacter(character);
            }
            else
            {
                Debug.LogError("Missing animator or locomotion state reference");
            }
        }
    }
}
#endif