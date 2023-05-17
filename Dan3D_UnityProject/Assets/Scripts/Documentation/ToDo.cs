/*

//Current
-Interior Area
-Add music
-Bedroom
-Sleeping logic, wake up
-Go wake up Zim ( sleeping in bedroom )
-Go gather resources for crafting ( 3x stone, 3x wood, resource for food )
-Get back to make food in cauldron
-display controls on some corner ( sprint, frying pan, interact, inventory )

-Build two more areas and interior
-Create area system to load different areas and player position based on previous area
-UI pop ups next to tree and stone to show it has resources
-Enemy temp critter model
-UI box to quest ( collect resources, go back to house to prepare dinner )

-Pay for Appstore to show games
-Pitch ( core game idea, loops, how much money I need, tell about myself, financials, why to fund this game ( genre, numbers etc ) I am working on this game, because I want to play a cozy game, but with a bit of drama and purpose, recognise failure of DWT pixel art, game features, marketing, doing it solo )

-Wake up in the house
-Get popup to prepare food for your family
-Go out, collect bunch of resources ( collect carrots, gingerbread etc.), collect specific resources to complete it ( always at the top )
-Fight enemies with frying pan ( popup saying that enemies don't do anything for now, but your chest will run away and you lose items you collected )
-Once collected, go back to house, kitchen, simple, just press button
-Have lunch with them ( dialoge or popup with temp dialogues )
-Go to Tavern shopping window where you can trade, recipes, items, blueprints for new rooms, decorations etc.

-Create scene with house, put there bunch of critters, fox, etc. all hanging around in the house
-A lot of food resources outside in the forest ( carrots, beats, gingerbread etc )


What things need to be implemented
-House Exterior, House Interior, Kitchen ( Bedroom, Crafting Room )
-Sleep interation ( bed )
-Demo popups with texts
-Food resources ( carrots etc )
-Enemies models, steal item logic ( hard limit on how much they can steal )
-Recipes in Inventory
-Update Inventory UI
-Kitchen cooking, one button
-Family table, all sitting around, eating
-Tavern trading visuals ( no logic )



-Gameplay shot with Zim, Sun animated
-Twitter, Reddit, Tiktok, Insta



//Area logic (what area it is, load surrounding areas)
//Map (ui, display position)
//Areas loading additively. Setup Home area, left, right area, forward and backward area.
//Setup area map, first index sets y direction ( 0 default where home is, 1, going forward, -1 gobing backwards)
//So 0,1 is first area on the right of home. 0,-4, is fourth area on the left of home. 2,3, is two lanes forward from hom and 3 to the right
//Have UI map, display position

//Setup home, enter home, 3d temp art, logic. Probabaly have some map for home, do room upgrades there? how to build new rooms and upgrades

-Add Zim, just running around.
-Add basic Astar navigation

//AI for characters to move between areas and in the area.

// LATER
-Unity Pathfinding
-Add animated enemy
-Enemies should not be attacking, but more messing with Dan, running around, scaring Chompy. When chompy is scared he runs away
-Implement Chompy
-Chomp logic, character, can run away
-Zim running around collecting things. when he runs to something like a flower, you can press a button to collect as well, even faster
-Probably show how many pieces of wood are there available, timer etc.
-Resource object timer, refreshing resource over time. Pop up, show how much wood is available, when player hits more, tree gets annoyed, takes longer to refresh. ( Wood, hit tree with frying pan ) Have you ever wondered how do you use frying pan to get wood and stone? Well, it's obvious, it's magical frying pan. Simple.

// NOTES
//Scare meter for Chompy? It's more like you are protecting Chompy, Zim talks about how Chomp realy does not like this. Chompy can sense
//their dark energy as he is more magical creature as well, so it affects him a bit more.
//Dan health? reduced only from survival elements or enemies also?
//Enemies should be obnoxious, running arond Dan, biting him etc or do something else? what enemies do? they don't attack


*/
