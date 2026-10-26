================================================================================
PSX Retro 3D Shader for Unity URP (Unity 6 / 6.5 Ready)
================================================================================

Thank you for purchasing / downloading this shader!
This package provides an authentic retro PlayStation 1 aesthetic designed 
specifically for the Universal Render Pipeline (URP).


--- COMPATIBILITY ---
• Render Pipeline: Universal Render Pipeline (URP)
• Unity Version: Unity 6 / 6.5+ (Optimized for URP Forward+) & Unity 2022.3 LTS (URP)
• Target Platforms: PC / Mac / WebGL / Mobile / Consoles


--- QUICK START GUIDE ---
1. Make sure your project is set up with Universal Render Pipeline (URP).
2. Create a new Material in your project (Right-click > Create > Material).
3. Set the Shader to:
   "Custom/PSXRetro_URP"
   (or duplicate the included 'M_PSXRetro_Default' material).
4. Assign your Base Map texture and adjust the settings in the Inspector.
5. (Tip): For the best retro look, set your texture's 'Filter Mode' to 'Point (no filter)' in its Texture Import Settings.


--- PARAMETERS GUIDE ---

[Surface & Maps]
• Base Map & Tint: Main diffuse texture and color tint.
• Cull Mode: Polygon culling (Back, Front, or Off for double-sided).
• Normal Map & Strength: Tangent-space normal mapping. (Set Strength to 0 if no normal map is used).
• Emission Map & Color: Self-illumination color and texture.

[PSX Retro Effects]
• Vertex Snap: Resolution of the fixed-point vertex grid. Lower values create more dramatic wobbling/jittering. (Default: 160)
• Affine Strength: Simulates perspective-incorrect affine texture warping. 1 = Full PS1 distortion, 0 = Modern perspective.
• Color Levels: Quantizes colors per RGB channel to emulate retro color depth. (Default: 16)
• Dither Strength: Controls the intensity of the 4x4 Bayer dithering matrix. (Default: 0.5)

[Lighting]
• Ambient Boost: Adds flat ambient lift to shadows, preventing completely pitch-black areas.


--- PACKAGE CONTENTS ---
• PSXRetro_URP.shader         - Core HLSL URP Shader
• Editor/PSXRetroShaderGUI.cs - Custom Inspector GUI
• Material/                   - Ready-to-use default preset
• README.txt                  - Documentation & usage instructions


--- LICENSE & SUPPORT ---
• You are free to use this asset in both commercial and non-commercial games/projects.
• Reselling, redistributing, or sublicensing the raw shader source files is prohibited.
• Support / Feedback: [18189useclip@gmail.com]

Enjoy building your retro project! o(*￣▽￣*)ブ
================================================================================