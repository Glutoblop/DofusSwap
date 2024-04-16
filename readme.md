# Dofus Swap Multi-Account Helper <img src="https://raw.githubusercontent.com/Glutoblop/DofusSwap/master/Icon/Swords.png" width=50 height=50/>
  
### Disclaimer

    From what I can tell Ankama does not have a procedure
    to be able to confirm if a desktop application is or isn't
    against their terms of service.  
    
    This program only provides hotkeys which simulate the behaviour  
    of Alt+Tab'ing, however I cannot guarentee  
    Ankama would not disagree.  
      
    Because of this, you are using this application at your own
    risk.  
 
[<img src="https://raw.githubusercontent.com/Glutoblop/DofusSwap/master/Wiki/res/download_here.png" width=600>](https://raw.github.com/Glutoblop/DofusSwap/master/Downloadables/dofusswap_1.1.8.zip)

The full source of this application is available under MIT License: 
https://github.com/Glutoblop/DofusSwap     

## How it works
This is a very simple WinForms app which uses [Win32 hooks](https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-keyup) to detect key presses, and will check against those key presses to see if they they match your registered hotkeys.  
If it does, it will consume this keypress and find the Dofus windows process running, and essentially Alt + Tab into that window for you by using virtual keys to pretend to pressing Alt + Tab.  
It uses [SetForegroundWindow](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setforegroundwindow), [SwitchToThisWindow](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-switchtothiswindow) and [BringWindowToTop](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-bringwindowtotop) in winuser.h to preform the focusing of the window.  
While simulating pressing the Alt key using this keycode: [0x12](https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes) submitting to [keybd_event](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-keybd_event)

### Adding Characters
<img src="https://raw.githubusercontent.com/Glutoblop/DofusSwap/master/Wiki/res/add_character.gif" width=700> 


### Reordering Characters Quickly
If your characters change initiative order due to not all being full health, you can quickly drag them around and line up the hotkeys to the new initiative order.  
  
<img src="https://raw.githubusercontent.com/Glutoblop/DofusSwap/master/Wiki/res/drag_characters.gif" width=700> 
  
  
License:
Copyright 2024 Glutoblop

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WIT
