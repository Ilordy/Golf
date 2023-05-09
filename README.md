# Angry Golf
Unity Game  
Running on Unity 2020.3.30f1  
  
## [Scripts](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/)

#### [GameEvents](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/GameEvents.cs)
- Contains all custom events for the game.
#### [Manager](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/Manager.cs)  
- Handles most of the game logic and the entire game loop.  
#### [UIManager](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/UIManager.cs)
- Handles most of the UI components.
#### [ShopManager](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/ShopManager.cs)
- Separates some of the UI handling from the main UIManager to reduce complexity.
#### [AudioManager](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/AudioManager.cs)
- Handles all audio playback through event calls.
#### [EnemyClass](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/EnemyClass.cs)/[Enemy](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/Enemy.cs)/[ShieldEnemy](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/ShieldEnemy.cs)/[CartEnemy](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/CartEnemy.cs)
- These scripts handle the behaviors of all three enemy times. EnemyClass is the base class from which each inherits.
#### [Projectile](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/Projectile.cs)/[PowerUpProjectile](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/PowerUpProjectile.cs)
- Handle behaviors of regular and power up projectiles.
#### [PowerUpAnimation](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/PowerUpAnimation.cs)
- Handles the playback of the animation for the power up activation.
#### [Cosmetic](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/Cosmetic.cs)/[CosmeticItem](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/CosmeticItem.cs)
- Defines data and behaviors for cosmetics.
#### [FragSelfDestruct](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/FragSelfDestruct.cs)/[TestSpin](https://github.com/rmagikz/Golf/blob/main/Golf/Assets/Scripts/TestSpin.cs)
- Miscellaneous scripts.
