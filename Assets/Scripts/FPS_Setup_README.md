# FPS Weapon System Setup Guide

This guide will help you set up a complete FPS weapon system in Unity with projectile shooting, weapon sway, recoil, and dynamic crosshair.

## 📋 Prerequisites

- Unity 2021.3 or later
- Basic understanding of Unity's GameObject hierarchy
- The following scripts in your project:
  - `WeaponController.cs`
  - `Projectile.cs`
  - `Target.cs`
  - `Crosshair.cs`

## 🎮 Player Setup

### 1. Player GameObject Structure
Your player hierarchy should look like this:
```
Player (GameObject)
├── PlayerBody (with Movement.cs and Rigidbody)
└── CameraHolder
    └── Main Camera (with CameraController.cs)
        └── WeaponHolder
            └── GunModel
                └── FirePoint (Empty GameObject)
```

### 2. Setting up the Weapon
1. **Create Weapon Holder**:
   - Right-click on your Main Camera
   - Create Empty GameObject, name it "WeaponHolder"
   - Position it at (0.4, -0.3, 0.5) for a typical FPS view

2. **Add Your Gun Model**:
   - Import or create a gun model
   - Place it as a child of WeaponHolder
   - Adjust position/rotation to look correct in Game view

3. **Add Fire Point**:
   - Create an empty GameObject at the gun's barrel tip
   - Name it "FirePoint"
   - Make sure its Z-axis (blue arrow) points forward

4. **Attach WeaponController Script**:
   - Add `WeaponController.cs` to your gun model
   - Configure the following in the Inspector:
     - **Fire Point**: Drag the FirePoint GameObject
     - **Fire Rate**: 0.3 (3-4 shots per second)
     - **Projectile Speed**: 50
     - **Max Ammo**: 30

## 🔫 Projectile Setup

### 1. Create Projectile Prefab
1. Create a new Sphere (3D Object > Sphere)
2. Scale it down to (0.1, 0.1, 0.1)
3. Remove the Sphere Collider component
4. Add `Projectile.cs` script
5. Configure in Inspector:
   - **Damage**: 10
   - **Life Time**: 5
   - **Impact Force**: 30
   - **Use Trail**: ✓ (checked)

### 2. Save as Prefab
1. Drag the sphere into your Prefabs folder
2. Name it "Bullet" or "Projectile"
3. Delete the sphere from the scene
4. Assign this prefab to WeaponController's **Projectile Prefab** field

## 🎯 Crosshair Setup

1. Create an empty GameObject in your scene
2. Name it "CrosshairUI"
3. Add the `Crosshair.cs` script
4. Configure settings:
   - **Crosshair Size**: 20
   - **Crosshair Thickness**: 2
   - **Crosshair Gap**: 5
   - **Dynamic Crosshair**: ✓ (checked)

## 🎪 Target Setup (for Testing)

1. Create some Cube GameObjects in your scene
2. Space them at different distances
3. Add `Target.cs` script to each cube
4. Configure:
   - **Max Health**: 100
   - **Damage Color**: Red
   - **Destroy On Death**: ✓ (checked)

## ⚙️ Additional Configuration

### Audio Setup
1. Add an AudioSource component to your gun model
2. Import shooting and reload sound effects
3. Assign them to WeaponController:
   - **Fire Sound**: Your shooting sound
   - **Reload Sound**: Your reload sound

### Visual Effects
1. **Muzzle Flash**:
   - Create a Particle System as child of FirePoint
   - Configure it for a quick burst effect
   - Assign to WeaponController's **Muzzle Flash** field

2. **Impact Effects**:
   - Create a particle system prefab for bullet impacts
   - Assign to Projectile's **Impact Effect** field

### Recoil Settings
Fine-tune in WeaponController:
- **Recoil Amount**: 0.1 (increase for more kick)
- **Recoil Pattern**: (-0.1, 0.1, 0.05)
- **Recoil Speed**: 10
- **Recovery Speed**: 5

### Weapon Sway Settings
Adjust for feel:
- **Sway Amount**: 0.02
- **Max Sway**: 0.06
- **Smoothness**: 6
- **Rotation Sway**: 1

## 🏷️ Required Tags

Make sure these tags exist in your project:
- **"Player"** - Assign to your player GameObject

## 🎮 Controls

- **Left Mouse Button**: Shoot
- **R**: Reload
- **Mouse Movement**: Aim (with weapon sway)
- **WASD**: Move (affects crosshair spread)
- **Escape**: Unlock cursor

## 🐛 Troubleshooting

### Bullets not spawning
- Check that FirePoint is assigned
- Ensure Projectile Prefab is assigned
- Verify FirePoint's forward direction (blue arrow)

### No weapon sway
- Make sure CameraController.cs is on your camera
- Check that weapon is child of camera

### Crosshair not showing
- Ensure CrosshairUI GameObject is active
- Check that no UI Canvas is blocking it

### Targets not taking damage
- Verify targets have colliders
- Check that projectiles have trigger colliders
- Ensure Target.cs is attached

### No recoil effect
- Increase Recoil Amount value
- Check that camera has CameraController component

## 📝 Quick Setup Checklist

- [ ] Player hierarchy set up correctly
- [ ] WeaponController attached to gun model
- [ ] FirePoint positioned at barrel tip
- [ ] Projectile prefab created and assigned
- [ ] Crosshair GameObject created with script
- [ ] Test targets placed in scene
- [ ] Player GameObject tagged as "Player"
- [ ] Audio sources configured (optional)
- [ ] Particle effects created (optional)

## 🎨 Customization Tips

1. **Different Weapons**: Duplicate the gun setup and modify stats
2. **Projectile Types**: Create variations with different speeds/damage
3. **Crosshair Styles**: Modify the OnGUI method in Crosshair.cs
4. **Recoil Patterns**: Adjust the recoilPattern Vector3 for different guns

Happy shooting! 🎯 