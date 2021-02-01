![Windows x64](https://github.com/int0x81/Atratinus/workflows/Windows%20x64/badge.svg)
# Atratinus
A data restructuring tool that is capable of analyzing EDGAR files provided by the SEC.
## How to run this programm
1. You need .NET Runtime 5.0 or higher to run this programm: ![Download](https://dotnet.microsoft.com/download/dotnet/current/runtime)
2. Download the programm from GitHub
   1. First go to the 'Actions' tab
   2. Click on the latest successful run
   3. Scroll to the bottom of the Artifacts section and click on the win_x64 artifact
3. Once downloaded, extract the zip-folder on your computer.
4. Place the settings.json in the same folder as the programm. A template can be found in the repository: ![settings.json](https://github.com/int0x81/Atratinus/blob/main/templates)
5. Configure the settings.
6. Run the programm which is called Atratinus.DataTransformation.exe. You can either do it by double clicking on that, but if an error occurs the window will close immediatly
   leaving you without any clue. I suggest you open this from the command line. Here is a quick tutorial of how to do that:
   1. When you are in the folder of the proramm, click in the filepath field at the top and type in: "cmd" then press enter.
   2. A black window should now be open. Type in: "Atratinus.DataTransform.exe" then press enter.
   3. The programm should now run
