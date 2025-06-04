Commonly used scripts that are meant to be added to unity projects as module.

## Dependencies
* UniTask - download release from [here](https://github.com/Cysharp/UniTask/releases)
* Sirenix Odin - add as package

## Adding as submodule
in git bash :
`git submodule add https://github.com/Whaledevelop/Whaledevelop.Main.git Assets/_Whaledevelop.Main`

## Basic project setup
- Create Whaledevelop/Installers/ServicesInstaller
- Create Whaledevelop/Services/UIService for uGUI or Whaledevelop/Services/UIToolkitService for UIToolkit. Create Canvas prefab for uGUI and UIDocument prefab for UIToolkit
- Create Whaledevelop/Services/GameStatesService
- Create Whaledevelop/Services/GameSystemsService, create Whaledevelop/Systems/GameSystemsConfig for it and add reference. There will be place for new systems.
- Add all created services to ServicesInstaller
