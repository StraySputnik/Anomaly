using UnityEngine;
using UnityEditor;

public class PSXRetroShaderGUI : ShaderGUI
{
    private bool showSurface = true;
    private bool showRetro = true;
    private bool showLighting = true;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        MaterialProperty mainTex = FindProperty("_MainTex", properties);
        MaterialProperty color = FindProperty("_Color", properties);
        MaterialProperty cull = FindProperty("_Cull", properties);

        MaterialProperty bumpMap = FindProperty("_BumpMap", properties);
        MaterialProperty bumpStrength = FindProperty("_BumpStrength", properties);

        MaterialProperty emissionColor = FindProperty("_EmissionColor", properties);
        MaterialProperty emissionMap = FindProperty("_EmissionMap", properties);

        MaterialProperty vertexSnap = FindProperty("_VertexSnap", properties);
        MaterialProperty affineStrength = FindProperty("_AffineStrength", properties);
        MaterialProperty colorLevels = FindProperty("_ColorLevels", properties);
        MaterialProperty ditherStrength = FindProperty("_DitherStrength", properties);

        MaterialProperty ambientBoost = FindProperty("_AmbientBoost", properties);

        showSurface = EditorGUILayout.BeginFoldoutHeaderGroup(showSurface, "Surface & Maps");
        if (showSurface)
        {
            EditorGUILayout.Space(2);
            materialEditor.TexturePropertySingleLine(new GUIContent("Base Map"), mainTex, color);
            materialEditor.TextureScaleOffsetProperty(mainTex);
            materialEditor.ShaderProperty(cull, new GUIContent("Cull Mode"));

            EditorGUILayout.Space(4);
            materialEditor.TexturePropertySingleLine(new GUIContent("Normal Map"), bumpMap, bumpStrength);

            EditorGUILayout.Space(4);
            materialEditor.TexturePropertySingleLine(new GUIContent("Emission Map"), emissionMap, emissionColor);
            EditorGUILayout.Space(6);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        showRetro = EditorGUILayout.BeginFoldoutHeaderGroup(showRetro, "PSX Retro Effects");
        if (showRetro)
        {
            EditorGUILayout.Space(2);
            materialEditor.ShaderProperty(vertexSnap, new GUIContent("Vertex Snap (Lower = Wobblier)"));
            materialEditor.ShaderProperty(affineStrength, new GUIContent("Affine Strength (Warp)"));
            materialEditor.ShaderProperty(colorLevels, new GUIContent("Color Levels"));
            materialEditor.ShaderProperty(ditherStrength, new GUIContent("Dither Strength"));
            EditorGUILayout.Space(6);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        showLighting = EditorGUILayout.BeginFoldoutHeaderGroup(showLighting, "Lighting");
        if (showLighting)
        {
            EditorGUILayout.Space(2);
            materialEditor.ShaderProperty(ambientBoost, new GUIContent("Ambient Boost"));
            EditorGUILayout.Space(6);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}