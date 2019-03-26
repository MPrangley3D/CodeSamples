# CodeSamples
Some highly commented code samples from projects, to share and elaborate on the design concepts and processes

All the comments tagges [NOTES] are not comments I would ever ship in production code,they're here to elaborate on my thought process

I've posted the player controller from my 2D mobile platformer, because I made a few choices there that I was pretty happy with design-wise.  I utilized preprocessor directives to allow for very simple cross-platform development.  I utilized manager classes and interfaces.  The purpose of the project was actually just learning how to implement the UnityAds API to run video ads.  I may revisit it at a later date and re-skin it with commercially viable art (currently using free dev art assets) and add additional levels.  I also learned how to work with the new changes to the Unity Tilemap tool.

This one is actually live on the app store, though it's just one level:  https://play.google.com/store/apps/details?id=com.CodeGoblinGames.DungeonEscapeDemo&hl=en

The other project I've gone through is my procedural level generator.  This one was inspired by a blog I'd read about the generation methods used in Spelunky.  I wanted to develop something similar, and I was curious to see whether I could make it work through raycasting.  I've uploaded all the scripts, as well as a unity package that contains the entire project.  You'll want to open the "main" scene from the _Scenes subfolder.

I wanted to send the whole things over, because one of the highlights of this project in particular is a lot of the added features to the Unity Editor side.  The addition of Icons on the spawner objects, the decoration of the scripts at the editor level, etc.  This framework was designed to be highly editable, all the spawner objects use dynamic arrays, and the spawn logic for the items is all uniform, which keeps it very clean.  Long scope plans for this are to build something inspired by the old Japanese classic 'Tower of Druaga' and it would effectively be an infinite tower climb roguelike game.  Next steps will be a decent player controller, because the current one is extremely rudimentary.
