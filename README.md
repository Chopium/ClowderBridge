# ClowderBridge

## See it Live Here: https://colter.us/

ClowderBridge is a plugin for syncing Unity data objects with an open-source REST-API server called Clowder. https://clowder.ncsa.illinois.edu/clowder/

It brings asynchronous networking, data collection, and other features to Unity, and is intended to help artists, researchers, and developers make networked software easier.

It has features like anonymized user tracking through device hashing, admin/user privileges, local storage/backups of datasets, and a master switch for shaping the online space.

It can also load videos, pictures, and Unity Asset Bundles. 3D model loading can be handled through Trilib, but as that's a paid Unity package, the function has been stripped. 

## Getting Started

Note: For this example project, please grab your own copy of Demigrant's DoTween Library. This is only for the UI. 

Each synced data object needs two things: a serializable C# class container, and a manager for handling unity-side submission/fetching/updating/applying. These will be extensions from a base class, with function overrides to handle specifics. 

Making new objects requires making a new Clower dataset, linking its URL in the project, and writing out a serializable C# class.
Each app will also need a unique API key. The attached project has a demo, but will require you set up your own account or server instance. 

The code can be modified to target a different REST-API server than Clowder, but the API calls will have to be matched. 


## Credits (still populating)

Cyreides (3d modelling and art)

JewelBlind (Sound and Music)

VoidGoat (additional Code)

Clowder Team (support)

https://seansleblanc.itch.io/better-minimal-webgl-template (WebGL HTML Template Used)

https://github.com/Kodrin/URP-PSX (URP PSX Rendering Pipeline)

https://rsms.me/inter/ Inter (Font)

http://wiki.unity3d.com/index.php?title=JSONObjectJSONObject (JSON Serialization/Deserialization Utility)

http://dotween.demigiant.com/ Demigrant's DoTween Library

