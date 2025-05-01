[Setup]
AppName=UnitDesk
AppVersion=1.0.0
DefaultDirName={pf}\UnitDesk
DefaultGroupName=UnitDesk
OutputBaseFilename=UnitDeskSetup
OutputDir=.
Compression=lzma
SolidCompression=yes

[Files]
Source: "publish\\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\\UnitDesk"; Filename: "{app}\\UnitDesk.exe"
Name: "{group}\\Uninstall UnitDesk"; Filename: "{uninstallexe}"
