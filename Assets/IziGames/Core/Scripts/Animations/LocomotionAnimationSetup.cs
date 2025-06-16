using UnityEngine;
using UnityEditor.Animations;

public static class LocomotionAnimationSetup
{
    public static void ApplyLocomotionStateToCharacter(Character character)
    {
        if (character == null || character.baseState == null || character.animator == null)
            return;
        AnimatorOverrideController overrideController = GetOrCreateOverrideController(character.animator);
        ApplyAnimationOverrides(overrideController, character.baseState);
        EnsureAnimatorParameters(character.animator);
        character.animator.applyRootMotion = character.baseState.useRootMotion;
        character.animator.runtimeAnimatorController = overrideController;
    }
    
    public static void ApplyOtherLocomotionStateToCharacter(Character character, LocomotionState newLocomotion)
    {
        if (character == null || character.baseState == null || character.animator == null)
            return;
        AnimatorOverrideController overrideController = GetOrCreateOverrideController(character.animator);
        ApplyAnimationOverrides(overrideController, newLocomotion);
        EnsureAnimatorParameters(character.animator);
        character.animator.applyRootMotion =newLocomotion.useRootMotion;
        character.animator.runtimeAnimatorController = overrideController;
    }
    
    private static AnimatorOverrideController GetOrCreateOverrideController(Animator animator)
    {
        var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

        if (overrideController == null)
        {
            var baseController = Resources.Load<RuntimeAnimatorController>("BasicLocomotion");
            if (baseController == null)
            {
                Debug.LogError("Le controller de base 'BasicLocomotion' est manquant dans le dossier Resources");
                return null;
            }

            overrideController = new AnimatorOverrideController(baseController);
        }

        return overrideController;
    }
    
    private static void ApplyAnimationOverrides(AnimatorOverrideController controller, LocomotionState locomotion)
    {
        if (controller == null || locomotion == null) return;

        var overridesList = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<AnimationClip, AnimationClip>>();
        controller.GetOverrides(overridesList);
        var overridesDict = new AnimationClipOverrides(overridesList.Count);
        foreach (var kvp in overridesList)
        {
            if (kvp.Key != null)
                overridesDict[kvp.Key.name] = kvp.Value;
        }
        ApplyOverride(ref overridesDict, "Human@Stand_Idle", locomotion.idle);
        ApplyOverride(ref overridesDict, "Human@Stand_Fast_F", locomotion.foward);
        ApplyOverride(ref overridesDict, "Human@Stand_Fast_B", locomotion.backward);
        ApplyOverride(ref overridesDict, "Human@Stand_Fast_L", locomotion.left);
        ApplyOverride(ref overridesDict, "Human@Stand_Fast_R", locomotion.right);
        ApplyOverride(ref overridesDict, "Human@Stand_Fast_FL", locomotion.foward_left);
        ApplyOverride(ref overridesDict, "Human@Stand_Fast_FR", locomotion.foward_right);
        ApplyOverride(ref overridesDict, "Human@Stand_Fast_BL", locomotion.backward_left);
        ApplyOverride(ref overridesDict, "Human@Stand_Fast_BR", locomotion.backward_right);
        var newOverridesList = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var kvp in overridesList)
        {
            AnimationClip newClip = null;
            if (kvp.Key != null && overridesDict.TryGetValue(kvp.Key.name, out newClip))
                newOverridesList.Add(new System.Collections.Generic.KeyValuePair<AnimationClip, AnimationClip>(kvp.Key, newClip));
            else
                newOverridesList.Add(kvp);
        }
        controller.ApplyOverrides(newOverridesList);
    }
    
    private static void ApplyOverride(ref AnimationClipOverrides overrides, string key, AnimationClip clip)
    {
        if (overrides.ContainsKey(key) && clip != null) {
            overrides[key] = clip;
        } else {
            Debug.LogWarning($"L'animation '{key}' est manquante dans l'état de locomotion.");
        }
    }
    
    private static void EnsureAnimatorParameters(Animator animator)
    {
        string[] requiredParameters = {
            "Speed", "Speed-X", "Speed-Z", "Speed-Y", "Grounded", "Stand"
        };
        
        foreach (string param in requiredParameters)
        {
            if (!HasParameter(animator, param))
            {
                Debug.LogWarning($"L'Animator du personnage n'a pas le paramètre requis '{param}'");
            }
        }
    }
    
    private static bool HasParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }
}

public class AnimationClipOverrides : System.Collections.Generic.Dictionary<string, AnimationClip>
{
    public AnimationClipOverrides() : base() { }
    public AnimationClipOverrides(int capacity) : base(capacity) { }
}