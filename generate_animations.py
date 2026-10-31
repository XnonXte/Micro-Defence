#!/usr/bin/env python3
"""
Tower Animation Setup Script
Generates animation clips and animator controllers for BCell, TCell, and Macrophage towers
"""

import os
import uuid
from pathlib import Path

# Paths
PROJECT_ROOT = r"C:\UnityGame\Micro-Defence\Assets"
ANIMATION_DIR = os.path.join(PROJECT_ROOT, "Animations")
SPRITE_SOURCE_DIR = os.path.join(PROJECT_ROOT, "Assets", "Assets", "Animation-20261030T132549Z-1-001", "Animation")

# Tower configurations
TOWERS = {
    "BCell": {
        "idle_sprite": "bcellBlink.png",
        "attack_sprite": "bcellShooting.png",
    },
    "TCell": {
        "idle_sprite": "tcellBlink.png",
        "attack_sprite": "tcellShooting.png",
    },
    "Macrophage": {
        "idle_sprite": "macroBlink.png",
        "attack_sprite": "macroBlink.png",  # Using blink for both if shooting not available
    }
}

def get_sprite_guid(sprite_name: str) -> str:
    """
    Load sprite file and extract/generate GUID
    For simplicity, we'll generate consistent GUIDs based on sprite name
    """
    sprite_path = os.path.join(SPRITE_SOURCE_DIR, sprite_name)
    if not os.path.exists(sprite_path):
        print(f"WARNING: Sprite not found: {sprite_path}")
        return "00000000000000000000000000000000"
    
    # Generate consistent GUID from filename
    import hashlib
    hash_obj = hashlib.md5(sprite_name.encode())
    guid = hash_obj.hexdigest()
    return guid

def create_animation_clip(tower_name: str, anim_type: str, sprite_name: str, duration: float, loop: bool):
    """Create animation clip YAML"""
    sprite_guid = get_sprite_guid(sprite_name)
    
    yaml_content = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!74 &7400000
AnimationClip:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: {tower_name}_{anim_type}
  serializedVersion: 6
  m_Legacy: 0
  m_Compressed: 0
  m_UseHighQualityCurve: 1
  m_RotationCurveKeyVersion: 2
  m_Events: []
  m_FloatCurves: []
  m_PPtrCurves:
  - curve:
    - time: 0
      value: {{fileID: 21300000, guid: {sprite_guid}, type: 3}}
    - time: {duration}
      value: {{fileID: 21300000, guid: {sprite_guid}, type: 3}}
    attribute: m_Sprite
    path: 
    classID: 212
    script: {{fileID: 0}}
  m_SampleRate: 60
  m_WrapMode: 0
  m_Bounds:
    m_Center: {{x: 0, y: 0, z: 0}}
    m_Extent: {{x: 0, y: 0, z: 0}}
  m_ClipBindingConstant:
    genericBindings:
    - serializedVersion: 2
      path: 0
      attribute: 0
      script: {{fileID: 0}}
      typeID: 212
      customType: 23
      isPPtrCurve: 1
    pptrCurveMapping:
    - {{fileID: 21300000, guid: {sprite_guid}, type: 3}}
    - {{fileID: 21300000, guid: {sprite_guid}, type: 3}}
  m_AnimationClipSettings:
    serializedVersion: 2
    m_AdditiveReferencePoseClip: {{fileID: 0}}
    m_AdditiveReferencePoseTime: 0
    m_StartTime: 0
    m_StopTime: {duration}
    m_OrientationOffsetY: 0
    m_Level: 0
    m_CycleOffset: 0
    m_HasAdditiveReferencePose: 0
    m_LoopTime: {1 if loop else 0}
    m_LoopBlend: 0
    m_LoopBlendOrientation: 0
    m_LoopBlendPositionY: 0
    m_LoopBlendPositionXZ: 0
    m_KeepOriginalOrientation: 0
    m_KeepOriginalPositionY: 1
    m_KeepOriginalPositionXZ: 0
    m_HeightFromFeet: 0
    m_Mirror: 0
  m_EditorCurves: []
  m_EulerEditorCurves: []
  m_HasGenericRootTransform: 0
  m_HasMotionFloatCurves: 0
  m_Events: []
