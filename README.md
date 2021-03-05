# Dynamic Decals
==========
[Unity Forums](https://forum.unity.com/threads/released-dynamic-decals.450820/)

This solution was developed by Llockham-Industries and originally sold in the Unity Asset Store. Dynamic Decals was deprecated when the new Unity render pipeline was launched, but the original author [gave his permission](https://forum.unity.com/threads/released-dynamic-decals.450820/page-17#post-6814172) to open source the project as it still works for Unity's Built-in Render Pipeline.

![Dynamic Decals](dynamic-decals.jpg)

## Introduction

Dynamic Decals is the decal solution built from the ground up to be fast, easy to use and flexible. Perfect for bullet holes, blood effects, projected UI elements and just about anything else you can think of!

No more fiddling with quads to avoid z-fighting, or re-projecting / recalculating decal meshes every time you want to move a decal. Just drop a projection renderer onto an empty gameObject and setup your projection. You can save it as a prefab and spawn it like any other gameObject, as well as move, rotate and scale it at run-time without issue. It's that easy.

On top of this, I've built a fast fully featured and flexible pooling system, a layered masking system and some "Printers" & "Positioners" to help users with no programming experience use the system.

<strong>New Version 2.0 Features</strong>
- GPU instancing with per-instance properties and atlassing.
- Animated projections.
- Additive & Multiplicative projections.
- Masking via Unity layers.

<strong>Key Features</strong>
- Print and manipulate decals at run-time.
- Project decals onto skinned meshes.
- Overlap and order decals.
- Clean, simple editor integration.
- Run-time Printers & Positioners.
- Omni-directional Projections.
- Mobile & VR support
- Support for Deferred & Forward rendering paths & all shader models.
- Full source code included.
- Zero setup required.
