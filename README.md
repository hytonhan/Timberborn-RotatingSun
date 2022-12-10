# Timberborn-RotatingSun
This plugin for Timberborn enables for the in-game Sun to rotate around the map instead of being tied to the player camera.
Also allows optionally to make Sunflowers make rotate with the Sun, even though this is unrealistic. Pretty cool tho.

# Usage
Once you have downloaded the plugin, there are two ways to configure the plugin. One way is to boot the game and close. 
After the first boot up there should be a configuration file in the BepInEx/configs folder for this plugin. In the config
there are two options, one that enables the Sun to rotate and the other enables the Sunflowers to rotate.

It is also possible to control these settings via the in-game menu. The Plugin adds a menu button, like below.

![MenuButton](https://raw.githubusercontent.com/hytonhan/Timberborn-RotatingSun/main/package/MenuButton.PNG?raw=true)

The button will open up a settings menu, example below.

![OptionsMenu](https://raw.githubusercontent.com/hytonhan/Timberborn-RotatingSun/main/package/OptionsMenu.PNG?raw=true)


# Installing 
Recommenmed way to install the plugin is through [Thunderstore](https://timberborn.thunderstore.io/). You can install this plugin manually by cloning the repo, building it
and adding the dll to your bepinex plugins folder. This plugin is dependent on the magnificent [TimberAPI](https://github.com/Timberborn-Modding-Central/TimberAPI).

# Changelog

## v5.0.0 - 7.12.2022
- Rewrote Sun logic so that it should be much smoother

## v4.0.0 - 16.11.2022
- Got rid of BepInEx
- Changed configs to TimberAPI format

## v3.0.0 - 23.9.2022
- Updated to work with TimberAPI v0.5

## v2.2.0 - 10.9.2022
- Added Moonlight!
- Tweaks to the timings of the Sun's angles

## v2.1.2 - 1.9.2022
- Added japanese translations

## v2.1.1 - 7.8.2022
- Fixed incorrect label in option menu

## v2.1.0 - 7.8.2022
- Added alternate Sun path for Drought
- Added option to change Sun low and high angles

## v2.0.0 - 20.4.2022
- Updated to work with Update 2

## v1.1.1 - 15.4.2022
 - Minor tweak to Suns' rotation at dusk

## v1.1.0 - 15.4.2022
 - Added In-game option menus back

## v1.0.1 - 14.4.2022
 - Removed option menus due to errors

## v1.0.0 - 14.4.2022
 - Initial release with two features
	1. Enables Sun to rotate around the map
	1. Enables Sunflowers to face to Sun