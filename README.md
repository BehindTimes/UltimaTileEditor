ALWAYS BACKUP FILES BEFORE USING THIS

This requires .NET 10.0. (https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

This is a quick utility for extracting and inserting tiles into various Ultima games, currently Ultima 1-5 PC versions.  No enhanced or modified versions are currently supported, and this will only use the default tile set.  For example, with Ultima 5, it supports both CGA and EGA, but this currently will only support the EGA tile set.

Usage:

Set the mode of the tiles you want to work with on the left side of the application.
Set the directory of the data files you're using (typically the game location).
Set the directory you want the image files to work with.

Extracting will generate a .png file for you to edit with your preferred image editor.

Stay within the bounds of EGA for Ultima 1, 4, and 5 and CGA for Ultima 2 and 3.

EGA Colors:

* Black (#000000)
* Blue (#0000AA)
* Green (#00AA00)
* Cyan (#00AAAA)
* Red (#AA0000)
* Magenta (#AA00AA)
* Brown (#AA5500)
* Light Gray (#AAAAAA)
* Dark Gray (#555555)
* Bright Blue (#5555FF)
* Bright Green (#55FF55)
* Bright Cyan (#55FFFF)
* Bright Red (#FF5555)
* Bright Magenta (#FF55FF)
* Bright Yellow (#FFFF55)
* White (#FFFFFF)

CGA Colors:

* Black (#000000)
* Cyan (#00AAAA)
* Magenta (#AA00AA)
* Light Gray (#AAAAAA)

All invalid colors will be treated as black.

Notes:

Ultima 1:
* EGA Title Screen has a weird palette.  It does not support Cyan, Magenta, Red, nor Yellow.

Ultima 2
* The tiles are stored in the executable.  Whereas with upgrades to the other games, this tool could potentially still work, as the files are separate, this will not be true with Ultima 2.

Ultima 3
* The Lord British Signature can be extracted to a file. Unfortunately, there's no easy way to insert it back in without a lot of extra user input.

Ultima 5
* The character sets are monochrome. Only white and black are supported colors with those.

Ultima 4: Threat of the Trinity
* Being a fan game, there are new files here.  Select Ultima 4 from the program, but you might need to rename files (case-sensitive) to valid Ultima 4 files to access the appropriate extractor/compressor (RLE, LZW, etc.).  And after the appropriate extraction and insertion, you can just rename them back to the original name.
