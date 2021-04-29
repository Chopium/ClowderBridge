# ClowderBridge

## See it Live Here: https://colter.us/

ClowderBridge is a plugin for syncing Unity data objects with an open-source REST-API server called Clowder. 

It brings asynchronous networking, data collection, and other features to Unity, and is intended to help artist, researchers, and developers make networked software easier.

It has features like anonymized user tracking through device hashing, admin/user privileges, and a master switch for shaping the online space.

## Getting Started

Each synced object needs a manager for handling unity-side submission/fetching/updating/applying. 

Making new objects requires making a new Clower dataset, linking its address in the project, and writing out a serializable C# class.
The attached project has a demo. 


## Credits (still populating)

https://seansleblanc.itch.io/better-minimal-webgl-template (WebGL HTML Template Used)

https://github.com/popcron/gizmos (Allows Unity Gizmos to appear in built programs)

http://wiki.unity3d.com/index.php/Singleton (Unity Singleton Pattern) 

