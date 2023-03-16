# magica-cloth-bug-repro

To see bug open `BugReproScene` and start the scene.

`BugRepro.cs` do the following:
- Instantiate Head and Body and binds it to skeleton
- Disable Head GameObject
- Instantiate CustomHead and bind it to skeleton

Bug can be seen on the right HairTail, that is not properly updated by MagicaCloth simulation.

https://user-images.githubusercontent.com/25569360/225602071-5630865f-cf11-4655-a232-ef80aec07492.mov
