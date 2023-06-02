#a python script to delete C:\Program Files (x86)\Steam\steamapps\common\Regions Of Ruin\BepInEx\plugins\RegionsOfRuin_ConsoleCommands.dll, then move C:\Users\dalto\source\repos\RegionsOfRuin_ConsoleCommands\bin\Debug\net35\RegionsOfRuin_ConsoleCommands.dll to C:\Program Files (x86)\Steam\steamapps\common\Regions Of Ruin\BepInEx\plugins\

# FILEPATH: Untitled-1
import os
import shutil

# Define the paths to the files we want to move and delete
source_file = r'C:\Users\dalto\source\repos\RegionsOfRuin_ConsoleCommands\bin\Debug\net35\RegionsOfRuin_ConsoleCommands.dll'
dest_file = r'C:\Program Files (x86)\Steam\steamapps\common\Regions Of Ruin\BepInEx\plugins\RegionsOfRuin_ConsoleCommands.dll'
delete_file = r'C:\Program Files (x86)\Steam\steamapps\common\Regions Of Ruin\BepInEx\plugins\RegionsOfRuin_ConsoleCommands.dll'

# Delete the existing file
os.remove(delete_file)

# Move the new file to the correct location
shutil.move(source_file, dest_file)
