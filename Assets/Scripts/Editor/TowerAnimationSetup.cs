using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

namespace MicroDefense.Editor
{
    public class TowerAnimationSetup : MonoBehaviour
    {
        private const string ANIMATION_PATH = "Assets/Animations";
        private const string SPRITE_PATH = "Assets/Assets/Assets/Animation-20261030T132549Z-1-001/Animation";

        [MenuItem("Tools/Micro-Defense/Setup Tower Animations")]
        public static void SetupAllTowerAnimations()
        {
            Debug.Log("[TowerAnimationSetup] Starting tower animation setup...");

            // Ensure animations folder exists
            if (!AssetDatabase.IsValidFolder(ANIMATION_PATH))
            {
                AssetDatabase.CreateFolder("Assets", "Animations");
            }

            // Setup each tower
            SetupTowerAnimations("BCell", "bcellBlink.png", "bcellShooting.png");
            SetupTowerAnimations("TCell", "tcellBlink.png", "tcellShooting.png");
            SetupTowerAnimations("Macrophage", "macroBlink.png", "macroBlink.png");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[TowerAnimationSetup] Tower animation setup completed!");
        }

        private static void SetupTowerAnimations(string towerName, string idleFrameName, string attackFrameName)
        {
            Debug.Log($"[TowerAnimationSetup] Setting up {towerName} animations...");

            // Load sprite assets
            Sprite idleSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITE_PATH}/{idleFrameName}");
            Sprite attackSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITE_PATH}/{attackFrameName}");

            if (idleSprite == null)
            {
                Debug.LogError($"[TowerAnimationSetup] Failed to load idle sprite: {idleFrameName}");
                return;
            }

            if (attackSprite == null)
            {
                Debug.LogError($"[TowerAnimationSetup] Failed to load attack sprite: {attackFrameName}");
                return;
            }

            // Create Idle animation
            AnimationClip idleClip = CreateAnimationClip($"{towerName}_Idle", idleSprite, 1.0f, true);
            string idleAnimPath = $"{ANIMATION_PATH}/{towerName}_Idle.anim";
            AssetDatabase.CreateAsset(idleClip, idleAnimPath);
            Debug.Log($"[TowerAnimationSetup] Created {idleAnimPath}");

            // Create Attack animation
            AnimationClip attackClip = CreateAnimationClip($"{towerName}_Attack", attackSprite, 0.3f, false);
            string attackAnimPath = $"{ANIMATION_PATH}/{towerName}_Attack.anim";
            AssetDatabase.CreateAsset(attackClip, attackAnimPath);
            Debug.Log($"[TowerAnimationSetup] Created {attackAnimPath}");

            // Create Animator Controller
            CreateAnimatorController(towerName, idleClip, attackClip);
        }

        private static AnimationClip CreateAnimationClip(string clipName, Sprite sprite, float duration, bool loop)
        {
            AnimationClip clip = new AnimationClip();
            clip.name = clipName;

            // Set animation frame
            var keyframes = new ObjectReferenceKeyframe[2];
            keyframes[0] = new ObjectReferenceKeyframe { time = 0f, value = sprite };
            keyframes[1] = new ObjectReferenceKeyframe { time = duration, value = sprite };

            var curve = new AnimationCurve();
            EditorCurveBinding binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

            // Set loop
            SerializedObject serializedClip = new SerializedObject(clip);
            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = loop;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            clip.frameRate = 30f;

            return clip;
        }

        private static void CreateAnimatorController(string towerName, AnimationClip idleClip, AnimationClip attackClip)
        {
            string controllerPath = $"{ANIMATION_PATH}/{towerName}.controller";

            // Check if controller already exists
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if (controller != null)
            {
                Debug.Log($"[TowerAnimationSetup] Animator controller already exists: {controllerPath}");
                return;
            }

            // Create new controller
            controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            Debug.Log($"[TowerAnimationSetup] Created {controllerPath}");

            // Get root state machine
            AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

            // Create Idle state
            AnimatorState idleState = rootStateMachine.AddState("Idle", Vector3.zero);
            idleState.motion = idleClip;
            idleState.cycleOffset = 0f;
            rootStateMachine.defaultState = idleState;

            // Create Attack state
            AnimatorState attackState = rootStateMachine.AddState("Attack", new Vector3(300f, 0f, 0f));
            attackState.motion = attackClip;

            // Create transitions
            // Idle → Attack (on Attack trigger)
            AnimatorStateTransition idleToAttack = idleState.AddTransition(attackState);
            idleToAttack.AddCondition(AnimatorConditionMode.If, 0f, "Attack");
            idleToAttack.duration = 0f;
            idleToAttack.exitTime = 0f;

            // Attack → Idle (on end of animation)
            AnimatorStateTransition attackToIdle = attackState.AddTransition(idleState);
            attackToIdle.duration = 0f;
            attackToIdle.exitTime = 0.9f;
            attackToIdle.hasExitTime = true;

            // Add Attack trigger parameter
            controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);

            Debug.Log($"[TowerAnimationSetup] Setup animator controller for {towerName}");
        }
    }
}
