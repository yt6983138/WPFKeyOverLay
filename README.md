# WPFKeyOverLay
A cool key overlay for osu mania, lr2, o2jam, quaver and other games! <br/>

Note: you will need .Net runtime 8 to run this app
# Features
Incredibly customizable! You can add any WPF component at `Default.xaml` (Top layer overlay UI) 
and `ForEachKey.xaml` (Overlay UI for each key), and so many settings available in `Config.json`. <br/>

Note: You may need some coding skills to add your custom component in those xaml files.
## Hotkeys
By default, there are 5 hotkeys:
1. Numpad Subtract: Clear statistics
2. Numpad Multiply: Toggle click through (make your mouse be able to click through overlay)
3. Numpad Divide: Toggle always on top
4. Numpad NumLock: Toggle hiding
5. Numpad Add: Close overlay

You can modify those settings in `Config.json`.

# Settings
Color format: There are 2 types of format you can use to specify a color, RGB (ex. `#114514`) 
and ARGB (ex. `#19198100`), where the first 2 hex digits are used to specify alpha channel. <br/>

1. TrailPixelPerSecond: How fast the trail would go
2. KpsUpdatesPerSecond: How fast would the kps/peak kps update
3. DefaultHeight: The height of the window (hint: you can resize its height with mouse)
4. TrailUpdateFps: The fps of trail
5. WindowOpacity: The opacity of the window (this affects everything)
6. KeyButtonHeight: The height of the buttons 
7. WindowBackground: The background color of the window (transparency does not affect other things)
8. WindowPadding: The padding of the window
9. Keys: The keys that will be used (see Key Info section below for more info)
10. ExternalUISource: The top overlay for the overlay, set to `null` to disable
11. ForAllKeysUISource: The overlay for each of the keys, set to `null` to disable
12. SpecialHotKeys: Special hotkeys, see Hotkeys section above for more info

## Key Info
1. Width: The width of the key
2. KeyboardUpButtonColor: The color of button before you pressed the keys
3. KeyboardDownButtonColor: The color of button after you pressed the keys
4. TextColor: The color of the text
5. TrailColor: The color of the trail
6. TrailBackgroundColor: The background color of the trail
7. TrailMargin: The margin of the trail
8. ButtonMargin: The margin of the button
9. Key: The key to be used
10. Text: The text that will be used to display on button