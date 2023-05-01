# Synthesis
🌌 C# WPF Application for external management of Penumbra data files

The application aims to be a friendly alternative to tagging and folder assignments by providing an interface with user-defined choices and presets.

## Features
- View and search through all mods on interface with several filter options:
	- Sort by Missing Tags (Mod contains no tags)
	- Sort by Unassigned (Mod has no folder assignment)
- Customizable UI options for tags and presets
- **Tagging:** instantly assign tags to a mod configuration file by using interface buttons
- **Folder Assignments:** folder options based on tag selection and instantly assign folder selection to mod configuration
- **Presets:** Predefined user-configurable tag and folder assignments for common mods
- **Sort Mode:** Shows folder selection for a selected item based on its existing tag, skipping the tag feature.
- **Refresh Tag Data:** refresh tag & preset configuration without needing to restart the application

## User-defined Content

Tags and presets can be defined. by modifying **page_tags.json**.
If this file is not on your root folder, please run the application at least once to let it auto generate.

For additional tag options, add additional strings in a list-style between the brackets.
>["exampleTag", "exampleTag2", "exampleTag3"]

A total of 42 tag options can be defined per page. If the number of tag options exceeds 42, then additional pages will be created for all remaining options.
```json
 {
        "head": ["veil","ears","mask","hat","crown"],
        "body": ["exampleTag"],
        "options": ["exampleTag"],
        "legs": ["exampleTag"],
        "shoes": ["exampleTag"],
        "hands": ["exampleTag"],
        "accessories": ["exampleTag"],
        "animation": ["exampleTag"],
        "bdsm": ["exampleTag"],
        "hair": ["exampleTag"],
        "piercing": ["exampleTag"],
        "swimsuit": ["exampleTag"],
        "extras": ["exampleTag"],
	"presets": [
		[
		"Dress",
		"! Upper Wear/Dress/",
		"body",
		"dress",
		"cute"
		]
        ]
    }
```
Presets are user-configurable buttons that automatically assigns the tags and folder directory that is assigned in its JSON definition.

The title **must always** be the first element in the array, and the second **must always** be the folder directory. All elements after the second index will be configured as tag assignments.

To define additional presets, add additional arrays at the end of the last element. 
An example is shown below:
```json
"presets": [
			[
				"Dress",
				"! Upper Wear/Dress/",
				"body",
				"dress",
				"cute"
			],
			[
				"Additional Button Title", 
				"Additional Folder Dir",
				"Tag1",
				"Tag2",
				"Tag3"
			]
        ]
```
## Frequently Asked Questions
> I finished tagging and assigning a folder to a mod, but only the tags are set and the folder has not changed.

Folder assignments are not generated until you save. `File > Save`

An output directory and file will be generated in your Penumbra application data folder:
`%appdata%\XIVLauncher\pluginConfigs\Penumbra`

Replace the sort_order.json located in the output folder onto the root Penumbra directory, then disable and re-enable Penumbra in-game for the changes to reflect.

>I do not have a page_tags.json file!

Run the application at least once and it will auto-generate in your application root directory.

>How do I add tags or presets to the page_tags.json file?

Reference the User Generated Content section of this article for instructions and examples.

>What is the use of the primary symbol exclusion and the new mod folder settings?

Both of these settings are used for *Sort by Unassigned* filter which will use these settings to check if the mods are in the newly imported folder or if they are in a folder which are not the sorting folders.

This is useful if you have old folders being organized into newer ones, as you are in the process of properly organizing your mods.

> My folder assignments have completely reset to what they were, despite saving and replacing the sort_order.json file!

Penumbra will automatically update the sort_order.json file whenever you modify the mod in the tree.

It is highly recommended to follow these steps when replacing the sort file in order to preserve your changes:
1. Save all changes `File > Save`
2. Disable Penumbra through `/xlplugins`
3. **Copy** the sort_order.json file into your Penumbra root directory and replace.
4. Enable Penumbra through `/xlplugins`



