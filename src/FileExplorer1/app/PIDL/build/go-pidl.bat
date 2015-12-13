echo copy ..\..\common\build\*.xml /y
..\..\common\tools\nant\nant.exe import build -buildfile:pidl.build -logfile:compile.txt 
pause