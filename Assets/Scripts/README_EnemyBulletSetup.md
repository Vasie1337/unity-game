# Enemy Bullet Prefab Setup Guide

This guide will walk you through manually creating a bullet prefab for the Enemy script in Unity using only built-in Unity components.

## Step 1: Create the Bullet GameObject

1. In Unity, right-click in the Hierarchy window
2. Select **Create Empty** to create a new empty GameObject
3. Rename it to "EnemyBullet"

## Step 2: Add Visual Components

### Add 3D Model
1. With the EnemyBullet selected, click **Add Component** in the Inspector
2. Add **Mesh Filter** component
3. Add **Mesh Renderer** component
4. In the Mesh Filter component, click the circle next to "Mesh" and select **Sphere**

### Scale the Bullet
1. In the Transform component, set Scale to (0.2, 0.2, 0.2) for a small bullet
2. Or adjust to your preferred size

## Step 3: Create and Apply Material

1. In the Project window, right-click and select **Create > Material**
2. Name it "BulletMaterial"
3. Set the following properties:
   - **Albedo Color**: Orange (RGB: 255, 128, 0) or your preferred color
   - **Metallic**: 0.5
   - **Smoothness**: 0.8
   - **Emission**: Enable and set to the same orange color with intensity 1
4. Drag the material onto the EnemyBullet GameObject or assign it in the Mesh Renderer component

## Step 4: Add Physics Components

### Add Rigidbody
1. Select the EnemyBullet GameObject
2. Click **Add Component**
3. Add **Rigidbody** component
4. Configure the Rigidbody:
   - **Mass**: 0.1
   - **Drag**: 0
   - **Angular Drag**: 0
   - **Use Gravity**: Unchecked (bullets fly straight)
   - **Collision Detection**: Continuous (for fast-moving bullets)

### Add Collider
1. Click **Add Component**
2. Add **Sphere Collider**
3. Configure the collider:
   - **Is Trigger**: Checked (for detecting hits without physics collision)
   - **Radius**: 0.5 (adjusts with scale automatically)

## Step 5: Add Visual Effects (Optional)

### Add Light for Glow Effect
1. Right-click the EnemyBullet GameObject
2. Select **Light > Point Light**
3. Configure the light:
   - **Color**: Same as bullet (orange)
   - **Intensity**: 2
   - **Range**: 2
   - **Shadow Type**: None (for performance)

### Add Trail Renderer (Optional)
1. Select the EnemyBullet GameObject
2. Click **Add Component**
3. Add **Trail Renderer**
4. Configure for a bullet trail:
   - **Time**: 0.2
   - **Start Width**: 0.1
   - **End Width**: 0.02
   - **Material**: Create a new material with Sprites/Default shader
   - **Color**: Gradient from orange to transparent

## Step 6: Create the Prefab

1. Drag the EnemyBullet GameObject from the Hierarchy to the Project window
2. This creates a prefab asset
3. Delete the original EnemyBullet from the Hierarchy (the prefab is saved)

## Step 7: Configure the Enemy

1. Select your Enemy GameObject in the scene
2. In the Enemy component, find the **Bullet Prefab** field
3. Drag the EnemyBullet prefab from the Project window to this field

## Important Note: Bullet Behavior

Since you're not using the EnemyBullet script, the Enemy script will handle:
- Setting the bullet's velocity when spawned
- Destroying the bullet after its lifetime
- Damage dealing through collision detection

Make sure your Enemy.cs script includes the EnemyBullet class at the bottom (it's already included in the provided Enemy script).

## Step 8: Create Hit Effect Prefab (Optional)

1. Create a new empty GameObject, name it "BulletHitEffect"
2. Add a **Particle System** component
3. Configure for an impact effect:
   - **Duration**: 0.5
   - **Start Lifetime**: 0.3
   - **Start Speed**: 5
   - **Start Size**: 0.2
   - **Emission**: Set Bursts with Count: 20
   - **Shape**: Sphere with Radius: 0.1
   - **Color over Lifetime**: Fade from orange to transparent
4. Save as prefab

## Enemy Configuration Tips

### Combat Settings
- **Detection Range**: 20 (units) - How far the enemy can detect the player
- **Attack Range**: 15 (units) - How close the enemy needs to be to shoot
- **Fire Rate**: 1 (second) - Time between shots
- **Bullet Speed**: 20 - How fast bullets travel
- **Bullet Damage**: 10 - Damage per hit
- **Bullet Lifetime**: 5 (seconds) - Bullets destroy after this time

### Movement Settings
- **Move Speed**: 3 - How fast the enemy moves
- **Rotation Speed**: 5 - How quickly the enemy turns to face player

## Troubleshooting

### Bullets Not Moving
- Ensure the Rigidbody component is added to the bullet prefab
- Check that Use Gravity is unchecked
- Verify the Enemy script is setting the bullet velocity

### Bullets Not Spawning
- Ensure the Bullet Prefab field is assigned in the Enemy component
- Check that the player GameObject has the "Player" tag
- Verify the enemy can see the player (no obstacles blocking line of sight)

### Bullets Not Hitting Player
- Ensure the bullet has a Sphere Collider with Is Trigger checked
- Check that the player has a Collider component
- Verify the player has the "Player" tag

### Enemy Not Detecting Player
- Check the player tag is exactly "Player" (case-sensitive)
- Increase Detection Range if needed
- Ensure no layers are blocking the line of sight check

## Layer Setup (Important)

1. Create layers for organization:
   - Layer 6: "Player"
   - Layer 7: "Enemy"
   - Layer 8: "EnemyBullet"

2. Configure Physics collision matrix (Edit > Project Settings > Physics):
   - Uncheck EnemyBullet vs Enemy (bullets don't hit enemies)
   - Keep EnemyBullet vs Player checked

## Testing

1. Place an Enemy in your scene
2. Ensure your player has the "Player" tag
3. Run the game and approach the enemy
4. The enemy should:
   - Turn red when detecting you
   - Rotate to face you
   - Shoot bullets at you when in range
   - Move closer if you're too far away

## Performance Optimization

- Limit the number of enemies in the scene
- Use object pooling for bullets if you have many enemies
- Disable shadows on bullet lights
- Keep particle effects simple
- Consider using LOD (Level of Detail) for enemy models 