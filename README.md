# Diablo_Style_Inventory
Overview
	- a diablo-style inventory with identical capabilities (except for 
		quick slots for potions, which are not implemented yet)

Features

    Game behaviour

	- auto-pickup when closed 
	- auto-equip when closed and suitable slot is empty
	- only chosen weapon types can appear in hands' slots together (e.g. 
		one-handed + shield; bow + arrows and so on)
	- two-handed weapon occupy both hands' slots and can be retrieved from either one
	- two hands' slots pairs for quick swap
	- autoscalable slots - all slots will have screen sizes depending on their size in cells
		with common cell size. (set 'execute' on FixedRatioRectTransform script on 
		Inventory Game Object to false if you don't need this feature, or remove
		items you want to exclude from 'ItemContainers' serialized array.)

    API

	- easy input - single class InventoryController with "default" option (when 
		no input needed whatsoever. hover over 'Control' in inspector to
		see the default keys. Use InventoryController's  public methods 
		for custom control)
	- easy output - everything is being translated to InventoryEventsManager's events. 
		Latter contain all important references and coordinates
	- all you need is to supply InventoryItemData to InventoryController's 
		AddItem method when weapon object is being picked up in a game world. 
		InventoryItemData will determine weapon's representation in inventory-related space.
	- all other stuff is fully controlled by inventory system unless item is dropped
		back to a world, which you will get notified about by OnItemDroppedIntoWorld
		event with InventoryItem object and screen space coordinates
		
		InventoryItemData contains:
			- Sprite
			- Size in cells object
			- FitRule (determines, which slots can be equipped to. e.g 
				Hands, Body, Ring, Consumable and so on)
			- Instead of FitRule hands' items contain it's subclass
				FitAndPairRule, which contains PairTypes. Latter determines
				item which it can be matched with (e.g. OneHandedWeapon 
				can be paired with Shield and OneHandedWeapon, Bow - with Arrows 
				only and so on)
			- ImageScale range, that can add scale to item's image if native 
				size is unhandy

			* InventoryItemData, FitRule, FitType, FitAndPairRule, PairRule and IntSize
				objects can be created via:
				right-click -> Create -> Scriptable Objects -> Inventory

     Prefabs

	- Standard Inventory prefab contains fully functional inventory
	- Item Slot - single item slot
	- Item Storage Space - celled space for items
	- Paired Item Slot - hands slots, that should reference each other since
		their content is codependent
	- TestObject contains UI for interactions with inventory (pickUpItem, open/close, switch slots)
		as well as two windows printing events data. smaller one - for minor stuff such as
		new highlights. larger one - for major events like ItemEquipped and so on.

	* Don't forget to set [SerializedField] references on newly added to scene objects
	
     Have Fun! Ivan Kryuchkov
