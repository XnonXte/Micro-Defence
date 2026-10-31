#!/usr/bin/env python3
"""
Direct Prefab YAML Updater
Assigns animator controllers to tower prefabs by updating YAML references
"""

import os
import re

PROJECT_ROOT = r"C:\UnityGame\Micro-Defence\Assets"
PREFABS_DIR = os.path.join(PROJECT_ROOT, "Prefabs")

# Controllers dengan file IDs (generated saat dibuat)
CONTROLLERS = {
    "BCell.controller": "91100000",
    "TCell.controller": "91100001", 
    "Macrophage.controller": "91100002",
}

TOWER_PREFABS = {
    "Bcell.prefab": "BCell.controller",
    "T cell.prefab": "TCell.controller",
    "Macho tower.prefab": "Macrophage.controller",
}

def get_controller_file_id(controller_name: str) -> str:
    """Get file ID from controller file"""
    controller_path = os.path.join(PROJECT_ROOT, "Animations", controller_name)
    
    if not os.path.exists(controller_path):
        print(f"[ERROR] Controller not found: {controller_path}")
        return None
    
    # Read first line to extract fileID (format: --- !u!91 &12345678)
    try:
        with open(controller_path, 'r') as f:
            for line in f:
                if '!u!91 &' in line:
                    # Extract fileID from: --- !u!91 &12345678
                    match = re.search(r'&(\d+)', line)
                    if match:
                        return match.group(1)
    except Exception as e:
        print(f"[ERROR] Failed to read controller {controller_name}: {e}")
    
    return None

def update_prefab(prefab_path: str, controller_file_id: str, controller_name: str):
    """Update prefab YAML to reference animator controller"""
    
    if not os.path.exists(prefab_path):
        print(f"[ERROR] Prefab not found: {prefab_path}")
        return False
    
    try:
        with open(prefab_path, 'r', encoding='utf-8') as f:
            content = f.read()
        
        # Find Animator component and update controller reference
        # Look for pattern: m_Controller: {fileID: 0} or similar
        
        # Strategy: Find the Animator section and inject/update controller reference
        lines = content.split('\n')
        updated = False
        result_lines = []
        
        i = 0
        while i < len(lines):
            line = lines[i]
            
            # Look for Animator component (type: 93)
            if '!u!93 &' in line or ('m_Component:' in line and i < len(lines) - 5):
                # Check if this is Animator component
                result_lines.append(line)
                i += 1
                
                # Scan ahead for controller field
                found_controller = False
                for j in range(i, min(i + 50, len(lines))):
                    if 'm_Controller:' in lines[j]:
                        # Replace controller reference
                        indent = len(lines[j]) - len(lines[j].lstrip())
                        result_lines.append(' ' * indent + f'm_Controller: {{fileID: {controller_file_id}, guid: 0, type: 2}}')
                        found_controller = True
                        updated = True
                        # Skip original line
                        i = j + 1
                        break
                
                if not found_controller:
                    # m_Controller field doesn't exist yet, add it after m_Enabled
                    for j in range(i, min(i + 20, len(lines))):
                        result_lines.append(lines[j])
                        if 'm_Enabled:' in lines[j]:
                            # Add controller reference after enabled
                            indent = len(lines[j]) - len(lines[j].lstrip())
                            result_lines.append(' ' * indent + f'm_Controller: {{fileID: {controller_file_id}, guid: 0, type: 2}}')
                            updated = True
                            i = j + 1
                            break
            else:
                result_lines.append(line)
                i += 1
        
        if updated:
            with open(prefab_path, 'w', encoding='utf-8') as f:
                f.write('\n'.join(result_lines))
            print(f"[OK] Updated {os.path.basename(prefab_path)}")
            return True
        else:
            print(f"[SKIP] No Animator found in {os.path.basename(prefab_path)}")
            return False
            
    except Exception as e:
        print(f"[ERROR] Failed to update {prefab_path}: {e}")
        return False

def main():
    print("[PrefabAnimatorUpdater] Starting prefab updates...\n")
    
    for prefab_name, controller_name in TOWER_PREFABS.items():
        print(f"Processing {prefab_name}...")
        
        # Get controller file ID
        controller_file_id = get_controller_file_id(controller_name)
        if not controller_file_id:
            print(f"[ERROR] Could not get file ID for {controller_name}\n")
            continue
        
        prefab_path = os.path.join(PREFABS_DIR, prefab_name)
        update_prefab(prefab_path, controller_file_id, controller_name)
        print()
    
    print("[SUCCESS] Prefab animator assignment complete!")

if __name__ == "__main__":
    main()
