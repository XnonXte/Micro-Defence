using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

namespace MicroDefense.UI
{
    /// <summary>
    /// Editor utility untuk menambahkan UIButtonSFX ke semua button di scene
    /// </summary>
    public class AddButtonSFXUtility : EditorWindow
    {
        [MenuItem("Tools/Micro Defense/Add Button SFX to All Buttons")]
        public static void AddSFXToAllButtons()
        {
            // Find all buttons in scene
            Button[] allButtons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
            
            int addedCount = 0;
            int skippedCount = 0;
            List<string> processedButtons = new List<string>();
            
            foreach (Button button in allButtons)
            {
                // Check if already has UIButtonSFX
                if (button.GetComponent<UIButtonSFX>() != null)
                {
                    skippedCount++;
                    continue;
                }
                
                // Add UIButtonSFX component
                button.gameObject.AddComponent<UIButtonSFX>();
                addedCount++;
                processedButtons.Add(button.gameObject.name);
                
                // Mark object as dirty for saving
                EditorUtility.SetDirty(button.gameObject);
            }
            
            // Show result
            string message = $"Added UIButtonSFX to {addedCount} buttons.\nSkipped {skippedCount} buttons (already have component).\n\nProcessed buttons:\n";
            foreach (string btnName in processedButtons)
            {
                message += $"- {btnName}\n";
            }
            
            Debug.Log($"[AddButtonSFXUtility] {message}");
            
            EditorUtility.DisplayDialog(
                "Add Button SFX Complete", 
                message, 
                "OK"
            );
        }
        
        [MenuItem("Tools/Micro Defense/Remove Button SFX from All Buttons")]
        public static void RemoveSFXFromAllButtons()
        {
            UIButtonSFX[] allSFXComponents = Object.FindObjectsByType<UIButtonSFX>(FindObjectsSortMode.None);
            
            int removedCount = 0;
            
            foreach (UIButtonSFX sfx in allSFXComponents)
            {
                Object.DestroyImmediate(sfx);
                removedCount++;
            }
            
            Debug.Log($"[AddButtonSFXUtility] Removed UIButtonSFX from {removedCount} buttons.");
            
            EditorUtility.DisplayDialog(
                "Remove Button SFX Complete", 
                $"Removed UIButtonSFX from {removedCount} buttons.", 
                "OK"
            );
        }
        
        [MenuItem("Tools/Micro Defense/List All Buttons in Scene")]
        public static void ListAllButtons()
        {
            Button[] allButtons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
            
            string message = $"Found {allButtons.Length} buttons in scene:\n\n";
            
            foreach (Button button in allButtons)
            {
                bool hasSFX = button.GetComponent<UIButtonSFX>() != null;
                string sfxStatus = hasSFX ? "[HAS SFX]" : "[NO SFX]";
                message += $"{sfxStatus} {button.gameObject.name}\n";
            }
            
            Debug.Log($"[AddButtonSFXUtility]\n{message}");
            
            EditorUtility.DisplayDialog(
                "All Buttons in Scene", 
                message, 
                "OK"
            );
        }
    }
}
