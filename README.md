# Diablo_Style_Inventory

#	Overview

	This is a small project I've made for my portfolio. It's a Diablo 2 inventory copy with the identical features.

#	Game behaviour

	- auto-pickup when closed 
	- auto-equip when closed and suitable slot is empty
	- only chosen weapon types can appear in hands' slots together (e.g. 
		one-handed + shield; bow + arrows and so on)
	- two-handed weapon occupy both hands' slots and can be retrieved from either one
	- hands' slots have alternative pair of slots for quick weapon swap
	- autoscalable slots - all slots will have screen sizes depending on their size in cells
		with common cell size, same is true for items
	- item gets dropped into world if clicked outside inventory panel while cursor-carrying one
	- item gets dropped into world immediately on pickup if inventory is slosed and full
	- if cursor-carried item overlaps only one item in a storage - it will replace it on click
		if more than one - operation will not be possible
	- If you equip a two-handed weapon when there're already two items equipped in both hands  
		or you equip an item that cannot be paired with already equipped one in another hand
		item from the second slot will be put in a backpack automatically if there's a place for it. 
		Otherwise you will not be able to do it without rearranging your items manually
	- all the cases from above are reflected via highlighting - it shows whether you can put an item having certain 
		conditions or not by color and shows a potentially replaced item's projection instead of a carried item's 
		projection if there's a possibility to replace one 

#	API

	InventoryController.cs manages input and output of the system. First is made by serialized input handlers for each action, second
		is made by a series of special event handlers, that describe system's behaviour via events and provide last arguments raised anytime in between:
	- Pickup/Equip/Upequip/Drop of an item
	- Highlight area changes
	- Opening/Closing of an inventory, Switching weapons
	
#	Prefabs

	- Inventory - standard layout inventory
	- Container - simple one-item container
	- Paired Container - subversion that should be connected with another container via serialized reference 
	- Multi Item Container - "main storage" of standard layout inventory. Cell-divided space
	- Weapons swithcer - containes two serialized lists of containers to switch between 

#	Asset menu (Right Click -> Create -> Scriptable Object -> Inventory -> ...)

	- Item Data. Template of an inventory appearance of an item. Sprite, size, fit rule, pair rule, additional sprite scale, size in cells
	- Fit Rule. Part of an item template. Compared to slot's fit rule to determine, whether an item can be equipped there
	- Fit Type. Content of a fit rule. A single one or a list of types that are associated with that rule. "All" can be checked as well
	- Pair Rule, Pair Type. Same as the above but a subversion that adds pairing logic for paired slots and items that goes there
	- Int Size. Just a serialized Vector2Int. Used to describe containers' and items' size in cells 

#	Custom inventory layout notes
	
	- all containers must be added to ContainerManager serialized list in order to function
	- first in the list should be the one you consider a default storage/backpack
	- paired containers should be paired to one-another - no chaining availible
	- switchable containers should be added to a container switcher (only one switch button is availible at this moment
		so if you want several switchable groups on different buttons - go edit InventoryController.cs SwitchWeapons method)
	- autosizing for containers is performed by FixedRatioForScreenRects.cs that makes all the work in editor mode automatically
		once you provide it with ContainerManager reference and check "Execute" box
	- ScreenRect instance on the core "Inventory" object is there to detect whether a cursor is overlapping inventory panel
		so InventoryController knows whether it should drop an item on click or just do nothing
	
     Have Fun! Ivan Kryuchkov
