#!/usr/bin/env python3
"""
Tower Animation Setup Script v2
Generates animation clips with 5 frames per animation from spritesheet
"""

import os
import uuid
from pathlib import Path

# Paths
PROJECT_ROOT = r"C:\UnityGame\Micro-Defence\Assets"
ANIMATION_DIR = os.path.join(PROJECT_ROOT, "Animations")
SPRITE_SOURCE_DIR = os.path.join(PROJECT_ROOT, "Assets", "Assets", "Animation-20261030T132549Z-1-001", "Animation")

# Tower configurations - each sprite has 5 frames
TOWERS = {
    "BCell": {
        "idle_sprite": "bcellBlink.png",
        "attack_sprite": "bcellShooting.png",
        "frame_count": 5,
    },
    "TCell": {
        "idle_sprite": "tcellBlink.png",
        "attack_sprite": "tcellShooting.png",
        "frame_count": 5,
    },
    "Macrophage": {
        "idle_sprite": "macroBlink.png",
        "attack_sprite": "macroBlink.png",
        "frame_count": 5,
    }
}

def get_sprite_guid(sprite_name: str) -> str:
    """Generate consistent GUID from sprite name"""
    import hashlib
    hash_obj = hashlib.md5(sprite_name.encode())
    guid = hash_obj.hexdigest()
    return guid

def create_animation_clip_multiframe(tower_name: str, anim_type: str, sprite_name: str, frame_count: int, duration: float, loop: bool):
    """Create animation clip YAML with multiple frames"""
    sprite_guid = get_sprite_guid(sprite_name)
    
    # Generate frame keyframes
    frame_interval = duration / frame_count
    
    # Build PPtrCurve for each frame
    curve_frames = []
    for i in range(frame_count):
        time = i * frame_interval
        curve_frames.append(f"    - time: {time}\n      value: {{fileID: {i}, guid: {sprite_guid}, type: 3}}")
    
    curve_str = "\n".join(curve_frames)
    
    # Build pptrCurveMapping
    mapping_items = []
    for i in range(frame_count):
        mapping_items.append(f"    - {{fileID: {i}, guid: {sprite_guid}, type: 3}}")
    
    mapping_str = "\n".join(mapping_items)
    
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
{curve_str}
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
{mapping_str}
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

def main():
    # Ensure animations directory exists
    os.makedirs(ANIMATION_DIR, exist_ok=True)
    print(f"[AnimationSetup] Animation directory: {ANIMATION_DIR}\n")
    
    # Generate animation files for each tower
    for tower_name, config in TOWERS.items():
        print(f"[AnimationSetup] Generating animations for {tower_name}...")
        
        idle_sprite = config["idle_sprite"]
        attack_sprite = config["attack_sprite"]
        frame_count = config["frame_count"]
        
        # Idle animation: 1 second loop (5 frames = 0.2s per frame)
        idle_anim = create_animation_clip_multiframe(tower_name, "Idle", idle_sprite, frame_count, 1.0, True)
        idle_path = os.path.join(ANIMATION_DIR, f"{tower_name}_Idle.anim")
        with open(idle_path, "w") as f:
            f.write(idle_anim)
        print(f"  [OK] Created {tower_name}_Idle.anim (5 frames, 1.0s loop)")
        
        # Attack animation: 0.5 second (5 frames = 0.1s per frame)
        attack_anim = create_animation_clip_multiframe(tower_name, "Attack", attack_sprite, frame_count, 0.5, False)
        attack_path = os.path.join(ANIMATION_DIR, f"{tower_name}_Attack.anim")
        with open(attack_path, "w") as f:
            f.write(attack_anim)
        print(f"  [OK] Created {tower_name}_Attack.anim (5 frames, 0.5s no-loop)")
        print()

if __name__ == "__main__":
    main()
    print("[SUCCESS] All animations regenerated with 5 frames!")
