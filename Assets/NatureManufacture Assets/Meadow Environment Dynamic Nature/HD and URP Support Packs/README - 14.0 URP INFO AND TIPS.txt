BEFORE YOU START:
- you need Unity 2022.3+ with URP 14.0 Im not sure how many upper versions it will support without changes.
- import URP 14.1 support pack so it will replace materials, prefabs, shaders into URP compatibile version.

You can improve FPS amount by 30% if you change rendering path from forward to deferred at rendering setting. 
Find File "UniversalRenderer" and change Rendering path from forward to deferred. Forward render is ok too but it's slower for big open scenes.


