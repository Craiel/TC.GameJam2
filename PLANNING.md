## Game
Quick description:
Final fight/Streets of rage clone

Special features:
- Exchangable bionic limbs
	- Scavenging from mobs
- Breakable objects
	- Punch enemy through windows/walls
- Elaborate death animations
	- Environment context
- Fatalities :-D
- Steal - critical block
- Health regen
	- need to make enemies not explode, then excrete enzymes, then suck up biomass
	- biomass tank is used to regen health
- bionic parts wear out, so you need to replace them constantly
	- need to make enemies not explode
- enemies explode if you fight them
	- need to use environmental contextual attack to immobilize them to perform
	  operation
- effects
	- exploding, imploding, shattering, disintegrating, burning



# Codewise work
- 2.5D camera viewport
	- viewport has margins that, when stepped in by player, moves the viewport
	- optional feature: pan vertically
- Player controller
	- Left right up down
	- Jump, Attack, grab
		- Grab combo is dependent on directional modifier
	- Attack yields standard 4-hit combo
	- Jump & attack
	- Jump + down attack
	- Doubletap dash // ?
	- CLearance special - damages player, but clears mob and is unblockable
	- Each attack in combo successively increases damage (x2)	
- Enemies
	- Stagger on hit
	- Block?
	- 3 types:
		- Common mob
			- 3 hit combo
		- Heavy dude
			- Single hit w tell
		- Quick/weak punk
			- Single hit
	- Boss
	- Enemies deaths:
		- explosions
		- standard
	- AI		

# Assets work
- Test stage
	- Model, texture, lights
- Player
	- Model, rig, animations, texture
- Enemies
	- Model, rig, animations, texture
- Start menu?
- Soundtrack
- Sounds
	- Punch empty
	- Punch connecting
	- Pain groan 3x
	- Death sound x3
	- Jump
	
	
	
	
	# Work queue
## Code
- 2.5D camera
- Player controls
- Main game logic
- Collision detection
- Hit detection
	- Basics of melee attack
	- Block handling
	- Hit reactions
		- Stagger on  hit
- Combo system
	- Interrupts
	- Combo timeout
- Enemy AI
	- Common mob
* Breakable objects
	- Obstacle
* Ragdoll physics
* Enemies explode, unless env.context. attack is used
* Gross health regen/biomass suckup
* Enemies have biomass rating aside HP
* Biomass can be used for other things than healing
* Wave mechanics/checkpoints
	- Beat everybody before moving on


## Asset work
- Placeholder art
- Stage
- PC
- Common mob
- Heavy mob
- Boss
- Quick mob
- Material
* Textures
* More limbs


## Design
- Assembling placehold art on the stage
* Waves


## Sound work
- Soundtrack
- Sounds