"""
    return yaml_content

def create_animator_controller(tower_name: str, idle_guid: str, attack_guid: str):
    """Create animator controller YAML"""
    # Generate GUIDs for states and transitions
    layer_guid = str(uuid.uuid4()).replace("-", "")
    idle_state_guid = str(uuid.uuid4()).replace("-", "")
    attack_state_guid = str(uuid.uuid4()).replace("-", "")
    transition1_guid = str(uuid.uuid4()).replace("-", "")
    transition2_guid = str(uuid.uuid4()).replace("-", "")
    param_guid = str(uuid.uuid4()).replace("-", "")
    
    yaml_content = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!91 &9100000
AnimatorController:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: {tower_name}
  serializedVersion: 5
  m_AnimatorParameters:
  - m_Name: Attack
    m_Type: 9
    m_DefaultFloat: 0
    m_DefaultInt: 0
    m_DefaultBool: 0
    m_Controller: {{fileID: 0}}
  m_AnimatorLayers:
  - serializedVersion: 5
    m_Name: Base Layer
    m_StateMachine: {{fileID: {layer_guid}}}
    m_Mask: {{fileID: 0}}
    m_Motions: []
    m_Behaviours: []
    m_BlendingMode: 0
    m_SyncedLayerIndex: -1
    m_DefaultWeight: 0
    m_IKPass: 0
    m_SyncedLayerAffectsTiming: 0
    m_Controller: {{fileID: 9100000}}
--- !u!1102 &{idle_state_guid}
AnimatorState:
  serializedVersion: 6
  m_ObjectHideFlags: 1
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: Idle
  m_Speed: 1
  m_CycleOffset: 0
  m_Transitions:
  - {{fileID: {transition1_guid}}}
  m_StateMachineBehaviours: []
  m_Position: {{x: 50, y: 50, z: 0}}
  m_IKOnFeet: 0
  m_WriteDefaultValues: 0
  m_Mirror: 0
  m_SpeedParameterActive: 0
  m_MirrorParameterActive: 0
  m_CycleOffsetParameterActive: 0
  m_TimeParameterActive: 0
  m_Motion: {{fileID: {idle_guid}}}
  m_Tag: 
  m_SpeedParameter: 
  m_MirrorParameter: 
  m_CycleOffsetParameter: 
  m_TimeParameter: 
--- !u!1102 &{attack_state_guid}
AnimatorState:
  serializedVersion: 6
  m_ObjectHideFlags: 1
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: Attack
  m_Speed: 1
  m_CycleOffset: 0
  m_Transitions:
  - {{fileID: {transition2_guid}}}
  m_StateMachineBehaviours: []
  m_Position: {{x: 50, y: 50, z: 0}}
  m_IKOnFeet: 0
  m_WriteDefaultValues: 0
  m_Mirror: 0
  m_SpeedParameterActive: 0
  m_MirrorParameterActive: 0
  m_CycleOffsetParameterActive: 0
  m_TimeParameterActive: 0
  m_Motion: {{fileID: {attack_guid}}}
  m_Tag: 
  m_SpeedParameter: 
  m_MirrorParameter: 
  m_CycleOffsetParameter: 
  m_TimeParameter: 
--- !u!1101 &{transition1_guid}
AnimatorStateTransition:
  m_ObjectHideFlags: 1
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: 
  m_Conditions:
  - m_ConditionMode: 1
    m_ConditionEvent: Attack
    m_EventTreshold: 0
  m_DstStateMachine: {{fileID: 0}}
  m_DstState: {{fileID: {attack_state_guid}}}
  m_Solo: 0
  m_Mute: 0
  m_IsExit: 0
  serializedVersion: 3
  m_TransitionDuration: 0
  m_TransitionOffset: 0
  m_ExitTime: 0
  m_HasExitTime: 0
  m_HasFixedDuration: 1
  m_InterruptionSource: 0
  m_OrderedInterruption: 1
  m_CanTransitionToSelf: 1
--- !u!1101 &{transition2_guid}
AnimatorStateTransition:
  m_ObjectHideFlags: 1
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: 
  m_Conditions: []
  m_DstStateMachine: {{fileID: 0}}
  m_DstState: {{fileID: {idle_state_guid}}}
  m_Solo: 0
  m_Mute: 0
  m_IsExit: 0
  serializedVersion: 3
  m_TransitionDuration: 0
  m_TransitionOffset: 0
  m_ExitTime: 0.9
  m_HasExitTime: 1
  m_HasFixedDuration: 1
  m_InterruptionSource: 0
  m_OrderedInterruption: 1
  m_CanTransitionToSelf: 1
--- !u!1107 &{layer_guid}
AnimatorStateMachine:
  serializedVersion: 6
  m_ObjectHideFlags: 1
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: Base Layer
  m_ChildStates:
  - serializedVersion: 1
    m_State: {{fileID: {idle_state_guid}}}
    m_Position: {{x: 50, y: 50, z: 0}}
  - serializedVersion: 1
    m_State: {{fileID: {attack_state_guid}}}
    m_Position: {{x: 300, y: 0, z: 0}}
  m_ChildStateMachines: []
  m_AnyStateTransitions: []
  m_EntryTransitions: []
  m_StateMachineTransitions: {{}}
  m_StateMachineBehaviours: []
  m_AnyStatePosition: {{x: 50, y: -50, z: 0}}
  m_EntryPosition: {{x: 50, y: 120, z: 0}}
  m_ExitPosition: {{x: 800, y: 120, z: 0}}
  m_ParentStateMachinePosition: {{x: 800, y: 20, z: 0}}
  m_DefaultState: {{fileID: {idle_state_guid}}}
  m_DefaultState: {{fileID: {idle_state_guid}}}
"""
    return yaml_content

