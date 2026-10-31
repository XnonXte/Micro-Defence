#!/usr/bin/env python3
"""
Fix Animator Controllers - Update animation clip references with proper GUIDs
"""

import os

PROJECT_ROOT = r"C:\UnityGame\Micro-Defence\Assets"
ANIMATION_DIR = os.path.join(PROJECT_ROOT, "Animations")

# Animation GUIDs
ANIMATION_GUIDS = {
    "BCell": {
        "idle": "23f4441a5bc1ead43b92413733077245",
        "attack": "f505824e08573854b8b79735c7787b1c",
    },
    "TCell": {
        "idle": "d6a6798a106db1844b356faa3b77faf8",
        "attack": "17ab230aefc4cfd478ad34e5bd049ef3",
    },
    "Macrophage": {
        "idle": "c6e3858c0f716ac4fb4adee41c591c1c",
        "attack": "1a6d001cf078c1e46b2d250c37f74da6",
    }
}

def create_animator_controller(tower_name: str, idle_guid: str, attack_guid: str):
    """Create animator controller with correct animation GUIDs"""
    
    idle_state_id = 1102 + hash(tower_name + "Idle") % 9000
    attack_state_id = 1102 + hash(tower_name + "Attack") % 9000
    transition1_id = 1101 + hash(tower_name + "T1") % 9000
    transition2_id = 1101 + hash(tower_name + "T2") % 9000
    statemachine_id = 1107 + hash(tower_name + "SM") % 9000
    
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
    m_StateMachine: {{fileID: {statemachine_id}}}
    m_Mask: {{fileID: 0}}
    m_Motions: []
    m_Behaviours: []
    m_BlendingMode: 0
    m_SyncedLayerIndex: -1
    m_DefaultWeight: 0
    m_IKPass: 0
    m_SyncedLayerAffectsTiming: 0
    m_Controller: {{fileID: 9100000}}
--- !u!1102 &{idle_state_id}
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
  - {{fileID: {transition1_id}}}
  m_StateMachineBehaviours: []
  m_Position: {{x: 50, y: 50, z: 0}}
  m_IKOnFeet: 0
  m_WriteDefaultValues: 0
  m_Mirror: 0
  m_SpeedParameterActive: 0
  m_MirrorParameterActive: 0
  m_CycleOffsetParameterActive: 0
  m_TimeParameterActive: 0
  m_Motion: {{fileID: 7400000, guid: {idle_guid}, type: 2}}
  m_Tag: 
  m_SpeedParameter: 
  m_MirrorParameter: 
  m_CycleOffsetParameter: 
  m_TimeParameter: 
--- !u!1102 &{attack_state_id}
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
  - {{fileID: {transition2_id}}}
  m_StateMachineBehaviours: []
  m_Position: {{x: 50, y: 50, z: 0}}
  m_IKOnFeet: 0
  m_WriteDefaultValues: 0
  m_Mirror: 0
  m_SpeedParameterActive: 0
  m_MirrorParameterActive: 0
  m_CycleOffsetParameterActive: 0
  m_TimeParameterActive: 0
  m_Motion: {{fileID: 7400000, guid: {attack_guid}, type: 2}}
  m_Tag: 
  m_SpeedParameter: 
  m_MirrorParameter: 
  m_CycleOffsetParameter: 
  m_TimeParameter: 
--- !u!1101 &{transition1_id}
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
  m_DstState: {{fileID: {attack_state_id}}}
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
--- !u!1101 &{transition2_id}
AnimatorStateTransition:
  m_ObjectHideFlags: 1
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: 
  m_Conditions: []
  m_DstStateMachine: {{fileID: 0}}
  m_DstState: {{fileID: {idle_state_id}}}
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
--- !u!1107 &{statemachine_id}
AnimatorStateMachine:
  serializedVersion: 6
  m_ObjectHideFlags: 1
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: Base Layer
  m_ChildStates:
  - serializedVersion: 1
    m_State: {{fileID: {idle_state_id}}}
    m_Position: {{x: 50, y: 50, z: 0}}
  - serializedVersion: 1
    m_State: {{fileID: {attack_state_id}}}
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
  m_DefaultState: {{fileID: {idle_state_id}}}
"""
    
    return yaml_content

def main():
    print("[AnimatorFix] Updating animator controllers with correct animation GUIDs...\n")
    
    for tower_name, guids in ANIMATION_GUIDS.items():
        controller_path = os.path.join(ANIMATION_DIR, f"{tower_name}.controller")
        yaml_content = create_animator_controller(tower_name, guids["idle"], guids["attack"])
        
        with open(controller_path, "w") as f:
            f.write(yaml_content)
        
        print(f"[OK] Updated {tower_name}.controller")
        print(f"     Idle: {guids['idle']}")
        print(f"     Attack: {guids['attack']}\n")
    
    print("[SUCCESS] All animator controllers updated with correct GUIDs!")

if __name__ == "__main__":
    main()
