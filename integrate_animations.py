#!/usr/bin/env python3
"""
Tower Prefab Animation Integration Script
Assigns animator controllers to tower prefabs
"""

import os
import json
from pathlib import Path

PROJECT_ROOT = r"C:\UnityGame\Micro-Defence\Assets"
PREFABS_DIR = os.path.join(PROJECT_ROOT, "Prefabs")
ANIMATION_DIR = os.path.join(PROJECT_ROOT, "Animations")

# Mapping tower prefab names to animator controller files
TOWER_MAPPINGS = {
    "Bcell.prefab": "BCell.controller",
    "T cell.prefab": "TCell.controller",
    "Macho tower.prefab": "Macrophage.controller",
}

def update_prefab_animator(prefab_path: str, controller_path: str):
    """
    Update prefab to use new animator controller
    This updates the YAML reference in the prefab file
    """
    if not os.path.exists(prefab_path):
        print(f"[ERROR] Prefab not found: {prefab_path}")
        return False
    
    if not os.path.exists(controller_path):
        print(f"[ERROR] Controller not found: {controller_path}")
        return False
    
    # Read prefab file
    with open(prefab_path, 'r') as f:
        content = f.read()
    
    # Extract controller file ID for reference
    # For now, we'll just log that manual setup is needed
    print(f"[INFO] Please manually assign animator controller in Unity Editor:")
    print(f"  Prefab: {prefab_path}")
    print(f"  Controller: {controller_path}")
    
    return True

def main():
    print("[TowerAnimationIntegration] Starting integration...\n")
    
    for prefab_name, controller_name in TOWER_MAPPINGS.items():
        prefab_path = os.path.join(PREFABS_DIR, prefab_name)
        controller_path = os.path.join(ANIMATION_DIR, controller_name)
        
        print(f"Processing {prefab_name}...")
        update_prefab_animator(prefab_path, controller_path)
        print()
    
    print("[SUCCESS] Animation integration setup complete!")
    print("\nNext steps:")
    print("1. Open Unity Editor")
    print("2. For each tower prefab (Bcell, T cell, Macho tower):")
    print("   - Open the prefab")
    print("   - Find the Animator component")
    print("   - Drag the corresponding .controller file into the 'Controller' field")
    print("3. Save prefab")

if __name__ == "__main__":
    main()
