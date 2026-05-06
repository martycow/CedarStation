Here's my pet-project video game called Cedar Station.

For now, it's just my field of experiments. An attempt to build something from scratch and on my own.

===== Logs =====
05.06.2026
Not much to say. I have basic project code which is not bound to any specific genre or visual style. Because, well, I don't know what I build.

First of all, a Cedar Container
It's just a simple DI (Dependency Injection) framework. My attempt to understand how Zenject-like plugins work. I put it in a dedicated assebly which doesn't know anything about Unity Engine/Editor, so theoretically I could use it anywhere else, not just in Unity. I don't know why I did this lol, basically I just wanted to play around with Assembly References etc.
The point of using DI is to use it to create, manage and destroy upper-level systems like PlayerController, LevelManager, SaveManager from a single entry point. I take over the full control of system creation process, including passing required injections (let's say, PlayerController required PlayerSpawner) through the contstructor or [Inject] attribute. I also added a check to prevent circular dependencies loop. 

Cedar Logger
Nothing special, just a wrap around Unity's Debug.Log with support of different colors. When I create a new system, I add a new entry into SystemTag enum, so now I can choose the color of text in settings scriptable object. After that, I simply can use:

  var logger = new CedarLogger(LoggerSettings);  <-- I consider LoggerSettings as a top-level asset dependency, so I drag-n-drop it on ApplicationScope object on Application scene
  
  logger.Line();  <-- Just outputs a line filled with a specific symbols and having a specific string length. 
  logger.Info(SystemTag, string);  <-- General log message. Adds [SystemTag] in front of the message;
  logger.Warn(SystemTag, string);  <-- Same but for warnings. I barely use them
  logger.Warn(SystemTag, string);  <-- Same but for errors.
  logger.Success(SystemTag, string); <-- Log message which tells you that something was done right. Adds [Success] [SystemTag] in front of the message. Always green
  logger.Fail(SystemTag, string) <-- Same but if everything went not as expected. Adds [Fail] [SystemTag] in front of the message. Always red
  