def main():
    # Ensure animations directory exists
    os.makedirs(ANIMATION_DIR, exist_ok=True)
    print(f"Animation directory: {ANIMATION_DIR}")
    
    # Generate animation files for each tower
    for tower_name, config in TOWERS.items():
        print(f"\nGenerating animations for {tower_name}...")
        
        # Get sprite GUIDs
        idle_sprite = config["idle_sprite"]
        attack_sprite = config["attack_sprite"]
        idle_guid = get_sprite_guid(idle_sprite)
        attack_guid = get_sprite_guid(attack_sprite)
        
        # Create Idle animation
        idle_anim = create_animation_clip(tower_name, "Idle", idle_sprite, 0.5, True)
        idle_path = os.path.join(ANIMATION_DIR, f"{tower_name}_Idle.anim")
        with open(idle_path, "w") as f:
            f.write(idle_anim)
        print(f"  [OK] Created {tower_name}_Idle.anim")
        
        # Create Attack animation
        attack_anim = create_animation_clip(tower_name, "Attack", attack_sprite, 0.3, False)
        attack_path = os.path.join(ANIMATION_DIR, f"{tower_name}_Attack.anim")
        with open(attack_path, "w") as f:
            f.write(attack_anim)
        print(f"  [OK] Created {tower_name}_Attack.anim")
        
        # Create Animator Controller
        controller = create_animator_controller(tower_name, idle_guid, attack_guid)
        controller_path = os.path.join(ANIMATION_DIR, f"{tower_name}.controller")
        with open(controller_path, "w") as f:
            f.write(controller)
        print(f"  [OK] Created {tower_name}.controller")

if __name__ == "__main__":
    main()
    print("\n[SUCCESS] All animations generated successfully!")
